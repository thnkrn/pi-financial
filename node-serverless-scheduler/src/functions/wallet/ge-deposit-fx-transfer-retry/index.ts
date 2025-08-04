import { handlerPath } from '@libs/handler-resolver';

const commonConfigs = {
  timeout: 30,
};

export const walletRetryDepositFxTransfer = {
  ...commonConfigs,
  handler: `${handlerPath(__dirname)}/handler.retryFailedDepositFxTransfer`,
  events: [
    {
      schedule: 'cron(0 1 * * ? *)',
    },
  ],
};
