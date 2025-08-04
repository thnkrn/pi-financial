import { MailType } from 'src/constants/report';
import { Definition } from '../../serverless';

const settings = {
  batchSize: '2',
};

const definition: Definition = {
  Comment:
    'A state machine that processes customer IDs in batches with retry and error handling',
  StartAt: 'AtlasGlobalEquityGetCustomers',
  States: {
    AtlasGlobalEquityGetCustomers: {
      Type: 'Task',
      Parameters: {
        body: {
          batchSize: settings.batchSize,
          mailType: MailType.GlobalEquityStatement,
        },
      },
      Resource: {
        'Fn::GetAtt': ['atlasGenPortfolioStatementGetCustomers', 'Arn'],
      },
      Next: 'AtlasGlobalEquityBatchCustomers',
    },
    AtlasGlobalEquityBatchCustomers: {
      Type: 'Map',
      ItemsPath: '$.body',
      MaxConcurrency: 1,
      Iterator: {
        StartAt: 'AtlasGlobalEquityProcessBatch',
        States: {
          AtlasGlobalEquityProcessBatch: {
            Type: 'Task',
            Resource: {
              'Fn::GetAtt': [
                'atlasGenGlobalEquityStatementProcessBatch',
                'Arn',
              ],
            },
            End: true,
          },
        },
      },
      End: true,
    },
  },
};

export const AtlasMonthlyGlobalEquityStateMachine = {
  events: [
    {
      // schedule: {
      //   rate: ['cron(0 16 ? * * *)'], // Run at 11PM BKK
      //   enabled: process.env.AWS_ENVIRONMENT === 'production',
      // },
    },
  ],
  iamRoleStatements: [
    {
      Effect: 'Allow',
      Action: ['ssm:GetParameter'],
      Resource: [
        `arn:aws:ssm:ap-southeast-1:*:parameter/${process.env.AWS_ENVIRONMENT}/pi/functions/atlas/*`,
      ],
    },
  ],
  name: `pi-${process.env.AWS_ENVIRONMENT}-AtlasMonthlyGlobalEquityStateMachine`,
  definition,
};
