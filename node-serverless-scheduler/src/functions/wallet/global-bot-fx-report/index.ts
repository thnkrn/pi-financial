import schema from '@functions/wallet/global-bot-fx-report/schema';
import { handlerPath } from '@libs/handler-resolver';

const commonConfigs = {
  timeout: 30,
};

export const globalBotFxReportScheduler = {
  ...commonConfigs,
  handler: `${handlerPath(__dirname)}/handler.scheduleRunner`,
  events: [
    {
      schedule: 'cron(0 22 L * ? *)',
    },
  ],
};

export const globalBotFxReport = {
  handler: `${handlerPath(__dirname)}/handler.run`,
  events:
    process.env.AWS_ENVIRONMENT !== 'production'
      ? [
          {
            http: {
              method: 'post',
              path: 'report/global-equity/bot-fx',
              request: {
                schemas: {
                  'application/json': schema,
                },
              },
            },
          },
        ]
      : [],
};
