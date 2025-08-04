import { handlerPath } from '@libs/handler-resolver';

export const mt5CreateOracleMonthly = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  description: 'no longer used',
  events: [
    {
      schedule: {
        rate: ['cron(0 3 1 * ? *)'], // Schedule for create oracle monthly report every 1st day off month at 10:00 AM (GMT+7)
        enabled: false,
      },
    },
  ],
  timeout: 30,
};
