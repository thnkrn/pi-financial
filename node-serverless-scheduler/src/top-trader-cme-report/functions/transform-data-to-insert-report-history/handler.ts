import { middyfy } from '@libs/lambda';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import { v4 as uuidv4 } from 'uuid';

dayjs.extend(utc);
dayjs.extend(timezone);

interface Payload {
  fileName: string;
  date: string;
  status: string;
  fileKey?: string | null;
}

const run = async (event) => {
  console.info(event);
  const payload = event.body as Payload;
  console.info(payload);
  const date = dayjs(payload.date).tz('Asia/Bangkok').format('YYYY-MM-DD');

  const data = {
    id: uuidv4(),
    fileName: payload.fileName,
    dateFrom: date,
    dateTo: date,
    status: payload.status,
    fileKey: payload?.fileKey || null,
  };
  console.info(data);

  return {
    body: data,
  };
};

export const main = middyfy(run);
