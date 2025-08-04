export const FILE_TYPE_CONFIG = {
  clientAccountFilePrefix: {
    label: 'Client Account',
    allowedTypes: ['text/plain'] as string[],
    allowedExtensions: ['.txt'] as string[],
    accept: '.txt,text/plain',
  },
  commissionGroupFilePrefix: {
    label: 'Commission Group',
    allowedTypes: ['text/plain'] as string[],
    allowedExtensions: ['.txt'] as string[],
    accept: '.txt,text/plain',
  },
  tierFilePrefix: {
    label: 'Tier',
    allowedTypes: ['text/plain'] as string[],
    allowedExtensions: ['.txt'] as string[],
    accept: '.txt,text/plain',
  },
  traderFilePrefix: {
    label: 'Trader',
    allowedTypes: ['text/plain'] as string[],
    allowedExtensions: ['.txt'] as string[],
    accept: '.txt,text/plain',
  },
  traderGroupFilePrefix: {
    label: 'Trader Group',
    allowedTypes: ['text/plain'] as string[],
    allowedExtensions: ['.txt'] as string[],
    accept: '.txt,text/plain',
  },
  transactionFeeFilePrefix: {
    label: 'Transaction Fee',
    allowedTypes: ['text/plain'] as string[],
    allowedExtensions: ['.txt'] as string[],
    accept: '.txt,text/plain',
  },
  marginSymbolFilePrefix: {
    label: 'Margin Symbol',
    allowedTypes: ['application/json'] as string[],
    allowedExtensions: ['.json'] as string[],
    accept: '.json,application/json',
  },
}

export type FileTypeKey = keyof typeof FILE_TYPE_CONFIG
