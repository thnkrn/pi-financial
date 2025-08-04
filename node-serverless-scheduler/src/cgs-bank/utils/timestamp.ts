import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import timezone from 'dayjs/plugin/timezone';

export enum TimestampFormat {
  Normal,
  OnlyDate,
  Extra,
  Report,
}

export function nowUTC() {
  dayjs.extend(utc);
  dayjs.extend(timezone);
  return dayjs().utc();
}

export function nowBangkok() {
  return nowUTC().tz('Asia/Bangkok');
}

export function getTimestamp(
  format: TimestampFormat,
  date: dayjs.Dayjs = nowUTC()
): string {
  let dateFormat: string;
  // eslint-disable-next-line default-case
  switch (format) {
    case TimestampFormat.Normal:
      dateFormat = 'YYYYMMDDHHmmss';
      break;
    case TimestampFormat.OnlyDate:
      dateFormat = 'YYYYMMDD';
      break;
    case TimestampFormat.Extra:
      dateFormat = 'YYYYMMDDHHmmssSSS';
      break;
    case TimestampFormat.Report:
      dateFormat = 'DD/M/YYYY HH:mm:ss';
  }

  return date.format(dateFormat);
}

export function generateJulianDateFormatWithExpiredTime(
  date: dayjs.Dayjs
): string {
  const diff = date.diff(`${date.format('YYYY')}-01-01`, 'day') + 1;
  const numberWithPad = String(diff).padStart(3, '0');
  return `${date.format('YY')}${numberWithPad}${date.format('HHmm')}`;
}
