import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';

export const syncSetInitialMargins = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  runtime: runtime.node20,
  events: [
    {
      s3: {
        bucket: `set-initial-margin-sftp-${
          process.env.ENVIRONMENT === 'production' ? 'prod' : 'nonprod'
        }`,
        event: 's3:ObjectCreated:*',
        existing: true,
        rules: [{ suffix: '.dat' }],
      },
    },
  ],
};
