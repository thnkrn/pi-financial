import { Definition } from '../../serverless';

const settings = {
  portfolioSummaryBucket: `pi-tech-${process.env.AWS_ENVIRONMENT}-atlas-portfolio-summary`,
  timezone: 'asia/bangkok',
  dayOffset: '30',
};

const definition: Definition = {
  StartAt: 'CleanUpPortfolioInPartition',
  States: {
    // Clean up partition
    CleanUpPortfolioInPartition: {
      Type: 'Task',
      Parameters: {
        body: {
          'date.$': '$.time',
          timezone: settings.timezone,
          dayOffset: settings.dayOffset,
          isPurge: 'true',
          enabled: 'true',
        },
      },
      ResultPath: '$.cleanUpPortfolioInPartitionResult',
      Resource: { 'Fn::GetAtt': ['cleanUpPortfolioInPartition', 'Arn'] },
      End: true,
    },
  },
};

export const atlasMonthlyPortfolioCleanUp = {
  events: [
    {
      schedule: {
        rate: 'cron(0 1 ? * * *)', // Run at 8:00AM BKK Time Every day
        //rate: 'cron(*/5 * ? * MON-FRI *)', // For testing
        enabled: false,
      },
    },
    {
      http: {
        path: 'atlas/cleanup-portfolio-summary',
        method: 'POST',
        request: {
          schemas: {
            'application/json': {
              type: 'object',
              properties: {
                time: { type: 'string' },
              },
              required: ['time'],
            },
          },
        },
      },
    },
  ].filter((q) =>
    process.env.AWS_ENVIRONMENT === 'production' ? q.schedule : true
  ),
  iamRoleStatements: [
    {
      Effect: 'Allow',
      Action: ['s3:GetObject', 's3:ListBucket'],
      Resource: [`arn:aws:s3:::${settings.portfolioSummaryBucket}`],
    },
  ],
  name: `pi-${process.env.AWS_ENVIRONMENT}-AtlasMonthlyPortfolioCleanUp`,
  definition,
};
