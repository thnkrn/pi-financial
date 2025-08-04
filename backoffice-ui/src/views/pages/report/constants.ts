import dayjs from 'dayjs'

export const DATE_FORMAT = 'YYYY-MM-DD'
export const TODAY = new Date()
export const LAST_7_DAYS = dayjs().subtract(7, 'days')
