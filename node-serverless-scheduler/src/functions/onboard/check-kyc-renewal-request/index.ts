import { handlerPath } from '@libs/handler-resolver';

export const onboardCheckKycRenewalReq = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [
    {
      schedule: 'cron(0 0 * * ? *)', // Schedule for check kyc renewal request every day at 00:00 AM (GMT+7)
    },
  ],
  timeout: 30,
};
