import { blocktradeAxiosInstance } from '@/lib/api'
import {
  IPortfolioRequest,
  IPortfolioResponse,
  IRolloverExpectedFuturesPriceRequest,
  IRolloverExpectedFuturesPriceResponse, IRolloverListRequest, IRolloverListResponse, ISubmitRolloverRequest
} from './types'
import { AxiosResponse } from 'axios'

/**
 * Retrieves portfolio based on the provided filter.
 *
 * @param {IPortfolioRequest} filter - The filter to apply to the portfolio retrieval.
 * @return {Promise<IPortfolioResponse>} A promise that resolves to the retrieved portfolio and related information.
 */
export const getPortfolio = async (filter: IPortfolioRequest): Promise<IPortfolioResponse> => {
  // NOTE: need to define as any type since URLSearchParams only accept type string but we prefer to use our own type for filter
  const objString = '?' + new URLSearchParams(filter as any).toString()

  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`positions${objString}`)

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
 * Requesting the expected futures price for rollover using the provided payload.
 *
 * @param {IRolloverExpectedFuturesPriceRequest} payload - the request payload for requesting the expected futures price for rollover
 * @return {Promise<IRolloverExpectedFuturesPriceResponse>} A promise that resolves to the retrieved expected futures price for rollover
 */
export const getRolloverExpectedFuturesPrice = async (payload: IRolloverExpectedFuturesPriceRequest): Promise<IRolloverExpectedFuturesPriceResponse> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.post('futuresOrders/getRolloverExpectedPrice', payload)
}

/**
 * Retrieves rollover list of the order based on the provided payload.
 *
 * @param {IRolloverListRequest} payload - the request payload for retrieves rollover list of the order based on the provided payload.
 * @return {Promise<IRolloverListResponse>} A promise that resolves to the rollover list of the order.
 */
export const getRolloverList = async (payload: IRolloverListRequest): Promise<IRolloverListResponse> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.get(`positions/rolloverList?blockId=${payload.blockId}`)
}

/**
 * Submitting to rollover the order using the provided payload.
 *
 * @param {ISubmitRolloverRequest} payload - the request payload to rollover the order
 * @return {Promise<AxiosResponse<any, any>>} a promise that resolves to the response from the submitting to rollover the order
 */
export const submitRollover = async (payload: ISubmitRolloverRequest): Promise<AxiosResponse<any, any>> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.post('futuresOrders/rollover', payload)
}
