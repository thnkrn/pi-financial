export type PositionRowType = {
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
  futuresOrders: {
    id: number
    blockId: number | null
    futuresPrice: number | null
    equityPrice: number | null
    xd: number[]
    dealerSubmitted: boolean | null
    saleSubmitted: boolean | null
    dealerDateTime: string | null
    saleDateTime: string | null
    createdAt: string | null
    updatedAt: string | null
  }
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
  futuresClose: {
    xd: number | null
    mktPrice: number | null
    projFutures: number | null
    unrealizedPnlPerCont: number | null
  }
}
