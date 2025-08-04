import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';
import schema from './schema';

export const fetchHistory = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  runtime: runtime.node20,
  events: [
    {
      http: {
        method: 'post',
        path: 'list-report-history',
        request: {
          schemas: {
            'application/json': schema,
          },
        },
      },
    },
  ],
};
