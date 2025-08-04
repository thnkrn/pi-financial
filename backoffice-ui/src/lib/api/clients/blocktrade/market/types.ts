export type IOrderBookRequest = {
  symbol: string
}

export type IOrderBook = {
  id: number | null
  bidPrice: number | null
  bidVolume: number | null
  askPrice: number | null
  askVolume: number | null
}

export type IOrderBookInfo = {
  symbol: string | null
  lastPrice: number | null
  high: number | null
  low: number | null
  previousClose: number | null
  lastOpen: number | null
  floor: number | null
  celling: number | null
  timestamp: number | null
}

export type IOrderBookResponse = {
  symbolInfo: IOrderBookInfo
  orderBook: IOrderBook[]
}
