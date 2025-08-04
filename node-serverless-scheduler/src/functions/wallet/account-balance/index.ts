import schema from '@functions/wallet/account-balance/schema';
import { handlerPath } from '@libs/handler-resolver';

const commonConfigs = {
  timeout: 30,
};

export const freewillAccountBalanceScheduler = {
  ...commonConfigs,
  handler: `${handlerPath(__dirname)}/handler.updateFreewillAccountBalance`,
  events: [
    {
      schedule: 'cron(0 22 * * ? *)',
    },
  ],
};

export const freewillAccountBalance = {
  handler: `${handlerPath(__dirname)}/handler.run`,
  events:
    process.env.AWS_ENVIRONMENT !== 'production'
      ? [
          {
            http: {
              method: 'post',
              path: 'report/global-equity/sba',
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
