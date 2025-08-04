export type IActivityLogRequest = {
  ofUser?: number
  page: number
  pageSize: number
  orderBy: string
  orderDir: string
}

export type IActivityLogResponse = {
  data: IActivityLog[]
  page: number
  pageSize: number
  total: number
  orderBy: string
  orderDir: string
}

export interface IActivityLog {
  id: number
  action: string
  blockId: number
  orderId: number
  detail: string
  symbolId: number
  seriesId: number
  saleId: number
  submitterId: number
  createdAt: string
}
