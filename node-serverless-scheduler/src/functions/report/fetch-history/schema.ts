export default {
  type: 'object',
  properties: {
    page: { type: 'number' },
    pageSize: { type: 'number' },
    reportTypes: { type: ['array', 'null'], items: { type: 'string' } },
    dateFrom: { type: ['string', 'null'] },
    dateTo: { type: ['string', 'null'] },
  },
  required: ['page', 'pageSize'],
} as const;
