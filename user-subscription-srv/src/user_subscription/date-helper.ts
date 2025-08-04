import dayjs from 'dayjs';

export function renderDate(date: Date) {
  return date ? dayjs(date).format('YYYY-MM-DD') : null;
}
