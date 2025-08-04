import { handlerPath } from '@libs/handler-resolver';
import schema from './schema';

export const atlasMigrate = {
  handler: `${handlerPath(__dirname)}/handler.atlasMigrate`,
  package: {
    include: ['resources/atlas'],
  },
  runtime: 'nodejs18.x',
  events: [
    {
      http: {
        method: 'post',
        path: 'atlasMigrate',
        request: {
          schemas: {
            'application/json': schema,
          },
        },
      },
    },
  ],
  timeout: 600,
};
