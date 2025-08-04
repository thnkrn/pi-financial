import { middyfy } from '@libs/lambda';
import { storeFileToS3 } from '@libs/s3-utils';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';

dayjs.extend(utc);
dayjs.extend(timezone);

interface Payload {
  date: string;
  timezone: string;
  filePrefix: string;
  fileDir: string;
  backofficeBucket: string;
  cmeBucket: string;
}

const run = async (event) => {
  console.info('Event:', JSON.stringify(event));

  try {
    const payload = event.body as Payload;
    const { backofficeBucket, cmeBucket, filePrefix, fileDir, timezone, date } =
      payload;

    const fileContent = '';

    const formattedDate = dayjs(date).tz(timezone).format('YYYYMMDD');
    const fileName = `${filePrefix}_${formattedDate}.txt`;
    const fileKey = `${formattedDate}/${fileName}`;
    const boFileKey = `${fileDir}/${fileKey}`;

    await storeFileToS3(
      backofficeBucket,
      Buffer.from(fileContent, 'utf8'),
      boFileKey
    );
    await storeFileToS3(cmeBucket, Buffer.from(fileContent, 'utf8'), fileKey);

    console.info('File saved to S3:', fileKey);

    return {
      body: {
        fileKey: boFileKey,
        success: true,
      },
    };
  } catch (error) {
    console.error('Error details:', error);
    throw new Error(`Failed to process: ${error.message}`);
  }
};

export const main = middyfy(run);
