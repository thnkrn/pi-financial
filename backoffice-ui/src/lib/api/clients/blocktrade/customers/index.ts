import { blocktradeAxiosInstance } from '@/lib/api'
import {
  IGetCustomerListResponse,
} from './types'

/**
 * Retrieves the list of customer information based on the provided id of ic.
 * @param {number} icid - The id of ic to apply to the list of customer information retrieval.
 * @return {Promise<IGetCustomerListResponse>} A promise that resolves to the retrieved list of customers and related information.
 */
export const getCustomerList = async (icid: number): Promise<IGetCustomerListResponse> => {
  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`customers/getList?icId=${icid}`)

  return res.data
}
