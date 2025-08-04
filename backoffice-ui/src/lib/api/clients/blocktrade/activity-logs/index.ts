import { blocktradeAxiosInstance } from '@/lib/api'
import {
  IActivityLogRequest,
  IActivityLogResponse,
} from './types'

/**
 * Retrieves activity log based on the provided filter.
 *
 * @param {IActivityLogRequest} filter - The filter to apply to the activity log retrieval.
 * @return {Promise<IActivityLogResponse>} A promise that resolves to the retrieved activity log and related information.
 */
export const getActivityLog = async (filter: IActivityLogRequest): Promise<IActivityLogResponse> => {
  const objString = '?' + new URLSearchParams(filter as any).toString()

  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`activityLogs${objString}`)

  return {
    data: res?.data?.data,
    page: res?.data?.page,
    pageSize: res?.data?.pageSize,
    total: res?.data?.total,
    orderBy: res?.data?.orderBy,
    orderDir: res?.data?.orderDir
  }
}
