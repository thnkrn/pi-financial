import { ssoAdminAxiosInstance } from '@/lib/api'
import type { NextApiRequest, NextApiResponse } from 'next/dist/shared/lib/utils'

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  // รับค่า username จาก Body
  const { username } = req.body

  if (!username) {
    return res.status(400).json({ error: 'Username is required' })
  }

  try {
    const instance = await ssoAdminAxiosInstance()
    const response = await instance.post(`/internal/auth/request-reset-password/backOffice`, { username })

    return res.status(200).json(response.data)
  } catch (error: any) {
    return res.status(500).json({ error: 'Failed to unlock unlockPin' })
  }
}
