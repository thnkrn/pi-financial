export default {
  type: 'object',
  properties: {
    identificationHash: {
      type: 'string',
    },
    custcode: {
      type: 'string',
    },
    marketingId: {
      type: 'string',
    },
    sendDate: {
      type: 'string',
      format: 'date',
    },
    emailToSend: {
      type: 'string',
    },
  },
  required: ['marketingId', 'sendDate'],
} as const;

export const geSchema = {
  type: 'object',
  properties: {
    identificationHash: {
      type: 'string',
    },
    custcode: {
      type: 'string',
    },
    marketingId: {
      type: 'string',
    },
    dateFrom: {
      type: 'string',
      format: 'date',
    },
    dateTo: {
      type: 'string',
      format: 'date',
    },
    emailToSend: {
      type: 'string',
    },
  },
  required: ['marketingId', 'dateFrom', 'dateTo'],
};
