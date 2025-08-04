export default {
  type: 'object',
  properties: {
    marketingIds: {
      type: 'array',
      items: { type: 'string' },
    },
    customerSubTypes: {
      type: 'array',
      items: { type: 'string' },
    },
    cardTypes: {
      type: 'array',
      items: { type: 'string' },
    },
    emailType: {
      type: 'string',
    },
  },
} as const;
