import {backofficeAxiosInstance} from '@/lib/api'
import {IGetDepositTransactionsRequest, IGetDepositTransactionsResponse} from './types'

/**
 * Retrieves deposit transactions.
 *
 * @param {IGetDepositTransactionsRequest} payload - request payload
 * @return {Promise<IGetDepositTransactionsResponse>} promise of deposit transactions response
 */
export const getDepositTransactions = async (
  payload: IGetDepositTransactionsRequest
): Promise<IGetDepositTransactionsResponse> => {
  const queryParams = getRequestQueryParams(payload)
  const instance = await backofficeAxiosInstance()
  const res = await instance.get(`transactions/paginate${queryParams}`)

  return {
    transactions: res?.data?.data,
    page: res?.data?.page,
    pageSize: res?.data?.pageSize,
    total: res?.data?.total,
    orderBy: res?.data?.orderBy,
    orderDir: res?.data?.orderDir,
  }
}

const getRequestQueryParams = (request: IGetDepositTransactionsRequest): string => {
  const { filters, page, pageSize, orderBy, orderDir } = request

  const filtersQueryString = Object.entries(filters)
    .map(([key, value]) => {
      if (value !== undefined && value !== null) {
        return `Filters.${key}=${encodeURIComponent(value)}`
      }
      return null
    })
    .filter(Boolean)
    .join("&")

  const otherQueryString = `Page=${page}&PageSize=${pageSize}&OrderBy=${encodeURIComponent(orderBy)}&OrderDir=${encodeURIComponent(orderDir)}`

  const queryString = [filtersQueryString, otherQueryString].filter(Boolean).join("&")

  return `?${queryString}`
};
