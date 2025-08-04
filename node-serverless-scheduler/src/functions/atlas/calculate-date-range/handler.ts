import { middyfy } from '@libs/lambda';
import console from 'console';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';

dayjs.extend(utc);
dayjs.extend(timezone);
interface Payload {
  date: string;
  timezone: string;
  dayOffset: string;
}

interface IDate {
  date: string;
}

interface IDateRange {
  dates: IDate[];
}

const run = async (event) => {
  console.info(event);
  const payload = event.body as Payload;
  console.info(payload);
  const startDate = dayjs(payload.date)
    .subtract(Number(payload.dayOffset), 'day')
    .tz(payload.timezone)
    .format('YYYY-MM-DD');
  const endDate = dayjs(payload.date).tz(payload.timezone).format('YYYY-MM-DD');

  console.info('[start date, end date]: ', [startDate, endDate]);

  const dateRange = generateDateRange(startDate, endDate);

  return {
    body: {
      dateRange,
    },
  };
};

const generateDateRange = (startDate: string, endDate: string): IDateRange => {
  const dateRange: IDate[] = [];
  const currentDate = new Date(startDate);
  const untilDate = new Date(endDate);

  while (currentDate < untilDate) {
    dateRange.push({
      date: currentDate.toISOString().split('T')[0],
    });
    currentDate.setDate(currentDate.getDate() + 1);
  }

  return { dates: dateRange };
};

export const main = middyfy(run);
