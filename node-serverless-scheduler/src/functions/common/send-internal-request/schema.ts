export default {
  type: 'object',
  properties: {
    url: { type: 'string' },
    method: {
      type: 'string',
      enum: ['GET', 'POST', 'PUT', 'DELETE', 'PATCH'],
    },
    headers: { type: 'object' },
    body: { type: 'object' },
  },
  required: ['url', 'method'],
} as const;
