export interface IOrderState {
  isAmend: boolean | null
  id: number | null
  ic: number
  oc: string
  side: string
  symbol: string | null
  futuresProperty: {
    key: string
    value: string
    symbol: string
    series: string
    mul: number
    exp: string
    im: number
    blocksize: number
  }
  customer: string
  contractAmount: number
  equityPrice: number
  orderType: string
  futuresPrice: number | null
  orderStatus: string | null
  isLoading: boolean
  errorMessage: string | null
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
  canUndo: boolean
  blockOrders: {
    symbolId: number
    seriesId: number
    openClose: string
    clientSide: string
    numOfContract: number
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
    }
    series: {
      id: number
      series: string
      expDate: string
    }
  }
}
