import { PiBackofficeServiceAPIModelsDocumentResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsDocumentResponse'
import { PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse'

export interface IGetUserAccountsResponse {
  accounts: PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse[]
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
  documents?: PiBackofficeServiceAPIModelsDocumentResponse[] | null
}

export interface IGetOnboardingAccountsResponse {
  accounts: IAccounts[]
}
