import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';

export const getPreSignedUrl = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  runtime: runtime.node20,
  events: [
    {
      http: {
        method: 'get',
        path: 'report/{reportId}/presigned-url',
      },
    },
  ],
};
