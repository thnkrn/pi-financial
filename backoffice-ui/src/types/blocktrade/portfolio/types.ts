export interface IPortfolio {
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
  futuresOrders: {
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
  sales: {
    id: number
    keycloakId: string
    role: string
    name: string
    employeeId: number
    brokerId: number
    contact: string | null
    createdAt: string | null
    updatedAt: string | null
  }
  symbol: {
    id: number
    symbol: string
    blocksize: number
    tradeable: boolean
    createdAt: string
    updatedAt: string
  }
  series: {
    id: number
    series: string
    expDate: string
    createdAt: string
    updatedAt: string
  }
  futuresClose: {
    xd: number | null
    mktPrice: number | null
    projFutures: number | null
    unrealizedPnlPerCont: number | null
  }
}
