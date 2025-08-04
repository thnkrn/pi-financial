import { IPortfolio } from '@/types/blocktrade/portfolio/types'

type IPortfolioFilterProps = {
  symbol?: string
}

export type IPortfolioRequest = {
  filters: IPortfolioFilterProps
  ofUser?: number
  pnl?: boolean
  page: number
  pageSize: number
  orderBy: string
  orderDir: string
}

export type IPortfolioResponse = {
  data: IPortfolio[]
  page: number
  pageSize: number
  total: number
  orderBy: string
  orderDir: string
}

export type IRolloverExpectedFuturesPriceRequest = {
  openClose: string,
  side: string,
  symbol: string,
  series: string,
  rollToSeries: string,
  rollPrice: number,
  customerAccount: string,
  blocks: IBlocks[]
}

type IBlocks = {
  blockOrderId: number
}

export type IRolloverExpectedFuturesPriceResponse = {
  expectedFuturesPrice: number,
  multiplierDiff: boolean,
  openFutures: IOpenFutures[]
}

type IOpenFutures = {
  equityPrice: number,
  qty: number,
  totalQty: number,
  createdAt: string,
  dividendRate: number,
  marketFee: number,
  minDay: number,
  interestRate: number,
  vatRate: number,
  commissionRate: number,
  xd: number
}

export type IRolloverListRequest = {
  blockId: number
}

export type IRolloverListResponse = {
  id: number
  symbolId: number
  seriesId: number
  openClose: string
  clientSide: string
  numOfContract: number
  customerAccount: string
  saleId: number
  availContract: number
  submitterId: number
  dividendRate: number
  marketFee: number
  minDay: number
  interestRate: number
  vatRate: number
  commissionRate: number
  rollOverTo: number | null
  status: string
  createdAt: string
  updatedAt: string
  futuresOrders: IFuturesOrders
  sales: ISales
  symbol: ISymbol
  series: ISeries
}

type IFuturesOrders = {
  id: number
  blockId: number
  futuresPrice: number
  equityPrice: number
  xd: number[]
  dealerSubmitted: boolean
  saleSubmitted: boolean
  dealerDateTime: string
  saleDateTime: string
  createdAt: string
  updatedAt: string
}

type ISales = {
  id: number
  keycloakId: string
  role: string
  name: string
  employeeId: number
  brokerId: number
  contact: string
  verified: boolean
  createdAt: string
  updatedAt: string
}

type ISymbol = {
  id: number
  symbol: string
  blocksize: number
  tradeable: boolean
  createdAt: string
  updatedAt: string
}

type ISeries = {
  id: number
  series: string
  expDate: string
  createdAt: string
  updatedAt: string
}

export type ISubmitRolloverRequest = {
  rollToSeries: string,
  rollPrice: number,
  rollQty: number,
  customerAccount: string,
  blocks: IBlocks[]
}
