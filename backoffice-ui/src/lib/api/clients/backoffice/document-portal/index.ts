import { backofficeAxiosInstance } from '@/lib/api'
import { PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse'
import { IGetOnboardingAccountsResponse, IGetUserAccountsResponse } from './types'

export const getUserAccountsByCustcode = async (custcode: string): Promise<IGetUserAccountsResponse> => {
  const instance = await backofficeAxiosInstance()
  const res = await instance.get(`document_portal/open_accounts/${custcode}`)

  return {
    accounts: res?.data,
  }
}

export const getOnboardingAccounts = async (custcode: string): Promise<IGetOnboardingAccountsResponse> => {
  const res = await getUserAccountsByCustcode(custcode)

  const accounts = res?.accounts?.map((account: PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse) => ({
    id: account?.id,
    nationality: account?.identification?.nationality,
    userId: account?.identification?.userId,
    citizenId: account?.identification?.citizenId,
    title: account?.identification?.title,
    firstNameTh: account?.identification?.firstNameTh,
    lastNameTh: account?.identification?.lastNameTh,
    firstNameEn: account?.identification?.firstNameEn,
    lastNameEn: account?.identification?.lastNameEn,
    dateOfBirth: account?.identification?.dateOfBirth,
    idCardExpiryDate: account?.identification?.idCardExpiryDate,
    laserCode: account?.identification?.laserCode,
    email: account?.identification?.email,
    phone: account?.identification?.phone,
    createdDate: account?.createdDate,
    updatedDate: account?.updatedDate,
    custCode: account?.custCode,
    documents: account?.documents,
    status: account?.status,
    bpmReceived: account?.bpmReceived,
  }))

  return {
    accounts,
  }
}
