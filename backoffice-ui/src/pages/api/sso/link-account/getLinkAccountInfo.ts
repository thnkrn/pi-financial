import { ssoAdminAxiosInstance } from '@/lib/api'
import { ISSOError } from '@/lib/api/clients/sso/link-account/types'
import axios from 'axios'
import type { NextApiRequest, NextApiResponse } from 'next/dist/shared/lib/utils'

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  const url = new URL(req.url ?? '', `http://${req.headers.host}`)
  const custcode = url.searchParams.get('custcode')

  if (!custcode) {
    return res.status(400).json({
      data: [],
      currentPage: 1,
      pageSize: 0,
      hasNextPage: false,
      hasPreviousPage: false,
      totalPages: 0,
    })
  }

  try {
    const instance = await ssoAdminAxiosInstance()
    const response = await instance.get(`/internal/accounts/send-link-account/${custcode}`)

    const item = response?.data?.data

    return res.status(200).json({
      data: [
        {
          id: item.id,
          email: item.email,
          custcode: item.custcode,
          userId: item.userId,
          createdAt: item.createdAt,
          usedAt: item.usedAt,
          isUsed: item.isUsed,
        },
      ],
      currentPage: 1,
      pageSize: 1,
      hasNextPage: false,
      hasPreviousPage: false,
      totalPages: 1,
    })
  } catch (error) {
    const result: ISSOError = {
      status: 500,
      title: 'Internal Server Error',
      detail: 'An unexpected error occurred.',
    }

    if (axios.isAxiosError(error)) {
      result.status = error.response?.status || 500
      result.detail = error.response?.data?.detail || error.message || result.detail
      result.title = error.response?.data?.title || result.title
    } else if (error instanceof Error) {
      result.detail = error.message
    }

    return res.status(result.status).json(result)
  }
}
