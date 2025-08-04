import { ssoAdminAxiosInstance } from '@/lib/api'
import type { NextApiRequest, NextApiResponse } from 'next/dist/shared/lib/utils'
import { FilterObjectURLParamType, IGetAccountInfoRequest } from './types'

// Utility function สำหรับลบ `null` หรือ `undefined` ออกจาก object
const removeNullOrUndefined = (obj: Record<string, any>): Record<string, any> => {
  return Object.entries(obj)
    .filter(([value]) => value !== null)
    .reduce(
      (acc, [key, value]) => {
        acc[key] = value

        return acc
      },
      {} as Record<string, any>
    )
}
export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  const url = new URL(req.url ?? '', `http://${req.headers.host}`)
  const filter: IGetAccountInfoRequest = Object.fromEntries(url.searchParams.entries()) as IGetAccountInfoRequest

  // ลบ key ที่มีค่าเป็น null หรือ undefined
  const sanitizedFilter = removeNullOrUndefined(filter)

  // สร้าง query string
  const queryString = '?' + new URLSearchParams(sanitizedFilter as FilterObjectURLParamType).toString()

  try {
    const instance = await ssoAdminAxiosInstance()
    const response = await instance.get(`/internal/accounts/accountInfoByUsernameOrPage${queryString}`)
    const apiResponse = response.data.data

    //datadogLogs.logger.info('API response:', apiResponse)

    const defaultColValue = '-'

    const mappedData = apiResponse.data?.map((v: any) => ({
      id: v.id ?? defaultColValue,
      username: v.username ?? defaultColValue,
      isSyncPassword: v.isSyncPassword ?? false,
      isSyncPin: v.isSyncPin ?? false,
      loginPwdFailCount: v.loginPwdFailCount ?? 0,
      loginPinFailCount: v.loginPinFailCount ?? 0,
      isLock: v.isLock ?? false,
      updatedAt: v.updatedAt ?? defaultColValue,
      createdAt: v.createdAt ?? defaultColValue,
      userId: v.userId ?? defaultColValue,
      email: v.email ?? defaultColValue,
      mobile: v.mobile ?? defaultColValue,
      cardId: v.cardId ?? defaultColValue,
    }))

    return res.status(200).json({
      data: mappedData ?? [],
      currentPage: apiResponse.currentPage,
      pageSize: apiResponse.pageSize,
      hasNextPage: apiResponse.hasNextPage ?? false,
      hasPreviousPage: apiResponse.hasPreviousPage ?? false,
      totalPages: apiResponse.totalPages,
    })
  } catch (error: any) {
    //datadogLogs.logger.error('Error fetching account infos:', error)

    return res.status(200).json({
      data: [],
      currentPage: 1,
      pageSize: 0,
      hasNextPage: false,
      hasPreviousPage: false,
      totalPages: 0,
    })
  }
}
