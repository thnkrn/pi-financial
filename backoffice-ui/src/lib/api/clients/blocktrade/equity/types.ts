export type IEquityOrderRequest = {
  ofUser?: number
  page: number
  pageSize: number
  orderBy: string
  orderDir: string
}

export type IEquityOrderResponse = {
  data: IEquityOrder[]
  page: number
  pageSize: number
  total: number
  orderBy: string
  orderDir: string
}

export interface IEquityOrder {
  id: number
  orderId: string
  numOfShare: number
  numOfShareFilled: number
  orderPrice: number
  orderType: string
  executedPrice: number
  im: number
  status: string
  blockId: number
  completed: string
  createdBy: number
  createdAt: string
  updatedAt: string
  submitterId: number
  mainId: number | null
  canUndo: boolean | null
  blockOrders: {
    symbolId: number
    seriesId: number
    openClose: string
    clientSide: string
    numOfContract: number | null
    customerAccount: string
    saleId: number
    sales: {
      id: number
      keycloakId: string
      role: string
      name: string
      employeeId: number
      brokerId: number
      contact: string
      createdAt: string
      updatedAt: string
    }
    symbol: {
      id: number
      symbol: string
      blocksize: number
    } | null
    series: {
      id: number
      series: string
      expDate: string
    } | null
  }
}

export type ISubmitNewOrderRequest = {
  saleId: number
  openClose: string
  side: string
  symbol: string
  series: string
  numOfContract: number
  orderPrice: number
  orderType: string
  customerAccount: string
}

export type ISubmitAmendOrderRequest = {
  series: string
  orderPrice: number
}
