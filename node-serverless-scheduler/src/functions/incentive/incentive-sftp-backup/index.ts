import { handlerPath } from '@libs/handler-resolver';

// no longer used
export const incentiveSftpBackup = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [
    {
      schedule: {
        rate: ['cron(0 1 * * ? *)'], // Run the task at 08:00(UTC+07:00) every day.
        enabled: false,
      },
    },
  ],
  timeout: 30,
};
