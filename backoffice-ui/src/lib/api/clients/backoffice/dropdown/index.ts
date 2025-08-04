import { backofficeAxiosInstance } from '@/lib/api'
import { IGetDropdownRequest, IGetDropdownResponse } from './types'

/**
 * Retrieves dropdown data based on the provided filter.
 *
 * @param {IGetDropdownRequest} filter - the filter object containing the URL for dropdown data retrieval
 * @return {Promise<IGetDropdownResponse>} A promise that resolves with the dropdown data and the field name
 */
export const getDropdown = async (filter: IGetDropdownRequest): Promise<IGetDropdownResponse> => {
  const instance = await backofficeAxiosInstance()
  const res = await instance.get(filter.url)

  return {
    field: filter.field,
    data: res?.data?.data,
  }
}
