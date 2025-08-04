export default {
  type: 'object',
  properties: {
    dateFrom: {
      type: 'string',
    },
    dateTo: {
      type: 'string',
    },
  },
  required: ['dateFrom', 'dateTo'],
} as const;
