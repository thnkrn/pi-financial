import dayjs from 'dayjs'
import utc from 'dayjs/plugin/utc'
import Decimal from 'decimal.js'
import * as numerable from 'numerable'

dayjs.extend(utc)

const DEFAULT_FORMAT_DATE_OPTS = { format: 'DD/MM/YYYY', ifNull: '', isUTC: false }
export const formatDate = (date: Date | null, opts?: Partial<typeof DEFAULT_FORMAT_DATE_OPTS>): string => {
  if (date) {
    if (opts?.isUTC) return dayjs.utc(date).format(opts?.format ?? DEFAULT_FORMAT_DATE_OPTS.format)

    return dayjs(date).format(opts?.format ?? DEFAULT_FORMAT_DATE_OPTS.format)
  } else {
    return opts?.ifNull ?? DEFAULT_FORMAT_DATE_OPTS.ifNull
  }
}

export const formatCurrency = (value: Decimal | string | number) => {
  const num = Decimal.isDecimal(value) ? (value as Decimal).toFixed() : (value as string | number)

  return numerable.format(num, '0,0.00')
}

export const parseAccessToken = (accessToken: string) => {
  return JSON.parse(Buffer.from(accessToken.split('.')[1], 'base64').toString())
}

export const toLowerCaseFirstLetter = (str: string): string => {
  return str.charAt(0).toLowerCase() + str.slice(1)
}

export const capitalizeFirstLetter = (str: string): string => {
  return str.charAt(0).toUpperCase() + str.slice(1)
}

export const showErrors = (field: string, valueLen: number, max: number) => {
  return valueLen > 0 && valueLen > max ? `'${field}' must be less than or equal to ${max} characters` : ''
}
