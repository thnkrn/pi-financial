import { ssoAdminAxiosInstance } from '@/lib/api'
import { datadogLogs } from '@datadog/browser-logs'
import {
  FilterObjectURLParamType,
  IAccountInfo,
  IGetAccountInfoRequest,
  IGetAccountInfoResponse,
  IGetAccountInfoResponseWrapper
} from './types'

// ฟังก์ชันสำหรับเรียกข้อมูลบัญชีทั้งหมด
export const getAccountInfo = async (): Promise<IGetAccountInfoResponseWrapper> => {
  try {
    const instance = await ssoAdminAxiosInstance()
    const res = await instance.get(`/internal/accounts/accountInfoAll`)
    return res.data
  } catch (error) {
    datadogLogs.logger.error('Error api path `/internal/accounts/accountInfoAll` fetching all account info: ' + error)
    throw error
  }
}

// ฟังก์ชันสำหรับเรียกข้อมูลบัญชีโดยใช้ username
export const getAccountInfoByUsername = async (
  username: string
): Promise<IGetAccountInfoResponseWrapper> => {
  try {
    const instance = await ssoAdminAxiosInstance()
    const res = await instance.get(`/internal/accounts/accountInfoAll/${username}`)
    return res.data
  } catch (error) {
    datadogLogs.logger.error(`Error api path '/internal/accounts/accountInfoAll/{username}' fetching account info for username "${username}": ` + error)
    throw error
  }
}

// ฟังก์ชันสำหรับเรียกข้อมูลบัญชีแบบแบ่งหน้า
export const getAccountInfos = async (
  filter: IGetAccountInfoRequest
): Promise<IGetAccountInfoResponse> => {
  // ลบ key ที่มีค่าเป็น null หรือ undefined
  Object.keys(filter).forEach((key) => {
    if (!filter[key]) {
      delete filter[key]
    }
  })

  const objString =
    '?' + new URLSearchParams(filter as FilterObjectURLParamType).toString()

  try {
    const instance = await ssoAdminAxiosInstance()

    const apiResponse = (await instance.get(
      `/internal/accounts/accountInfoByUsernameOrPage${objString}`
    )).data.data as IGetAccountInfoResponse



    const defaultColValue = '-'
    const mapResponse: IAccountInfo[] =
      apiResponse.data?.map((v) => ({
        id: v.id ?? defaultColValue,
        username: v.username ?? defaultColValue,
        isSyncPassword: v.isSyncPassword ?? false,
        isSyncPin: v.isSyncPin ?? false,
        loginFailCount: v.loginFailCount ?? 0,
        isLock: v.isLock ?? false,
        updatedAt: v.updatedAt ?? defaultColValue,
        createdAt: v.createdAt ?? defaultColValue,
        userId: v.userId ?? null,
        email: v.email ?? null,
        mobile: v.mobile ?? null,
      })) || []





    return {
      data: mapResponse || [],
      currentPage: apiResponse.currentPage,
      pageSize: apiResponse.pageSize,
      hasNextPage: apiResponse.hasNextPage ?? false,
      hasPreviousPage: apiResponse.hasPreviousPage ?? false,
      totalPages: apiResponse.totalPages,
    }
  } catch (error) {
    datadogLogs.logger.error('Error api path `/internal/accounts/accountInfoByUsernameOrPage` fetching account infos:' + error)
    throw error
  }
}
