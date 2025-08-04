export const RESPONSE_DESCRIPTIONS_TO_HIDE_FAILED_REASON = [
  'Incorrect Source',
  'Insufficient Balance',
  'Unfavourable FX',
  'Refund Success',
  'Amount Mismatch',
  'Name Mismatch',
] as const

export type ResponseDescriptionsToHideFailedReason = (typeof RESPONSE_DESCRIPTIONS_TO_HIDE_FAILED_REASON)[number]

export const isResponseDescriptionExists = (responseCode: string): boolean =>
  RESPONSE_DESCRIPTIONS_TO_HIDE_FAILED_REASON.some(responseDescription => responseDescription.startsWith(responseCode))
