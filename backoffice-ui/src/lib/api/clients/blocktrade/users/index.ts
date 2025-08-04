import { blocktradeAxiosInstance } from '@/lib/api'
import {
  ICreateUserRequest,
  IGetMyUserDataResponse,
} from './types'
import { AxiosResponse } from 'axios'

/**
 * Retrieves the my user information.
 * @return {Promise<IGetCustomerListResponse>} A promise that resolves to the retrieved my user information.
 */
export const getMyUserData = async (): Promise<IGetMyUserDataResponse> => {
  const instance = await blocktradeAxiosInstance()
  const res = await instance.get(`users/getInfo`)

  return res.data
}

/**
 * Submit a creation of a user using the provided payload.
 *
 * @param {ICreateUserRequest} payload - the request payload for creating of a user
 * @return {Promise<AxiosResponse<any, any>>} a promise that resolves to the response from the creating of a user endpoint
 */
export const createUser = async (payload: ICreateUserRequest): Promise<AxiosResponse<any, any>> => {
  const instance = await blocktradeAxiosInstance()

  return await instance.post('users/register', payload)
}
