import { ssoAdminAxiosInstance } from '@/lib/api'
import type { NextApiRequest, NextApiResponse } from 'next/dist/shared/lib/utils'

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  // รับค่า username จาก Body
  const { username, newPin } = req.body

  if (!username) {
    return res.status(400).json({ error: 'Username is required' })
  }

  try {
    const instance = await ssoAdminAxiosInstance()
    const response = await instance.post(`/internal/accounts/forceChangePin`, { username, newPin })

    // check if the status is not 200
    if (response.status !== 200) {
      res.status(400).json(response.data)
    }

    return res.status(200).json(response.data)
  } catch (error: any) {
    return res.status(500).json({ error: 'Failed to unlock account' })
  }
}
