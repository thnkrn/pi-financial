import { PiBackofficeServiceAPIModelsTicketDetailResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsTicketDetailResponse'

export interface IGetTicketsRequest {
  responseCodeId?: string
  customerCode?: string
  status?: string
  page: number
  pageSize: number
  orderBy: string
  orderDir: string
}

export interface IGetTicketsResponse {
  tickets: PiBackofficeServiceAPIModelsTicketDetailResponse[]
  page: number
  pageSize: number
  total: number
  orderBy: string
  orderDir: string
}
