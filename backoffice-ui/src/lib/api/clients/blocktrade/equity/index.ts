import { blocktradeAxiosInstance } from '@/lib/api'
import {
  IEquityOrderRequest,
  IEquityOrderResponse,
  ISubmitAmendOrderRequest,
  ISubmitNewOrderRequest
} from './types'
import { AxiosResponse } from 'axios'

/**
 * Retrieves equity orders based on the provided filter.
 *
 * @param {IEquityOrderRequest} filter - The filter to apply to the equity orders retrieval.
 * @return {Promise<IEquityOrderResponse>} A promise that resolves to the retrieved equity orders and related information.
 */
export const getEquityOrders = async (filter: IEquityOrderRequest): Promise<IEquityOrderResponse> => {
  const objString = '?' + new URLSearchParams(filter as any).toString()

  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`equityOrders${objString}`)

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
 * Submit a creation of a equity order using the provided payload.
 *
 * @param {ISubmitNewOrderRequest} payload - the request payload for submitting new of a equity order
 * @return {Promise<AxiosResponse<any, any>>} a promise that resolves to the response from the submitting new of a equity order endpoint
 */
export const submitNewEquityOrder = async (payload: ISubmitNewOrderRequest): Promise<AxiosResponse<any, any>> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.post('equityOrders', payload)
}

/**
 * Submit a amendment of a equity order using the provided payload.
 *
 * @param {number} id - the request id of the equity order to amend
 * @param {ISubmitNewOrderRequest} payload - the request payload for submitting amendment of a equity order
 * @return {Promise<AxiosResponse<any, any>>} a promise that resolves to the response from the submitting amendment of a equity order endpoint
 */
export const submitAmendEquityOrder = async (id: number, payload: ISubmitAmendOrderRequest): Promise<AxiosResponse<any, any>> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.put(`equityOrders/amend/${id}`, payload)
}

/**
 * Submit a cancel of a equity order using the provided payload.
 *
 * @param {number} id - the request id of the equity order to cancel
 * @return {Promise<AxiosResponse<any, any>>} a promise that resolves to the response from the submitting cancel of a equity order endpoint
 */
export const submitCancelEquityOrder = async (id: number): Promise<AxiosResponse<any, any>> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.put(`equityOrders/cancel/${id}`)
}
