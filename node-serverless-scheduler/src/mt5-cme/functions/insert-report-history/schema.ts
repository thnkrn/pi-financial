export default {
  type: 'object',
  properties: {
    id: { type: 'string' },
    fileName: { type: 'string' },
    userName: { type: 'string' },
    dateFrom: { type: 'string' },
    dateTo: { type: 'string' },
    status: { type: ['string', 'null'] },
    fileKey: { type: ['string', 'null'] },
  },
  required: ['id'],
} as const;
