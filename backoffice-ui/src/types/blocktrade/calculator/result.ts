export interface PositionDetail {
  futuresPriceOpen?: number
  futuresPriceClose?: number
  im?: number
  timeToMaturity?: number
  commissionFeeOpen?: number
  cashUsage?: number
  interestPerShare?: number
  holdingPeriod?: number
  totalInterestAmount?: number
  commissionFeeClose?: number
}

export interface DataEntry {
  id: number
  closeStockPrice: number
  openSsfPrice: number
  closeSsfPrice: number
  pnl: number
}

export interface ProjectionData {
  data: { [key: string]: DataEntry }
}

export interface CalculationResult {
  lastDayTrading: string | null
  openDate: Date
  holdingPeriod: number
  symbol: string | null
  priceInterval: number
  totalCommissionFee: number
  initialPnL: number
  PnLAfterInt: number
  netPnL: number
  openPosition: PositionDetail
  closePosition: PositionDetail
  projUpper: ProjectionData
  projMiddle: ProjectionData
  projLower: ProjectionData
}
