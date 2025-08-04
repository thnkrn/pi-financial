import { blocktradeAxiosInstance } from '@/lib/api'
import {
  IOrderBookRequest,
  IOrderBookResponse
} from './types'

/**
 * Retrieves order book based on the provided payload.
 *
 * @param {IOrderBookRequest} payload - The payload to apply to the order book retrieval.
 * @return {Promise<IOrderBookResponse>} A promise that resolves to the retrieved order book and related information.
 */
export const getOrderBook = async (payload: IOrderBookRequest): Promise<IOrderBookResponse> => {
  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`stockPrice/getOrderbook?symbol=${payload.symbol}`)

  const response = {
    symbolInfo: {
      celling: res?.data?.symbolInfo?.celling,
      floor: res?.data?.symbolInfo?.floor,
      high: res?.data?.symbolInfo?.high,
      lastOpen: res?.data?.symbolInfo?.lastOpen,
      lastPrice: res?.data?.symbolInfo?.lastPrice,
      low: res?.data?.symbolInfo?.low,
      previousClose: res?.data?.symbolInfo?.previousClose,
      symbol: res?.data?.symbolInfo?.symbol,
      timestamp: res?.data?.symbolInfo?.timestamp,
    },
    orderBook: res?.data?.orderBook,
  }

  return response
}
