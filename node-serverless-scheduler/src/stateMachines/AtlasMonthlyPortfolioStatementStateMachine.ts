import { MailType } from 'src/constants/report';
import { Definition } from '../../serverless';

const settings = {
  batchSize: '2',
};

const definition: Definition = {
  Comment:
    'A state machine that processes customer IDs in batches with retry and error handling',
  StartAt: 'GetCustomers',
  States: {
    GetCustomers: {
      Type: 'Task',
      Parameters: {
        body: {
          batchSize: settings.batchSize,
          mailType: MailType.PortfolioStatement,
        },
      },
      Resource: {
        'Fn::GetAtt': ['atlasGenPortfolioStatementGetCustomers', 'Arn'],
      },
      Next: 'BatchCustomers',
    },
    BatchCustomers: {
      Type: 'Map',
      ItemsPath: '$.body',
      MaxConcurrency: 1,
      Iterator: {
        StartAt: 'ProcessBatch',
        States: {
          ProcessBatch: {
            Type: 'Task',
            Resource: {
              'Fn::GetAtt': ['atlasGenPortfolioStatementProcessBatch', 'Arn'],
            },
            End: true,
          },
        },
      },
      End: true,
    },
  },
};

export const atlasMonthlyPortfolioStatementStateMachine = {
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
  name: `pi-${process.env.AWS_ENVIRONMENT}-AtlasMonthlyPortfolioStatementStateMachine`,
  definition,
};
