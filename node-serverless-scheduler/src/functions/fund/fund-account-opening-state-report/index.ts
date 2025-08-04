import { handlerPath } from '@libs/handler-resolver';

export const fundAccOpeningStateReportSchedule = {
  handler: `${handlerPath(__dirname)}/handler.schedule`,
  events: [
    {
      schedule: 'cron(0 7 * * ? *)', //Run at 07:00 am (UTC) every day
    },
  ],
};

export const fundAccOpeningStateReport = {
  handler: `${handlerPath(__dirname)}/handler.run`,
  events: [],
};
