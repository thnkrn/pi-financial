import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';
import { Handler, S3Event } from 'aws-lambda';
import { SendSyncInitialMarginRequest } from './api';

const syncInitialMargin = async (bucketName: string, fileKey: string) => {
  const [serviceHost] = await getConfigFromSsm('set', ['set-srv-host']);
  const response = await SendSyncInitialMarginRequest(serviceHost, {
    bucketName: bucketName,
    fileKey: fileKey,
  });
  console.info('response: ', response);

  return {
    statusCode: 200,
    body: JSON.stringify({
      message: 'Initial margin data sync successfully',
    }),
  };
};

export const handleUpdateInitialMargin: Handler = async (event: S3Event) => {
  try {
    if ('Records' in event) {
      for (const record of event.Records) {
        if (record.eventSource !== 'aws:s3') continue;

        const bucketName = record.s3.bucket.name;
        const fileKey = record.s3.object.key;

        await syncInitialMargin(bucketName, fileKey);
      }
    } else {
      throw new Error('Unsupported event type');
    }
  } catch (error) {
    console.error(
      'Error processing Event. Cannot sync set initial margins:',
      error
    );
    throw error;
  }
};

export const main = middyfy(handleUpdateInitialMargin);
