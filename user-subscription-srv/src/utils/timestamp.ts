import dayjs from 'dayjs';

export enum TimestampFormat {
  Normal,
  OnlyDate,
  Extra,
  Report,
}

export function nowUTC() {
  return dayjs().utc();
}

export function nowBangkok() {
  return nowUTC().tz('Asia/Bangkok');
}
export function getTimestamp(
  format: TimestampFormat,
  date: dayjs.Dayjs = nowUTC(),
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
