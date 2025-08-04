import { ssoAdminAxiosInstance } from '@/lib/api'
import { ISSOError } from '@/lib/api/clients/sso/link-account/types'
import axios from 'axios'
import type { NextApiRequest, NextApiResponse } from 'next/dist/shared/lib/utils'

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  if (req.method !== 'POST') {
    return res.status(405).json({ message: 'Method not allowed' })
  }

  const { custcode } = req.body

  if (!custcode) {
    return res.status(400).json({ message: 'Missing custcode in request body' })
  }

  try {
    const instance = await ssoAdminAxiosInstance()
    const response = await instance.post('/internal/accounts/sendLinkAccount', { custcode })

    return res.status(200).json({
      message: 'Send link account created successfully',
      data: response?.data,
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
