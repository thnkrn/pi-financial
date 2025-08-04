export const convertCurrencyPrefixToCurrencySymbol = (cur: string): string => {
  if (cur.toLowerCase() === 'usd') return '$'

  return '฿'
}

const getProductType = (accountType: string) => (accountType === 'Global Equity' ? 'ge' : 'non-ge')

export const getAccountTypeHref = (accountType: string, channelName: string, transactionType: string): string => {
  const productType = channelName === 'SetTrade E-Payment' ? 'settrade' : getProductType(accountType)

  if (transactionType === 'deposit') {
    return `/deposit/${productType}/`
  } else {
    return `/withdraw/${productType}/`
  }
}

export const getAccountTypeName = (accountType: string): string => {
  if (accountType === 'Global Equity') return 'Global Equity'

  return 'Thai Equity & TFEX'
}

export const getBankURL = (channel: string): string => {
  if (!channel || channel === 'ALL') return 'banks'

  return `banks?channel=${channel}`
}

export const getProductTypeHref = (accountType: string, transactionType: string): string => {
  const productType = accountType === 'Global Equity' ? 'ge' : 'non-ge'

  if (transactionType === 'deposit') {
    return `/deposit/${productType}/`
  } else {
    return `/withdraw/${productType}/`
  }
}

export const bytesToKilobytes = (bytes: number) => {
  return `${(bytes / 1024).toFixed(2)} KB`
}

export const mbToBytes = (megabytes: number) => {
  const bytes = megabytes * 1024 * 1024

  return bytes
}
