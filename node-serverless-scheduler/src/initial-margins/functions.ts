import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';
import { request } from './schema';

export const upsertInitialMargins = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  runtime: runtime.node20,
  events: [
    {
      http: {
        method: 'post',
        path: 'intial-margin',
        request: {
          schemas: {
            'application/json': request,
          },
        },
      },
    },
  ],
};
