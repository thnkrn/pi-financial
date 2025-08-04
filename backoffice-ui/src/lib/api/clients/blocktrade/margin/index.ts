import { blocktradeAxiosInstance } from '@/lib/api'
import {
  IGetMarginDateResponse,
  IGetMarginPDFRequest,
  IGetMarginPDFResponse,
  IImportMarginRequest,
} from './types'
import { AxiosResponse } from 'axios'

/**
 * Retrieves imported margin date list.
 *
 * @return {Promise<IEquityOrderResponse>} A promise that resolves to the retrieved imported margin date list.
 */
export const getMarginDate = async (): Promise<IGetMarginDateResponse> => {
  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`importMargin`)

  return res?.data
}

/**
 * Retrieves PDF of Margin Report.
 *
 * @param {IGetMarginPDFRequest} payload - the request payload for retrieving PDF of Margin Report
 * @return {Promise<IEquityOrderResponse>} A promise that resolves to the retrieved PDF of Margin Report.
 */
export const getMarginPDF = async (payload: IGetMarginPDFRequest): Promise<IGetMarginPDFResponse> => {
  const instance = await blocktradeAxiosInstance()
  const { effectiveDate, effectiveDateFrom, institute } = payload

  let url = `pdfMargin?effectiveDate=${effectiveDate}`
  if (effectiveDateFrom) {
    url += `&effectiveDateFrom=${effectiveDateFrom}`
  }
  url += `&institute=${institute}`

  const res = await instance.get(url)

  return res?.data
}

/**
 * Import the margin information using the provided payload.
 *
 * @param {IImportMarginRequest} payload - the request payload for import the margin information
 * @return {Promise<AxiosResponse<any, any>>} a promise that resolves to the response from the import of margin endpoint
 */
export const importMargin = async (payload: IImportMarginRequest): Promise<AxiosResponse<any, any>> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.post('importMargin', payload)
}
