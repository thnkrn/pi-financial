export default {
  type: 'object',
  properties: {
    id: { type: 'string' },
    reportName: { type: 'string' },
    userName: { type: 'string' },
    dateFrom: { type: 'string' },
    dateTo: { type: 'string' },
    status: { type: 'string' },
    fileName: { type: 'string' },
  },
  required: ['id'],
} as const;
