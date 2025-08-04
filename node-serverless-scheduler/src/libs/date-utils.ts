import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import customParseFormat from 'dayjs/plugin/customParseFormat';

dayjs.extend(utc);
dayjs.extend(timezone);
dayjs.extend(customParseFormat);

export type GetStartEndDateResult = { startDate: Date; endDate: Date };

export function dateToStartEndDate(date: Date): GetStartEndDateResult;
export function dateToStartEndDate(
  startDate: Date,
  endDate?: Date
): GetStartEndDateResult {
  endDate ??= startDate;

  return {
    startDate: new Date(
      Date.UTC(
        startDate.getUTCFullYear(),
        startDate.getUTCMonth(),
        startDate.getDate()
      )
    ),
    endDate: new Date(
      Date.UTC(
        endDate.getUTCFullYear(),
        endDate.getUTCMonth(),
        endDate.getDate(),
        23,
        59,
        59,
        999
      )
    ),
  };
}

export const formatDate = (
  date: string | Date,
  pattern = 'YYYY-MM-DD HH:mm:ss'
) => {
  return date === '-' ? '-' : dayjs(date).tz('Asia/Bangkok').format(pattern);
};

export function formatDateTimeUTC(
  date: string | Date,
  pattern = 'YYYY-MM-DD HH:mm:ss'
): string {
  return dayjs(date).format(pattern);
}

export function areDatesEqual(date1: Date, date2: Date): boolean {
  return (
    date1.getFullYear() === date2.getFullYear() &&
    date1.getMonth() === date2.getMonth() &&
    date1.getDate() === date2.getDate()
  );
}

export function isDateBetween(date: Date, start: Date, end: Date): boolean {
  const startDateWithoutTime = new Date(start);
  const endDateWithoutTime = new Date(end);
  const dateWithoutTime = new Date(date);
  dateWithoutTime.setHours(0, 0, 0, 0);
  startDateWithoutTime.setHours(0, 0, 0, 0);
  endDateWithoutTime.setHours(0, 0, 0, 0);
  return (
    dateWithoutTime > startDateWithoutTime &&
    dateWithoutTime < endDateWithoutTime
  );
}

export function isTimeGreaterThan(
  date: Date,
  hours: number,
  minutes: number
): boolean {
  const h = date.getHours();
  const m = date.getMinutes();
  return h > hours || (h == hours && m > minutes);
}

export function toTHDateTime(input: string | Date): Date {
  const date = new Date(input);
  date.setHours(date.getHours() + 7);
  return date;
}

export function parseDate(
  input: string,
  format: string,
  isThTime: boolean
): Date {
  return isThTime
    ? dayjs.tz(input, format, 'Asia/Bangkok').utc().toDate()
    : dayjs.tz(input, format).utc().toDate();
}
