import { PiBackofficeServiceApplicationModelsSblSblInstrument } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceApplicationModelsSblSblInstrument'
import { PiBackofficeServiceApplicationModelsSblSblOrder } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceApplicationModelsSblSblOrder'

export const SBL_ORDER_STATUS = {
  pending: 'pending',
  approved: 'approved',
  rejected: 'rejected',
}

export interface IGetOrdersRequest {
  page: number
  pageSize: number
  orderBy: string
  orderDir: string
  statues: string[] | string
  tradingAccountNo?: string
}

export interface IGetOrdersResponse {
  orders: PiBackofficeServiceApplicationModelsSblSblOrder[]
  page: number
  pageSize: number
  total: number
  orderBy: string
  orderDir: string
}

export interface IGetInstrumentsRequest {
  page: number
  pageSize: number
  orderBy: string
  orderDir: string
  symbol?: string
}

export interface IGetInstrumentsResponse {
  instruments: PiBackofficeServiceApplicationModelsSblSblInstrument[]
  page: number
  pageSize: number
  total: number
  orderBy: string
  orderDir: string
}

export interface IUpdateOrderActionRequest {
  status: string
  rejectedReason: string
}
