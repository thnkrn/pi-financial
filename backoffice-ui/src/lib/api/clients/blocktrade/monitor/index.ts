import { blocktradeAxiosInstance } from '@/lib/api'
import {
  IAdminCreateFuturesRequest,
  IFetchAllUserResponse,
  IFlagICRequest,
  IGetExpectedFuturesPriceRequest,
  IGetExpectedFuturesPriceResponse,
  IGetFuturesPropRequest,
  IGetFuturesPropResponse,
  IListJPOrderResponse,
  IOrdersFetchingResponse, ISplitOrderRequest, ISplitUndoRequest,
  ISubmitJPRequest
} from './types'
import { AxiosResponse } from 'axios'

/**
 * Retrieves all users for Admin.
 *
 * @return {Promise<IFetchAllUserResponse>} A promise that resolves to the retrieved all user and related information.
 */
export const getAllUser = async (): Promise<IFetchAllUserResponse> => {
  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`users/getAll`)

  return res.data
}

/**
 * Flag IC of an order using the provided payload.
 *
 * @param {IFlagICRequest} payload - the request payload to flag IC an order
 * @return {Promise<AxiosResponse<any, any>>} a promise that resolves to the response from the flag of an order endpoint
 */
export const flagIC = async (payload: IFlagICRequest): Promise<AxiosResponse<any, any>> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.post('equityOrders/flagIC', payload)
}

/**
 * Submitting of JPM Orders using the provided payload.
 *
 * @param {ISubmitJPRequest} payload - the request payload to submit JPM orders
 * @return {Promise<AxiosResponse<any, any>>} a promise that resolves to the response from the submitting of JPM orders endpoint
 */
export const submitJP = async (payload: ISubmitJPRequest): Promise<AxiosResponse<any, any>> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.post('equityOrders/submitJP', payload)
}

/**
 * Retrieves the existing JPM orders.
 *
 * @return {Promise<IFetchAllUserResponse>} A promise that resolves to the retrieved existing JPM orders.
 */
export const listJP = async (): Promise<IListJPOrderResponse> => {
  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`equityOrders/listJP`)

  return res.data
}

/**
 * Start fetching the orders.
 *
 * @return {Promise<IOrdersFetchingResponse>} A promise that resolves to the start of fetching orders.
 */
export const startOrdersFetching = async (): Promise<IOrdersFetchingResponse> => {
  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`equityOrders/startOrdersFetching`)

  return res.data
}

/**
 * Stop fetching the orders.
 *
 * @return {Promise<IOrdersFetchingResponse>} A promise that resolves to the stop of fetching orders.
 */
export const stopOrdersFetching = async (): Promise<IOrdersFetchingResponse> => {
  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`equityOrders/stopOrdersFetching`)

  return res.data
}

/**
 * Retrieves the order fetching status.
 *
 * @return {Promise<IOrdersFetchingResponse>} A promise that resolves to the retrieved order fetching status.
 */
export const getOrdersFetchingStatus = async (): Promise<IOrdersFetchingResponse> => {
  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`equityOrders/getOrdersFetching`)

  return res.data
}

/**
 * Submitting of a creation of futures order for admin using the provided payload.
 *
 * @param {IAdminCreateFuturesRequest} payload - the request payload to submit JPM orders
 * @return {Promise<AxiosResponse<any, any>>} a promise that resolves to the response from the submitting of JPM orders endpoint
 */
export const adminCreateFutures = async (payload: IAdminCreateFuturesRequest): Promise<AxiosResponse<any, any>> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.post('futuresOrders/adminCreate', payload)
}

/**
 * Retrieves the expected futures price based on the provided payload.
 *
 * @param {IGetExpectedFuturesPriceRequest} payload - The payload to apply to the expected futures price retrieval.
 * @return {Promise<IGetExpectedFuturesPriceResponse>} A promise that resolves to the expected futures price and related information.
 */
export const getExpectedFuturesPrice = async (payload: IGetExpectedFuturesPriceRequest): Promise<IGetExpectedFuturesPriceResponse> => {
  const instance = await blocktradeAxiosInstance()
  const res = await instance.post(`futuresOrders/getExpectedPrice`, payload)

  return res.data
}

/**
 * Retrieves futures property based on the provided payload.
 *
 * @param {IGetFuturesPropRequest} payload - The payload to apply to the futures property retrieval.
 * @return {Promise<IGetFuturesPropResponse>} A promise that resolves to the retrieved futures property and related information.
 */
export const getFuturesProp = async (payload: IGetFuturesPropRequest): Promise<IGetFuturesPropResponse> => {
  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`futuresProperties/getOne?symbol=${payload.symbol}&series=${payload.series}`)

  return res.data
}

/**
 * Submitting of a split of equity order for admin using the provided payload.
 *
 * @param {ISplitOrderRequest} payload - the request payload to split an equity order
 * @return {Promise<AxiosResponse<any, any>>} a promise that resolves to the response from the split of equity order endpoint
 */
export const splitOrder = async (payload: ISplitOrderRequest): Promise<AxiosResponse<any, any>> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.post('equityOrders/splitOrders', payload)
}

/**
 * Submitting of a undo of split order for admin using the provided payload.
 *
 * @param {ISplitUndoRequest} payload - the request payload to undo an split order
 * @return {Promise<AxiosResponse<any, any>>} a promise that resolves to the response from the undo of split order endpoint
 */
export const splitUndo = async (payload: ISplitUndoRequest): Promise<AxiosResponse<any, any>> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.post('equityOrders/splitUndo', payload)
}
