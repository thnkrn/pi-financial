import { IFuturesOrder } from '@/types/blocktrade/futures/types'

export type IFuturesOrderRequest = {
  ofUser?: number
  page: number
  pageSize: number
  orderBy: string
  orderDir: string
}

export type IFuturesOrderResponse = {
  data: IFuturesOrder[]
  page: number
  pageSize: number
  total: number
  orderBy: string
  orderDir: string
}

export type IUndoFuturesOrderRequest = {
  futuresId: number
}

export type ISubmitFuturesOrderRequest = {
  futuresId: number
  isIC: boolean
  placeTradeReport: boolean
}
