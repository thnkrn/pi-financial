export type EquityRowType = {
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

export type ListJPOrderPropsType = {
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
  blockOrders: BlockOrdersPropsType
}

export type BlockOrdersPropsType = {
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
