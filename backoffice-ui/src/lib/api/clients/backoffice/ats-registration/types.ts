import { PiBackofficeServiceAPIModelsDocumentResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsDocumentResponse'
import { PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse'

export type FilterObjectURLParamType = {
  atsUploadType?: string
  requestDate?: string
  page?: string
  pageSize?: string
}

export interface IGetUserAccountsResponse {
  accounts: PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse[]
  page: number
  pageSize: number
  total: number
  orderBy: string
  orderDir: string
}

export interface IGetAtsReportsRequest {
  atsUploadType: string | null
  requestDate: string | null
  page?: number
  pageSize?: number
  [key: string]: any
}

export interface IGetAtsReportsData {
  id: string | null,
  atsUploadType: string | null,
  requestDate: string | null,
  status: string | null,
  reportName: string | null,
  userName: string | null,
}

export interface IGetAtsReportsResponse {
  data: IGetAtsReportsData[]
  page?: number
  pageSize?: number
  totalPages?: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export interface IAccounts {
  id?: string | null
  citizenId?: string | null
  title?: string | null
  firstNameTh?: string | null
  lastNameTh?: string | null
  firstNameEn?: string | null
  lastNameEn?: string | null
  dateOfBirth?: string | null
  idCardExpiryDate?: string | null
  laserCode?: string | null
  nationality?: string | null
  userId?: string | null
  email?: string | null
  phone?: string | null
  status?: string | null
  createdDate?: string | null
  updatedDate?: string | null
  bpmReceived?: boolean
  custCode?: string | null
  referId?: string | null
  transId?: string | null
  documents?: PiBackofficeServiceAPIModelsDocumentResponse[] | null
}

export interface IGetOpenAccountsResponse {
  accounts: IAccounts[]
  status?: string
  page: number
  pageSize: number
  total: number
  orderBy: string
  orderDir: string
}

export interface IGetAtsReportDownloadResponse {
  blobData: Blob
  fileName: string
}

export interface IUploadAtsRequest {
  uploadFile: File
  userName: string
  uploadType: string
}

export interface IUploadAtsResponse {
  rowCount: number | null
  error: string | null
}

export interface IUploadAtsErrorResponse {
  type: string,
  title: string,
  status: number,
  detail: string,
  traceId: string
}
