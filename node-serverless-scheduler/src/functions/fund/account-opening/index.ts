import { handlerPath } from '@libs/handler-resolver';

export const fundAccountOpeningSchedule = {
  handler: `${handlerPath(__dirname)}/handler.schedule`,
  events: [
    {
      schedule: 'cron(0 1 * * ? *)', //Run at 08:00 am (GMT+7) every day
    },
  ],
};

export const fundAccountOpening = {
  handler: `${handlerPath(__dirname)}/handler.run`,
  events: [],
};
