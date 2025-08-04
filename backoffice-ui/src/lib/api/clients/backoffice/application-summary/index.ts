import { backofficeAxiosInstance } from '@/lib/api'
import { PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse'
import {
  FilterObjectURLParamType,
  IGetOpenAccountsRequest,
  IGetOpenAccountsResponse,
  IGetUserAccountsResponse,
} from './types'

export const getUserAccounts = async (filter: IGetOpenAccountsRequest): Promise<IGetUserAccountsResponse> => {
  Object.keys(filter).forEach(key => {
    if (
      filter[key] === null ||
      filter[key] === '' ||
      filter[key] === undefined ||
      (key === 'bpmReceived' && filter['bpmReceived'] === true)
    ) {
      delete filter[key]
    }
  })

  const objString = '?' + new URLSearchParams(filter as FilterObjectURLParamType).toString()

  const instance = await backofficeAxiosInstance()
  const res = await instance.get(`onboard/openaccount/list${objString}`)

  return {
    accounts: res?.data?.data,
    page: res?.data?.page,
    pageSize: res?.data?.pageSize,
    total: res?.data?.total,
    orderBy: res?.data?.orderBy,
    orderDir: res?.data?.orderDir,
  }
}

export const getOpenAccounts = async (filter: IGetOpenAccountsRequest): Promise<IGetOpenAccountsResponse> => {
  const userAccounts = await getUserAccounts(filter)

  const accounts = userAccounts?.accounts.map((account: PiBackofficeServiceAPIModelsOnboardingOpenAccountResponse) => ({
    id: account?.id,
    citizenId: account?.identification?.citizenId,
    title: account?.identification?.title,
    firstNameTh: account?.identification?.firstNameTh,
    lastNameTh: account?.identification?.lastNameTh,
    firstNameEn: account?.identification?.firstNameEn,
    lastNameEn: account?.identification?.lastNameEn,
    dateOfBirth: account?.identification?.dateOfBirth,
    idCardExpiryDate: account?.identification?.idCardExpiryDate,
    laserCode: account?.identification?.laserCode,
    nationality: account?.identification?.nationality,
    userId: account?.identification?.userId,
    email: account?.identification?.email,
    phone: account?.identification?.phone,
    status: account?.status,
    createdDate: account?.createdDate,
    updatedDate: account?.updatedDate,
    bpmReceived: account?.bpmReceived,
    custCode: account?.custCode,
    referId: account?.referId,
    transId: account?.transId,
    documents: account?.documents,
  }))

  return {
    accounts,
    page: userAccounts?.page,
    pageSize: userAccounts?.pageSize,
    total: userAccounts?.total,
    orderBy: userAccounts?.orderBy,
    orderDir: userAccounts?.orderDir,
  }
}
