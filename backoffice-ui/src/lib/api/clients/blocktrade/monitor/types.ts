export type IFlagICRequest = {
  blockId: number
  userId: number
}

export type IFetchUserResponse = {
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
  lineToken: string
}

export type IFetchAllUserResponse = IFetchUserResponse[]

export type ISubmitJPRequest = {
  jp: IJPDataRequest[]
}

export type IJPDataRequest = {
  orderId: string
  oc: string
  side: string
  symbol: string
  entryPrice: number
  orderType: string
  qty: number
  filled: number
  avgPrice: number
  status: string
  added: boolean
}

export type IListJPOrderResponse = {
  id: number
  orderId: string
  mainId: number | null
  numOfShare: number
  numOfShareFilled: number
  orderPrice: number
  orderType: string
  executedPrice: number
  im: number
  status: string
  blockId: number
  submitterId: number
  channel: string
  createdAt: string
  updatedAt: string
  blockOrders: IBlockOrders
}

export type IBlockOrders = {
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
}

export type IOrdersFetchingResponse = {
  status: string
  message: string
}

export type IAdminCreateFuturesRequest = {
  targetBlockId: number
  series: string
  saleId: number
  customerAccount: string
  placeICTradeReport: boolean
  equities: IEquityID[]
}

export type IEquityID = {
  equityOrderId: number
}

export type IGetExpectedFuturesPriceRequest = {
  openClose: string,
  side: string,
  symbol: string,
  series: string,
  customerAccount: string,
  equities: IEquityID[]
}

export type IGetExpectedFuturesPriceResponse = {
  expectedFuturesPrice: number,
  openFutures: IOpenFutures[]
}

export type IOpenFutures = {
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

export type IGetFuturesPropRequest = {
  symbol: string,
  series: string
}

export type IGetFuturesPropResponse = {
  id: number
  symbolId: number
  symbol: string
  seriesId: number
  series: string
  blocksize: number
  mm: number
  multiplier: number
  expDate: string
  createdAt: string
  updatedAt: string
}

export type ISplitOrderRequest = {
  equityOrderId: number,
  splitOrders: {
    id: number,
    qty: number
  }[]
}

export type ISplitUndoRequest = {
  mainId: number
}
