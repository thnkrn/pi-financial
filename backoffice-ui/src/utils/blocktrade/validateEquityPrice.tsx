export const validateAndRoundPrice = (price: string) => {
  const priceNum = parseFloat(price)
  if (isNaN(priceNum)) return ''

  const ranges = [
    { min: 0, max: 2, step: 0.01 },
    { min: 2, max: 5, step: 0.02 },
    { min: 5, max: 10, step: 0.05 },
    { min: 10, max: 25, step: 0.1 },
    { min: 25, max: 100, step: 0.25 },
    { min: 100, max: 200, step: 0.5 },
    { min: 200, max: 400, step: 1 },
    { min: 400, max: Infinity, step: 2 },
  ]

  for (const range of ranges) {
    if (priceNum >= range.min && priceNum < range.max) {
      const roundedPrice = Math.round(priceNum / range.step) * range.step

      return roundedPrice.toFixed(2)
    }
  }

  return price
}
