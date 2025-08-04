export class VatCalculator {
  private readonly vat: number;

  constructor(vatPercentRate = Number(process.env.VAT_PERCENT_RATE) ?? 0.0) {
    this.vat = 100.0 + vatPercentRate;
  }

  extractPrice(price: number): { rawPrice: number; vat: number } {
    const rawPrice = (price * 100.0) / this.vat;
    const tax = price - rawPrice;
    return {
      rawPrice: Number(rawPrice.toFixed(2)),
      vat: Number(tax.toFixed(2)),
    };
  }
}
