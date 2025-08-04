export type MonitorDataType = {
  id: number
  orderId: string
  blockId: number
  pos: string
  side: string
  symbol: string
  qty: number
  qtyFilled: number
  executedPrice: number
  mainId: number | null
}
