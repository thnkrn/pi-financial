import { handlerPath } from '@libs/handler-resolver';

export const onboardSubmitKycReq = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [
    {
      schedule: 'cron(25 1 ? * MON-FRI *)', // Schedule for submit kyc request every work day at 8:25 AM (GMT+7)
    },
  ],
  timeout: 30,
};
