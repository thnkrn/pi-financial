import dayjs, { Dayjs } from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';

dayjs.extend(utc);
dayjs.extend(timezone);

export function nowUTC() {
  return dayjs().utc();
}

export function nowBangkok() {
  return nowUTC().tz('Asia/Bangkok');
}

export function isWeekend(day: Dayjs) {
  return day.day() === 0 || day.day() === 6;
}

/**
 * Get hours, min, sec, ms  from time string.
 *
 * @param time - time in string format HH:mm
 * @returns Time in number arrays [hours , minutes, seconds, ms]
 */
export function getTimeFromString(time: string): number[] {
  const parts = time.split(':');
  return [parseInt(parts[0]), parseInt(parts[1]), 0, 0];
}
