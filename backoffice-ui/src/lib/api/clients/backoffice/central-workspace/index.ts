import { backofficeAxiosInstance } from '@/lib/api'
import { IGetTicketsRequest, IGetTicketsResponse } from './types'

/**
 * Retrieves tickets based on the provided filter.
 *
 * @param {IGetTicketsRequest} filter - The filter to apply to the tickets retrieval.
 * @return {Promise<IGetTicketsResponse>} A promise that resolves to the retrieved tickets and related information.
 */
export const getTickets = async (filter: IGetTicketsRequest): Promise<IGetTicketsResponse> => {
  // NOTE: need to define as any type since URLSearchParams only accept type string but we prefer to use our own type for filter
  const objString = '?' + new URLSearchParams(filter as any).toString()

  const instance = await backofficeAxiosInstance()
  const res = await instance.get(`tickets${objString}`)

  return {
    tickets: res?.data?.data,
    page: res?.data?.page,
    pageSize: res?.data?.pageSize,
    total: res?.data?.total,
    orderBy: res?.data?.orderBy,
    orderDir: res?.data?.orderDir,
  }
}
