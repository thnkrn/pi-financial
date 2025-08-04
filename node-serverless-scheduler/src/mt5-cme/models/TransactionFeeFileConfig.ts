import { FieldConfig } from './FieldConfig';

export const transactionFeeFileConfig: FieldConfig[] = [
  { name: 'commission_category', start: 1, length: 10 },
  { name: 'fo_type_code', start: 11, length: 10 },
  { name: 'custodian_account', start: 21, length: 15 },
  { name: 'trade_category', start: 36, length: 1 },
  {
    name: 'trading_fee',
    start: 37,
    length: 11,
    decimal: true,
    leftDigitsCount: 5,
    rightDigitCount: 5,
  },
  {
    name: 'clearing_fee',
    start: 48,
    length: 11,
    decimal: true,
    leftDigitsCount: 5,
    rightDigitCount: 5,
  },
  { name: 'filler', start: 59, length: 42 },
];
