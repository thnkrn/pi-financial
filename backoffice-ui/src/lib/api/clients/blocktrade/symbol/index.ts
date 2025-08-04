import { blocktradeAxiosInstance } from '@/lib/api'
import {
  IGetSymbolListResponse,
} from './types'

/**
 * Retrieves the list of symbol information .
 * @return {Promise<IGetCustomerListResponse>} A promise that resolves to the retrieved list of symbol and related information.
 */
export const getSymbolList = async (): Promise<IGetSymbolListResponse> => {
  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`futuresProperties/getList`)

  return res.data
}
