import { handlerPath } from '@libs/handler-resolver';

export const mt5CreateOracleDaily = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  description: 'no longer used',
  events: [
    {
      schedule: {
        rate: ['cron(5 17 ? * MON-FRI *)'], // Schedule for reconcile report every work day at 00:05 AM (GMT+7)
        enabled: false,
      },
    },
  ],
  timeout: 30,
};
