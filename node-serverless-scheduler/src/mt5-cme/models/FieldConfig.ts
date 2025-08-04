export interface FieldConfig {
  name: string;
  start: number;
  length: number;
  decimal?: boolean;
  empty?: boolean;
  leftDigitsCount?: number;
  rightDigitCount?: number;
}
