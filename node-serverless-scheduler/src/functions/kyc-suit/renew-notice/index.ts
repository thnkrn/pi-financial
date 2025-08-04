import { handlerPath } from '@libs/handler-resolver';

const commonConfigs = {
  timeout: 30,
};

export const kycSuitCreateAdvanceNotices = {
  ...commonConfigs,
  handler: `${handlerPath(__dirname)}/handler.createAdvanceNotices`,
  events: [
    {
      schedule: 'cron(40 0 * * ? *)',
    },
  ],
};

export const kycSuitRemoveAdvanceNotices = {
  ...commonConfigs,
  handler: `${handlerPath(__dirname)}/handler.removeAdvanceNotices`,
  events: [
    {
      schedule: 'cron(0 1 * * ? *)',
    },
  ],
};

export const kycSuitCreateExpiredNotice = {
  ...commonConfigs,
  handler: `${handlerPath(__dirname)}/handler.createExpiredNotice`,
  events: [
    {
      schedule: 'cron(20 1 * * ? *)',
    },
  ],
};
