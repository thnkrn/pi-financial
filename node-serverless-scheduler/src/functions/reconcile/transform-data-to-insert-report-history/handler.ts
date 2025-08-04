import { middyfy } from '@libs/lambda';
import { v4 as uuidv4 } from 'uuid';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import timezone from 'dayjs/plugin/timezone';

dayjs.extend(utc);
dayjs.extend(timezone);

interface Payload {
  reportName: string;
  bucket: string;
  filename: string;
  status: string;
  timezone: string;
  dateFrom: string;
  dateTo: string;
  timeFrom: string;
  timeTo: string;
}

const run = async (event) => {
  console.info(event);
  const payload = event.body as Payload;
  console.info(payload);
  const dateFrom = dayjs.tz(
    dayjs(payload.dateFrom).tz(payload.timezone).format('YYYY-MM-DD') +
      'T' +
      payload.timeFrom, // Please add .subtract(1, 'day') in the future
    payload.timezone
  );
  const dateTo = dayjs.tz(
    dayjs(payload.dateTo).tz(payload.timezone).format('YYYY-MM-DD') +
      'T' +
      payload.timeTo,
    payload.timezone
  );

  const data = {
    id: uuidv4(),
    reportName: payload.reportName,
    dateFrom: dateFrom,
    dateTo: dateTo,
    status: payload.status,
    fileName: payload.filename,
  };
  console.info(data);

  return {
    body: data,
  };
};

export const main = middyfy(run);
