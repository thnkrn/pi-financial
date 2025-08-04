import { middyfy } from '@libs/lambda';
import { getObject } from '@libs/s3-utils';
import { getConfigFromSsm } from '@libs/ssm-config';
import { parseXmlContent } from '@libs/xml-utils';
import { APIGatewayProxyEvent, Handler, S3Event } from 'aws-lambda';
import { InitialMargin, upsertInitialMargin } from './api';

// Custom parse date function for SPAN file
const parseCustomDateString = (dateString: string): Date => {
  if (dateString.length !== 12) {
    throw new Error(
      "Invalid date string length. Expected format: 'YYYYMMDDHHMM'."
    );
  }

  // Extract the components
  const year = parseInt(dateString.substring(0, 4), 10);
  const month = parseInt(dateString.substring(4, 6), 10) - 1; // Month is 0-indexed
  const day = parseInt(dateString.substring(6, 8), 10);
  const hour = parseInt(dateString.substring(8, 10), 10);
  const minute = parseInt(dateString.substring(10, 12), 10);

  // Create and return the Date object
  return new Date(year, month, day, hour, minute);
};

const processInitialMarginFile = async (
  bucketName: string,
  fileKey: string
) => {
  const data = await getObject(bucketName, fileKey);
  if (!data.Body) {
    throw new Error(`SPAN file not found. fileKey: ${fileKey}`);
  }

  const xmlContent = await data.Body.transformToString('utf-8');
  if (xmlContent === null || xmlContent === '') {
    throw new Error(`file content empty. fileKey: ${fileKey}`);
  }

  // parsed xml content from SPAN file
  const parsedData = await parseXmlContent(xmlContent);

  // Extract data
  const initialMarginData: InitialMargin[] = [];
  const targetNode = parsedData.spanFile.pointInTime[0].clearingOrg[0].ccDef;
  const asOfDate = parseCustomDateString(parsedData.spanFile.created[0]);

  targetNode.forEach((node) => {
    const pfLink = node.pfLink[0];
    const priceScan = node.rateTiers[0].tier[0].scanRate[0].priceScan[0];

    initialMarginData.push({
      symbol: pfLink.pfCode[0],
      productType: pfLink.pfType[0],
      im: priceScan,
    });
  });

  // save initial margin data to RDS
  // save initial margin data to RDS
  const hosts = await getConfigFromSsm('tfex', [
    'tfex-srv-host',
    'market-data-proxy-host',
  ]);

  await Promise.all(
    hosts.map(async (h) => {
      const response = await upsertInitialMargin(h, {
        asOfDate: asOfDate,
        data: initialMarginData,
      });
      if (!response.data) {
        throw new Error(
          `upsert initial margin to ${h} failed. fileKey: ${fileKey}`
        );
      }
    })
  );

  return {
    statusCode: 200,
    body: JSON.stringify({
      message: 'Initial margin data upserted successfully',
    }),
  };
};

export const handleUpdateInitialMargin: Handler = async (
  event: S3Event | APIGatewayProxyEvent
) => {
  try {
    if ('Records' in event && event.Records[0].eventSource === 'aws:s3') {
      // Handle S3 Event
      const bucketName = event.Records[0].s3.bucket.name;
      const fileKey = event.Records[0].s3.object.key;

      return await processInitialMarginFile(bucketName, fileKey);
    } else if ('httpMethod' in event && event.httpMethod === 'POST') {
      // Handle HTTP POST Request
      const { bucketName, fileKey } = JSON.parse(event.body);

      if (!bucketName || !fileKey) {
        throw new Error(
          'Missing required fields in POST request: bucketName and fileKey'
        );
      }

      return await processInitialMarginFile(bucketName, fileKey);
    } else {
      throw new Error('Unsupported event type');
    }
  } catch (error) {
    console.error(
      'Error processing Event. Cannot upsert initial margins:',
      error
    );
    throw error;
  }
};

export const main = middyfy(handleUpdateInitialMargin);
