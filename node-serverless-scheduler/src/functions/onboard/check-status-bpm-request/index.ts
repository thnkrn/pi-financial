import { handlerPath } from '@libs/handler-resolver';

export const onboardCheckStatusBpmReq = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [
    {
      schedule: 'cron(0 * * * ? *)', // Schedule for every minutes
    },
  ],
  timeout: 30,
};
