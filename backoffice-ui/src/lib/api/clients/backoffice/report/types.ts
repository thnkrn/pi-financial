import { PiBackofficeServiceAPIModelsReportResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsReportResponse'

export interface IGetReportsRequest {
  generatedType: string
  dateFrom?: string
  dateTo?: string
  reportType?: string
  page: number
  pageSize: number
  orderBy: string
  orderDir: string
}

export interface IGetReportsResponse {
  reports: PiBackofficeServiceAPIModelsReportResponse[]
  page: number
  pageSize: number
  total: number
  orderBy: string
  orderDir: string
}

export interface IDownloadReportResponse {
  URL: string
}

export interface IGenerateReportRequest {
  type: string
  dateFrom: string
  dateTo: string
}
