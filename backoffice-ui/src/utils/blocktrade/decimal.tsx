export const DecimalNumber = (value: number, decimal: number) => {
  return new Intl.NumberFormat('en-US', {
    minimumFractionDigits: decimal,
    maximumFractionDigits: decimal,
  }).format(value)
}
