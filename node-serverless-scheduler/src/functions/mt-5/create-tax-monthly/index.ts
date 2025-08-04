import { handlerPath } from '@libs/handler-resolver';

export const mt5CreateTaxMonthly = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  description: 'no longer used',
  events: [
    {
      schedule: {
        rate: ['cron(0 3 1 * ? *)'], // Schedule for tax report every 1st working day of month at 10:00 AM (GMT+7)
        enabled: false,
      },
    },
  ],
  timeout: 30,
};
