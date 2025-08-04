import { handlerPath } from '@libs/handler-resolver';

export const resyncDataFromFreewill = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [
    {
      schedule: 'cron(30 1 ? * MON-FRI *)', // 8.30 GMT+7 MON-FRI
    },
  ],
  timeout: 30,
};
