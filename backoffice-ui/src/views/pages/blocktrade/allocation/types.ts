export type FuturesRowType = {
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
  blockOrders: {
    id: number | null
    symbolId: number | null
    seriesId: number | null
    openClose: string | null
    clientSide: string | null
    numOfContract: number | null
    customerAccount: string | null
    saleId: number | null
    availContract: number | null
    submitterId: number | null
    dividendRate: number | null
    marketFee: number | null
    minDay: number | null
    interestRate: number | null
    vatRate: number | null
    commissionRate: number | null
    rollOverTo: number | null
    status: string | null
    createdAt: string | null
    updatedAt: string | null
    sales: {
      id: number | null
      keycloakId: string | null
      role: string | null
      name: string | null
      employeeId: number | null
      brokerId: number | null
      contact: string | null
      createdAt: string | null
      updatedAt: string | null
    }
    symbol: {
      id: number | null
      symbol: string | null
      blocksize: number | null
      tradeable: boolean | null
      createdAt: string | null
      updatedAt: string | null
    }
    series: {
      id: number | null
      series: string | null
      expDate: string | null
      createdAt: string | null
      updatedAt: string | null
    }
    futuresSettrade: FuturesSettradeType[]
  }
}

type FuturesSettradeType = {
  id: number
  blockId: number
  settradeId: number | null
  status: string
  traderIcId: number | null
}
