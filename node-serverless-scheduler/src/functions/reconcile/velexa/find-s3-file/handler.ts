import { middyfy } from '@libs/lambda';
import { listObject } from '@libs/s3-utils';
import console from 'console';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';

dayjs.extend(utc);
dayjs.extend(timezone);

interface Payload {
  reportPrefix: string;
  bucket: string;
  date: string;
  timezone: string;
}

const run = async (event) => {
  console.info(event);
  const payload = event.body as Payload;
  console.info(payload);
  const date = dayjs(payload.date).tz(payload.timezone).format('YYYY-MM-DD');
  console.info('Date', date);
  const reportName = `${payload.reportPrefix}_${date}`;
  console.info(`Finding Report: ${reportName}`);
  const response = await listObject(payload.bucket, reportName);
  console.info(response);

  if (!Array.isArray(response.Contents) || !response.Contents.length) {
    throw new Error(`File not found for ${reportName} in ${payload.bucket}`);
  }

  const report = response.Contents.find((q) => q.Key.startsWith(reportName));
  if (report === undefined)
    throw new Error(`File not found for ${reportName} in ${payload.bucket}`);
  console.info(`Found Report: ${report.Key} on Bucket: ${payload.bucket}`);

  return {
    body: {
      bucket: payload.bucket,
      key: report.Key,
      date,
    },
  };
};

export const main = middyfy(run);
