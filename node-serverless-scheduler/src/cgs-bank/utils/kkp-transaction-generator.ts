import { getTimestamp, TimestampFormat } from '@cgs-bank/utils/timestamp';
import dayjs from 'dayjs';

const partnerName = process.env.KKP_PARTNER_NAME;

export function getTransactionId(
  date: dayjs.Dayjs,
  transactionNo: string | undefined = undefined
) {
  if (transactionNo) {
    return `${partnerName}-${transactionNo}`;
  }

  return `${partnerName}${getTimestamp(TimestampFormat.Extra, date)}`;
}
