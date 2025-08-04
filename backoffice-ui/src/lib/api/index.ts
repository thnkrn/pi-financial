import axios from 'axios'
import 'dotenv/config'
import { getSession } from 'next-auth/react'

interface IConfig {
  backOfficeService: {
    baseUrl: string
  }
  blocktradeService: {
    baseUrl: string
  }
  ssoadminService: {
    baseUrl: string
  }
}

const config: IConfig = {
  backOfficeService: {
    baseUrl: process.env.NEXT_PUBLIC_BACK_OFFICE_API_BASE_URL as string,
  },
  blocktradeService: {
    baseUrl: process.env.NEXT_PUBLIC_BLOCKTRADE_API_BASE_URL as string,
  },
  ssoadminService: {
    baseUrl: process.env.NEXT_PUBLIC_SSOADMIN_API_BASE_URL as string,
  },
}

export const axiosInstance = async (baseURL: string) => {
  const session = await getSession()

  return axios.create({
    baseURL,
    headers: {
      Authorization: `Bearer ${session?.accessToken}`,
      'Content-Type': 'application/json; charset=utf-8',
    },
  })
}

export const axiosInstanceWithoutAuth = async (baseURL: string) => {
  return axios.create({
    baseURL,
    timeout: 10000, // 10 วินาที
  })
}

export const backofficeAxiosInstance = async () => {
  return await axiosInstance(`${config.backOfficeService.baseUrl}`)
}

export const blocktradeAxiosInstance = async () => {
  return await axiosInstance(`${config.blocktradeService.baseUrl}`)
}

export const ssoAdminAxiosInstance = async () => {
  return await axiosInstanceWithoutAuth(`${config.ssoadminService.baseUrl}`)
}
