import { handlerPath } from '@cgs-bank/libs/handlerResolver';

export default {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [
    {
      http: {
        method: 'post',
        path: 'GetToken',
        cors: true,
        private: true,
      },
    },
  ],
};
