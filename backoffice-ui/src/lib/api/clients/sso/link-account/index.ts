import { ISSOError, ISSOResponse } from "./types"


const handleSSOResponse = async <ISSOResponse>(response: Response): Promise<ISSOResponse> => {
  const json = await response.json()

  if (!response.ok) {
    const error: ISSOError = {
      status: response.status,
      title: json?.title || 'SSO_ERROR',
      detail: json?.detail || json?.message || 'Unknown error',
    }
    throw error
  }

  return json as ISSOResponse
}

export const getLinkAccountInfo = async (
  custcode: string
): Promise<ISSOResponse> => {
  const response = await fetch(`/api/sso/link-account/getLinkAccountInfo?custcode=${custcode}`)
  return handleSSOResponse<ISSOResponse>(response)
}

export const sendLinkAccount = async (
  custcode: string
): Promise<ISSOResponse> => {
  const response = await fetch('/api/sso/link-account/sendLinkAccount', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ custcode }),
  })
  return handleSSOResponse<ISSOResponse>(response)
}