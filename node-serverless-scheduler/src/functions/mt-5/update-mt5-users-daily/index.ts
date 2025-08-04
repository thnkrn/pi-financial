import { handlerPath } from '@libs/handler-resolver';

export const mt5UpdateMT5UsersDaily = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  description: 'no longer used',
  events: [
    {
      schedule: {
        rate: ['cron(5 3 ? * MON-FRI *)'], // Schedule for sync new mt5 users every work day at 10:05 AM (GMT+7)
        enabled: false,
      },
    },
  ],
  timeout: 30,
};
