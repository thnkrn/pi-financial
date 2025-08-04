import { middyfy } from '@libs/lambda';

interface Payload {
  startDate: string;
  endDate: string;
}

interface IDates {
  day: string;
}

interface IDateRange {
  dates: IDates[];
}

const run = async (event) => {
  console.info(event);
  const payload = event.body as Payload;
  console.info(payload);

  const dateRange = getDateRange(payload.startDate, payload.endDate);

  return {
    body: {
      dateRange,
    },
  };
};

const generateDateRange = (startDate: Date, endDate: Date): IDateRange => {
  const dateRange: IDates[] = [];
  const currentDate = new Date(startDate);

  while (currentDate <= endDate) {
    dateRange.push({
      day: currentDate.toISOString().split('T')[0] + ' 01:00:00',
    });
    currentDate.setDate(currentDate.getDate() + 1);
  }

  return { dates: dateRange };
};

const getDateRange = (startDate: string, endDate: string): IDateRange => {
  const start = new Date(startDate);
  const end = new Date(endDate);

  return generateDateRange(start, end);
};

export const main = middyfy(run);
