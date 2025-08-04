import { PiBackofficeServiceAPIModelsDocumentResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsDocumentResponse'
import { PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse'

export type FilterObjectURLParamType = {
  filterBy?: string
  filterValue?: string
  status?: string
  page?: string
  pageSize?: string
  orderBy?: string
  orderDir?: string
}

export interface IGetUserAccountsResponse {
  accounts: PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse[]
  page: number
  pageSize: number
  total: number
  orderBy: string
  orderDir: string
}

export interface IGetOpenAccountsRequest {
  status?: string
  custCode?: string
  userId?: string
  page?: number
  pageSize?: number
  orderBy?: string
  orderDir?: string
  bpmReceived?: boolean
  [key: string]: any
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
