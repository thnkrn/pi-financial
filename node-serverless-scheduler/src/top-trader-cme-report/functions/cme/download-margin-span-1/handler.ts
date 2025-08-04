import { middyfy } from '@libs/lambda';
import { storeFileToS3 } from '@libs/s3-utils';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';

dayjs.extend(utc);
dayjs.extend(timezone);

interface Payload {
  reportPrefix: string;
  reportURL: string;
  bucket: string;
  date: string;
  folder: string;
}

const downloadCsvFromUrl = async (url: string): Promise<Buffer> => {
  try {
    const response = await fetch(url);

    if (!response.ok) {
      throw new Error(
        `Failed to download CSV: ${response.status} ${response.statusText}`
      );
    }

    const arrayBuffer = await response.arrayBuffer();
    return Buffer.from(arrayBuffer);
  } catch (error) {
    console.error('Error downloading CSV:', error);
    throw error;
  }
};

const run = async (event) => {
  console.info('Event:', event);
  try {
    const payload = event.body as Payload;
    console.info('Payload:', payload);

    const date = dayjs(payload.date).tz('Asia/Bangkok').format('YYYY-MM-DD');
    console.info('Formatted Date:', date);

    const reportName = `${payload.reportPrefix}.csv`;
    const s3Key = `${payload.folder}/${date}/${reportName}`;

    const csvData = await downloadCsvFromUrl(payload.reportURL);

    await storeFileToS3(payload.bucket, csvData, s3Key);

    return {
      body: {
        date,
        reportName,
        status: 'succeeded',
      },
    };
  } catch (error) {
    console.error('Error in lambda execution:', error);

    throw new Error(`Failed to download CME Margin Span 1: ${error.message}`);
  }
};

export const main = middyfy(run);
