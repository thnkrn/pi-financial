import { handlerPath } from '@libs/handler-resolver';

export const mt5CreateTaxDaily = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  description: 'no longer used',
  events: [
    {
      schedule: {
        rate: ['cron(15 10 ? * MON-FRI *)'], // Schedule for tax report every working day a 17:15 AM (GMT+7)
        enabled: false,
      },
    },
  ],
  timeout: 30,
};
