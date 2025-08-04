import schema, {
  downloadRequest,
} from '@functions/wallet/pi-app-deposit-withdraw-reconcile/schema';
import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';

export const piAppDepositWithdrawReportScheduler = {
  handler: `${handlerPath(__dirname)}/handler.sendPiAppDepositWithdrawReport`,
  runtime: runtime.node20,
  events: [
    {
      schedule: 'cron(40 9 * * ? *)', //Run at 4:40 pm (GMT+7) every day
    },
  ],
};

export const piAppDepositWithdraw = {
  handler: `${handlerPath(__dirname)}/handler.run`,
  runtime: runtime.node20,
  events: [
    {
      http: {
        method: 'post',
        path: 'report/wallet/pi-app-deposit-withdraw',
        request: {
          schemas: {
            'application/json': schema,
          },
        },
      },
    },
  ],
};

export const downloadPiAppDepositWithdrawDailyReport = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  runtime: runtime.node20,
  timeout: 30,
  events: [
    {
      http: {
        method: 'post',
        path: 'report/wallet/pi-app-deposit-withdraw/download',
        request: {
          schemas: {
            'application/json': downloadRequest,
          },
        },
      },
    },
  ],
};
