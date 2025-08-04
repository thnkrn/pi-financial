export interface OrderDetail {
  id: number
  saleId: number
  orderId: number
  openClose: string
  side: string
  symbol: string
  series: string
  numOfContract: number
  numOfShare: number
  numOfShareFilled: number
  orderPrice: number
  execPrice: number
  cusAcc: number
  im: number
  status: string
}
