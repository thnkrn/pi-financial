import { handlerPath } from '@libs/handler-resolver';

export const mt5RemindUserSubscriptionSchedule = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  description: 'no longer used',
  events: [
    {
      schedule: {
        rate: ['cron(0 1 25 * ? *)'], // Run the task at 8AM(UTC+07:00) on the 25th day of every month
        enabled: false,
      },
    },
  ],
  timeout: 30,
};
