import { handlerPath } from '@libs/handler-resolver';

export const mt5UpdateSubscriptionsDaily = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  description: 'no longer used',
  events: [
    {
      schedule: {
        rate: ['cron(5 10 ? * MON-FRI *)'], // Schedule for reconcile report every 17:05 AM (GMT+7)
        enabled: false,
      },
    },
  ],
  timeout: 30,
};
