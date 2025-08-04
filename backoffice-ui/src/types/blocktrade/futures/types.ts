export interface IFuturesOrder {
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
    rollOverTo: number
    status: string
    createdAt: string
    updatedAt: string
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
  }
}
