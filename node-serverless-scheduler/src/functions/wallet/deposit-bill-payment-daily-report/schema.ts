export default {
  type: 'object',
  properties: {
    requestDate: { type: 'string' },
    type: { type: 'string' },
  },
} as const;

export const downloadRequest = {
  type: 'object',
  properties: {
    dateFrom: { type: 'string' },
    dateTo: { type: 'string' },
  },
};
