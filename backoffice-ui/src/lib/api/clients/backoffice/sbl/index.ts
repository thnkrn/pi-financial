import { backofficeAxiosInstance } from '@/lib/api'
import { AxiosResponse } from 'axios'
import {
  IGetInstrumentsRequest,
  IGetInstrumentsResponse,
  IGetOrdersRequest,
  IGetOrdersResponse,
  IUpdateOrderActionRequest,
} from './types'

/**
 * Uploads an SBL instrument file.
 *
 * @param {File} file - The file to be uploaded, typically in .csv format.
 * @return {Promise<AxiosResponse<any, any>>} The server response on successful upload.
 * @throws {Error} Throws an error if the upload fails.
 */
export const uploadSblFile = async (file: File): Promise<AxiosResponse<any, any>> => {
  const instance = await backofficeAxiosInstance()

  const formData = new FormData()
  formData.append('file', file)

  return await instance.post('sbl/instruments/upload', formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
      accept: 'text/plain',
    },
  })
}

const buildQueryString = (filter: IGetOrdersRequest) => {
  const params = new URLSearchParams()

  Object.entries(filter).forEach(([key, value]) => {
    if (value === '') return

    if (Array.isArray(value)) {
      value.forEach(val => params.append(key, val))
    } else {
      params.append(key, value.toString())
    }
  })

  return '?' + params.toString()
}

export const getSblOrders = async (filter: IGetOrdersRequest): Promise<IGetOrdersResponse> => {
  const instance = await backofficeAxiosInstance()

  const objString = buildQueryString(filter)
  const res = await instance.get(`sbl/orders/paginate${objString}`)

  return {
    orders: res?.data.data,
    page: res?.data?.page,
    pageSize: res?.data?.pageSize,
    total: res?.data?.total,
    orderBy: res?.data?.orderBy,
    orderDir: res?.data?.orderDir,
  }
}

export const getSblInstruments = async (filter: IGetInstrumentsRequest): Promise<IGetInstrumentsResponse> => {
  const instance = await backofficeAxiosInstance()

  const objString = '?' + new URLSearchParams(filter as any).toString()
  const res = await instance.get(`sbl/instruments/paginate${objString}`)

  return {
    instruments: res?.data.data,
    page: res?.data?.page,
    pageSize: res?.data?.pageSize,
    total: res?.data?.total,
    orderBy: res?.data?.orderBy,
    orderDir: res?.data?.orderDir,
  }
}

export const updateSblOrderAction = async (
  id: string,
  payload: IUpdateOrderActionRequest
): Promise<AxiosResponse<any, any>> => {
  const instance = await backofficeAxiosInstance()

  return await instance.patch(`sbl/orders/${id}`, payload)
}
