import { FieldConfig } from './FieldConfig';

export const commissionGroupFileConfig: FieldConfig[] = [
  { name: 'commission_group_code', start: 1, length: 10 },
  { name: 'commission_category', start: 11, length: 10 },
  { name: 'order_channel_code', start: 21, length: 1 },
  { name: 'formula_code', start: 22, length: 1 },
  { name: 'tier_code', start: 23, length: 10 },
  {
    name: 'commission_amount',
    start: 33,
    length: 8,
    decimal: true,
    leftDigitsCount: 5,
    rightDigitCount: 2,
  },
  {
    name: 'commission_rate',
    start: 41,
    length: 8,
    decimal: true,
    leftDigitsCount: 5,
    rightDigitCount: 2,
  },
  { name: 'trade_category', start: 49, length: 1 },
  { name: 'side_buy_sell', start: 50, length: 1 },
  { name: 'plan_code', start: 51, length: 10 },
  { name: 'filler', start: 61, length: 30 },
];
