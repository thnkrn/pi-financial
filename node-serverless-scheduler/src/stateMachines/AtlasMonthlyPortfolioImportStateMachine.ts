import { Definition } from '../../serverless';

const settings = {
  portfolioSummaryBucket: `pi-tech-${process.env.AWS_ENVIRONMENT}-atlas-portfolio-summary`,
  timezone: 'asia/bangkok',
  dayOffset: '10',
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
          'dayOffset.$': '$.dayOffset',
          timezone: settings.timezone,
          isPurge: 'false',
          enabled: 'false',
        },
      },
      ResultPath: '$.cleanUpPortfolioInPartitionResult',
      Resource: { 'Fn::GetAtt': ['cleanUpPortfolioInPartition', 'Arn'] },
      Next: 'CalculateDateRange',
    },
    CalculateDateRange: {
      Type: 'Task',
      Parameters: {
        body: {
          'date.$': '$.time',
          'dayOffset.$': '$.dayOffset',
          timezone: settings.timezone,
        },
      },
      ResultPath: '$.calculateDateRangeResult',
      Resource: { 'Fn::GetAtt': ['calculateDateRange', 'Arn'] },
      Next: 'IterateOverDatesForImportPortfolio',
    },
    IterateOverDatesForImportPortfolio: {
      Type: 'Map',
      ItemsPath: '$.calculateDateRangeResult.body.dateRange.dates',
      MaxConcurrency: 1,
      Iterator: {
        StartAt: 'ImportPortfolioDailySnapshot',
        States: {
          ImportPortfolioDailySnapshot: {
            Type: 'Task',
            Parameters: {
              body: {
                bucket: settings.portfolioSummaryBucket,
                'date.$': '$.date',
                importType: 'incremental',
              },
            },
            ResultPath: '$.importPortfolioDailySnapshotResult',
            Resource: { 'Fn::GetAtt': ['importPortfolioDailySnapshot', 'Arn'] },
            End: true,
          },
        },
      },
      End: true,
    },
  },
};

export const atlasMonthlyPortfolioImport = {
  events: [
    {
      schedule: {
        rate: 'cron(0 1 ? * * *)', // Run at 8:00 AM BKK Time Every day
        //rate: 'cron(*/30 * ? * MON-FRI *)', // For testing
        enabled: true,
        inputTransformer: {
          inputPathsMap: {
            time: '$.time',
          },
          inputTemplate: JSON.stringify({
            dayOffset: '10',
            time: '<time>',
          }),
        },
      },
    },
    {
      schedule: {
        rate: 'cron(0 6 ? * * *)', // Run at 1:00 PM BKK Time Every day
        enabled: true,
        inputTransformer: {
          inputPathsMap: {
            time: '$.time',
          },
          inputTemplate: JSON.stringify({
            dayOffset: '3',
            time: '<time>',
          }),
        },
      },
    },
    {
      http: {
        path: 'atlas/import-portfolio-summary',
        method: 'POST',
        request: {
          schemas: {
            'application/json': {
              type: 'object',
              properties: {
                time: { type: 'string' },
                dayOffset: { type: 'string' },
              },
              required: ['time', 'dayOffset'],
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
  name: `pi-${process.env.AWS_ENVIRONMENT}-AtlasMonthlyPortfolioImport`,
  definition,
};
