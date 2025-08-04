import schema from '@functions/wallet/reconcile-global/schema';
import { downloadRequest } from '@functions/wallet/pi-app-deposit-withdraw-reconcile/schema';
import { handlerPath } from '@libs/handler-resolver';

const commonConfigs = {
  timeout: 30,
};

export const reconcileAllDayScheduler = {
  ...commonConfigs,
  handler: `${handlerPath(__dirname)}/handler.sendGlobalReconcileReport`,
  events: [
    {
      schedule: 'cron(0 22 * * ? *)',
    },
  ],
};

export const reconcileAllDay = {
  handler: `${handlerPath(__dirname)}/handler.run`,
  events: [
    {
      http: {
        method: 'post',
        path: 'report/reconcile/globalDepositWithdraw',
        request: {
          schemas: {
            'application/json': schema,
          },
        },
      },
    },
  ],
};

export const downloadPiAppGlobalReconcileReport = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  timeout: 30,
  events: [
    {
      http: {
        method: 'post',
        path: 'report/wallet/global-reconcile/download',
        request: {
          schemas: {
            'application/json': downloadRequest,
          },
        },
      },
    },
  ],
};
