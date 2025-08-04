export default {
  type: 'object',
  properties: {
    status: { type: 'number' },
    body: {
      type: 'object',
      properties: {
        merchantId: { type: 'string' },
        encryptedValue: { type: 'string' },
      },
    },
  },
} as const;
