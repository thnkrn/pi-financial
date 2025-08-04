/* eslint-disable no-await-in-loop */
import dayjs, { extend } from 'dayjs';
import customParseFormat from 'dayjs/plugin/customParseFormat';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';

extend(utc);
extend(timezone);
extend(customParseFormat);

const DEFAULT_TZ = 'UTC';
// const BKK_TZ = 'Asia/Bangkok';

export const getDateStr = (options?: {
  dateStr?: string;
  format?: string;
  tz?: string;
  addDays?: number;
}): string => {
  const format = options?.format ?? 'YYYY-MM-DD';
  const tz = options?.tz ?? DEFAULT_TZ;
  const addDay = options?.addDays ?? 0;
  return options?.dateStr
    ? dayjs.tz(options.dateStr, tz).add(addDay, 'day').format(format)
    : dayjs().tz(tz).add(addDay, 'day').format(format);
};

export const getFirstDayOf = (options?: {
  dateStr?: string;
  format?: string;
  tz?: string;
  offset?: number;
  offsetUnit: 'day' | 'month' | 'year';
  firstOfUnit: 'day' | 'month' | 'year';
}): string => {
  const format = options?.format ?? 'YYYY-MM-DD';
  const tz = options?.tz ?? DEFAULT_TZ;
  const offset = options?.offset ?? 0;
  const { offsetUnit, firstOfUnit } = options;
  return options?.dateStr
    ? dayjs
        .tz(options.dateStr, tz)
        .add(offset, offsetUnit)
        .startOf(firstOfUnit)
        .tz(tz)
        .format(format)
    : dayjs()
        .tz(tz)
        .add(offset, offsetUnit)
        .startOf(firstOfUnit)
        .tz(tz)
        .format(format);
};

export const getLastDayOf = (options?: {
  dateStr?: string;
  format?: string;
  tz?: string;
  offset?: number;
  offsetUnit: 'day' | 'month' | 'year';
  lastOfUnit: 'day' | 'month' | 'year';
}): string => {
  const format = options?.format ?? 'YYYY-MM-DD';
  const tz = options?.tz ?? DEFAULT_TZ;
  const offset = options?.offset ?? 0;
  const { offsetUnit, lastOfUnit } = options;
  return options?.dateStr
    ? dayjs
        .tz(options.dateStr, tz)
        .add(offset, offsetUnit)
        .endOf(lastOfUnit)
        .tz(tz)
        .format(format)
    : dayjs()
        .tz(tz)
        .add(offset, offsetUnit)
        .endOf(lastOfUnit)
        .tz(tz)
        .format(format);
};

export const getFirstDayOfYear = (options?: {
  dateStr?: string;
  format?: string;
  tz?: string;
  addYears?: number;
}): string => {
  const format = options?.format ?? 'YYYY-MM-DD';
  const tz = options?.tz ?? DEFAULT_TZ;
  const addYear = options?.addYears ?? 0;
  return options?.dateStr
    ? dayjs
        .tz(options.dateStr, tz)
        .add(addYear, 'year')
        .startOf('year')
        .tz(tz)
        .format(format)
    : dayjs().tz(tz).add(addYear, 'year').startOf('year').tz(tz).format(format);
};

export const getLastDayOfYear = (options?: {
  dateStr?: string;
  format?: string;
  tz?: string;
  addYears?: number;
}): string => {
  const format = options?.format ?? 'YYYY-MM-DD';
  const tz = options?.tz ?? DEFAULT_TZ;
  const addYear = options?.addYears ?? 0;
  return options?.dateStr
    ? dayjs
        .tz(options.dateStr, tz)
        .add(addYear, 'year')
        .endOf('year')
        .tz(tz)
        .format(format)
    : dayjs().tz(tz).add(addYear, 'year').endOf('year').tz(tz).format(format);
};

export const getFirstDayOfMonth = (options?: {
  dateStr?: string;
  format?: string;
  tz?: string;
  addDays?: number;
}): string => {
  const format = options?.format ?? 'YYYY-MM-DD';
  const tz = options?.tz ?? DEFAULT_TZ;
  const addDay = options?.addDays ?? 0;
  return options?.dateStr
    ? dayjs
        .tz(options.dateStr, tz)
        .add(addDay, 'day')
        .startOf('month')
        .tz(tz)
        .format(format)
    : dayjs().tz(tz).add(addDay, 'day').startOf('month').tz(tz).format(format);
};

export const getLastDayOfMonth = (options?: {
  dateStr?: string;
  format?: string;
  tz?: string;
  addDays?: number;
}): string => {
  const format = options?.format ?? 'YYYY-MM-DD';
  const tz = options?.tz ?? DEFAULT_TZ;
  const addDay = options?.addDays ?? 0;
  return options?.dateStr
    ? dayjs
        .tz(options.dateStr, tz)
        .add(addDay, 'day')
        .endOf('month')
        .tz(tz)
        .format(format)
    : dayjs().tz(tz).add(addDay, 'day').endOf('month').tz(tz).format(format);
};

export const isWeekend = (options?: {
  dateStr?: string;
  tz?: string;
}): boolean => {
  const tz = options?.tz ?? DEFAULT_TZ;
  const checkingDate = options?.dateStr
    ? dayjs.tz(options.dateStr, tz)
    : dayjs().tz(tz);
  return checkingDate.day() === 0 || checkingDate.day() === 6;
};
