import { handlerPath } from '@libs/handler-resolver';

export const onboardCheckOpenAccountReq = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [
    {
      schedule: 'cron(15 2 ? * MON-FRI *)', // Schedule for reconcile report every work day at 9:15 AM (GMT+7)
    },
  ],
  timeout: 30,
};
