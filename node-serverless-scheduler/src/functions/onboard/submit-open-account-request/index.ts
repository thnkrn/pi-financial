import { handlerPath } from '@libs/handler-resolver';

export const onboardSubmitOpenAccReq = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [
    {
      schedule: 'cron(15 1 ? * MON-FRI *)', // Schedule for reconcile report every work day at 8:15 AM (GMT+7)
    },
  ],
  timeout: 30,
};
