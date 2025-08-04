import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';


export const setSblMigration = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [
    {
      http: {
        method: 'get',
        path: 'setSblMigration',
      },
    },
  ],
  runtime: runtime.node20,
};
