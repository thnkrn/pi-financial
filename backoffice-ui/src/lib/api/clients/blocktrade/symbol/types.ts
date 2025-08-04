export type ISymbol = {
  id: number | null
  symbolId: number | null
  symbol: string | null
  seriesId: number | null
  series: string | null
  blocksize: number | null
  mm: number | null
  multiplier: number | null
  expDate: string | null
  createdAt: string | null
  updatedAt: string | null
}

export type IGetSymbolListResponse = ISymbol[]
