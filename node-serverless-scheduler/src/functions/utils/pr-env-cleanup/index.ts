import { handlerPath } from '@libs/handler-resolver';
import schema from '@functions/utils/pr-env-cleanup/schema';

const commonConfigs = {
  timeout: 30,
};

export const PrEnvCleanUpScheduler = {
  ...commonConfigs,
  handler: `${handlerPath(__dirname)}/handler.prEnvCleanUp`,
  events:
    process.env.ENVIRONMENT == 'staging'
      ? [
          {
            schedule: 'cron(0 0 ? * SAT *)',
          },
        ]
      : [],
};

export const PrEnvCleanUp = {
  handler: `${handlerPath(__dirname)}/handler.run`,
  events:
    process.env.AWS_ENVIRONMENT !== 'production'
      ? [
          {
            http: {
              method: 'post',
              path: 'utils/pr-env-clean-up',
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
