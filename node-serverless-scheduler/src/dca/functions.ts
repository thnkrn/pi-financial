import { handlerPath } from '@libs/handler-resolver';

export const fundTriggerCronjob = {
  handler: `${handlerPath(__dirname)}/handler.fundTriggerCronjob`,
  description: 'DCA job trigger for Mutual Funds',
  events: [
    {
      schedule: {
        rate: ['cron(5 2 * * ? *)'], // Schedule for trigger every day at 09:05 UTC+7
        enabled: process.env.AWS_ENVIRONMENT === 'production',
      },
    },
  ],
  timeout: 60,
};

export default { fundTriggerCronjob };
