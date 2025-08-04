import { FieldConfig } from '../models/FieldConfig';

export class FixedLengthGenerator {
  constructor(private config: FieldConfig[]) {}

  private formatField(value: string | undefined, field: FieldConfig): string {
    if (field.empty === true || !value) {
      return ' '.repeat(field.length);
    }

    const str = value.trim();

    // Decimal field formatting
    if (field.decimal) {
      const [intPartRaw, fracPartRaw] = str.split('.');
      const intPart = intPartRaw?.padStart(field.leftDigitsCount ?? 0, '0');
      const fracPart = (fracPartRaw ?? '').padEnd(
        field.rightDigitCount ?? 0,
        '0'
      );
      const formatted = `${intPart}.${fracPart}`;

      const expectedLength =
        (field.leftDigitsCount ?? 0) + 1 + (field.rightDigitCount ?? 0);
      if (formatted.length !== expectedLength) {
        throw new Error(
          `Formatted decimal value "${formatted}" does not match expected length ${expectedLength}`
        );
      }

      return formatted;
    }

    // Default string field formatting
    if (str.length >= field.length) {
      return str.slice(0, field.length);
    }
    return str.padEnd(field.length, ' ');
  }

  public generate(records: Record<string, string>[]): string {
    return records
      .map((record) =>
        this.config
          .map((field) => this.formatField(record[field.name], field))
          .join('')
      )
      .join('\n');
  }
}
