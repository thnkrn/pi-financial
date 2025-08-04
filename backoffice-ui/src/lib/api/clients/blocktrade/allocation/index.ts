import { blocktradeAxiosInstance } from '@/lib/api'
import {
  IFuturesOrderRequest,
  IFuturesOrderResponse,
  ISubmitFuturesOrderRequest,
  IUndoFuturesOrderRequest
} from './types'
import { AxiosResponse } from 'axios'

/**
 * Retrieves futures orders based on the provided filter.
 *
 * @param {IFuturesOrderRequest} filter - The filter to apply to the futures orders retrieval.
 * @return {Promise<IFuturesOrderResponse>} A promise that resolves to the retrieved futures orders and related information.
 */
export const getFuturesOrders = async (filter: IFuturesOrderRequest): Promise<IFuturesOrderResponse> => {
  // NOTE: need to define as any type since URLSearchParams only accept type string but we prefer to use our own type for filter
  const objString = '?' + new URLSearchParams(filter as any).toString()

  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`futuresOrders${objString}`)

  return {
    data: res?.data?.data,
    page: res?.data?.page,
    pageSize: res?.data?.pageSize,
    total: res?.data?.total,
    orderBy: res?.data?.orderBy,
    orderDir: res?.data?.orderDir
  }
}

/**
 * Undo a creation of a futures order using the provided payload.
 *
 * @param {IUndoFuturesOrderRequest} payload - the request payload for undo of a futures order
 * @return {Promise<AxiosResponse<any, any>>} a promise that resolves to the response from the undo of a futures order endpoint
 */
export const undoFuturesOrders = async (payload: IUndoFuturesOrderRequest): Promise<AxiosResponse<any, any>> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.post('futuresOrders/undoSubmit', payload)
}

/**
 * Submitting of a futures order from dealers or sales using the provided payload.
 *
 * @param {ISubmitFuturesOrderRequest} payload - the request payload for submitting of a futures order
 * @return {Promise<AxiosResponse<any, any>>} a promise that resolves to the response from the submitting of a futures order endpoint
 */
export const submitFuturesOrders = async (payload: ISubmitFuturesOrderRequest): Promise<AxiosResponse<any, any>> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.put('futuresOrders', payload)
}
