import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';

export const syncThaiBmaInstruments = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [
    {
      s3: {
        bucket: `bond-thai-bma-instruments-${
          process.env.AWS_ENVIRONMENT === 'production' ? 'prod' : 'nonprod'
        }`,
        event: 's3:ObjectCreated:*',
        existing: true,
        rules: [{ suffix: '.csv' }],
      },
    },
  ],
  runtime: runtime.node20,
};
