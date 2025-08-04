import { handlerPath } from '@libs/handler-resolver';

export const directusSyncYouTubeVideos = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [
    {
      schedule: {
        // 05:00 (UTC+0) - 12:00 (UTC+7) && 09:00 (UTC+0) - 16:00 (UTC+7) every day
        rate: ['cron(0 5,9 * * ? *)'],
        enabled: process.env.AWS_ENVIRONMENT === 'production',
      },
    },
  ],
  timeout: 30,
};
