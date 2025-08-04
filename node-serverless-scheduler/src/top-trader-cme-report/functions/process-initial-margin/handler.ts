import { middyfy } from '@libs/lambda';
import { getObject, storeFileToS3 } from '@libs/s3-utils';
import console from 'console';
import csvParser from 'csv-parser';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import { Readable } from 'stream';

dayjs.extend(utc);
dayjs.extend(timezone);

interface CmeWithMargin {
  PRODUCT_CODE: string;
  PERIOD: string;
  SYMBOL: string;
  FIRST_TRADE_DATETIME: string;
  LAST_TRADE_DATETIME: string;
  INITIAL_OUTRIGHT_MARGIN: string;
}

interface ProductConfig {
  product_code: string;
  clearing_code: string;
}

interface LambdaEvent {
  configs: ProductConfig[];
  fileKey: string;
  bucket: string;
  finalFile: string;
  reportPrefix: string;
  date: string;
  marginFolder: string;
  marginFileName: string;
  finalFilePrefix: string;
  backofficeBucket: string;
  backofficeDir: string;
  cmeBucket: string;
}

const getFileFromS3 = async (fileName: string, s3bucket: string) => {
  console.info(`Start get ${fileName} from s3 bucket ${s3bucket}`);
  const data = await getObject(s3bucket, fileName);
  if (!data.Body) {
    throw new Error(`File content is null: ${fileName}`);
  }

  if (data.Body instanceof Readable) {
    return data.Body;
  } else {
    throw new Error(`Cannot read file as stream: ${fileName}`);
  }
};

const getProductsMarginData = async (marginData: Readable) => {
  const results: Map<string, string> = new Map();
  await new Promise((resolve, reject) => {
    marginData
      .pipe(csvParser())
      .on('data', (data) => {
        results.set(
          `${data['product_code']}|${data['period']}`,
          Number(data['margin']).toFixed(2)
        );
      })
      .on('end', resolve)
      .on('error', reject);
  });

  return results;
};

const combineMarginData = async (
  configs: ProductConfig[],
  processedData: Readable,
  productMargins: Map<string, string>
) => {
  const combinedResults: CmeWithMargin[] = [];
  await new Promise<void>((resolve, reject) => {
    let rawData = '';

    processedData
      .on('data', (chunk) => {
        rawData += chunk;
      })
      .on('end', () => {
        try {
          const jsonData = JSON.parse(rawData);
          const dataArray = Array.isArray(jsonData) ? jsonData : [jsonData];

          dataArray.forEach((data) => {
            const productCode = data.PRODUCT_CODE || data[Object.keys(data)[0]];
            const period = data.PERIOD;

            const margin = findInitialMargin(
              productMargins,
              productCode,
              period,
              configs
            );
            if (margin) {
              combinedResults.push({
                PRODUCT_CODE: productCode,
                PERIOD: period,
                SYMBOL: data.SYMBOL,
                FIRST_TRADE_DATETIME: data.FIRST_TRADE_DATETIME,
                LAST_TRADE_DATETIME: data.LAST_TRADE_DATETIME,
                INITIAL_OUTRIGHT_MARGIN: margin,
              });
            } else {
              console.warn(
                `Cannot get margin for ProductCode: ${productCode}, Period: ${period}`
              );
            }
          });
          resolve();
        } catch (error) {
          reject(error);
        }
      })
      .on('error', reject);
  });
  return combinedResults;
};

const findInitialMargin = (
  productMargins: Map<string, string>,
  productCode: string,
  period: string,
  configs: ProductConfig[]
) => {
  let matched: string;
  const product = configs.find((x) => x.product_code == productCode);
  if (product?.clearing_code !== null) {
    matched = productMargins.get(`${product.clearing_code}|${period}`);
  }

  return matched;
};

const run = async (event: LambdaEvent) => {
  console.info(event);
  const bucket = event.bucket;

  const date = dayjs(event.date).tz('Asia/Bangkok').format('YYYY-MM-DD');
  const marginFileKey = `${event.marginFolder}/${date}/${event.marginFileName}`;

  const marginData = await getFileFromS3(marginFileKey, bucket);
  const productMargins = await getProductsMarginData(marginData);

  const processedData = await getFileFromS3(event.fileKey, bucket);
  const combinedResults = await combineMarginData(
    event.configs,
    processedData,
    productMargins
  );

  const buffer = Buffer.from(
    JSON.stringify(combinedResults, null, 2).replace(/"(\d*\.\d*)"/g, '$1')
  );

  const formattedDate = dayjs(event.date).tz('Asia/Bangkok').format('YYYYMMDD');
  const fileName = `${event.finalFilePrefix}_${formattedDate}.json`;
  const fileKey = `${formattedDate}/${fileName}`;
  const boFileKey = `${event.backofficeDir}/${fileKey}`;

  await storeFileToS3(event.backofficeBucket, buffer, boFileKey);
  await storeFileToS3(event.cmeBucket, buffer, fileKey);

  return {
    fileKey: boFileKey,
  };
};
export const main = middyfy(run);
