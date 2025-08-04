export const request = {
  type: 'object',
  properties: {
    bucketName: { type: 'string' },
    fileKey: { type: 'string' },
  },
  required: ['bucketName', 'fileKey'],
};
