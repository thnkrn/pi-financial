import { handlerPath } from '@libs/handler-resolver';
import schema from './schema';

export const atlasTriggerEmailSender = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [
    {
      http: {
        method: 'post',
        path: 'atlasTriggerEmailSender',
        request: {
          schemas: {
            'application/json': schema,
          },
        },
      },
    },
  ],
  timeout: 300,
};
