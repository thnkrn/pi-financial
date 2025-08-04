import { MailType } from 'src/constants/report';
import { Definition } from '../../serverless';

const settings = {
  batchSize: '2',
};

const definition: Definition = {
  Comment:
    'A state machine that processes customer IDs in batches with retry and error handling',
  StartAt: 'AtlasMonthlySNCashMovementGetCustomers',
  States: {
    AtlasMonthlySNCashMovementGetCustomers: {
      Type: 'Task',
      Parameters: {
        body: {
          batchSize: settings.batchSize,
          mailType: MailType.SNCashMovement,
        },
      },
      Resource: {
        'Fn::GetAtt': ['atlasGenPortfolioStatementGetCustomers', 'Arn'],
      },
      Next: 'AtlasMonthlySNCashMovementBatchCustomers',
    },
    AtlasMonthlySNCashMovementBatchCustomers: {
      Type: 'Map',
      ItemsPath: '$.body',
      MaxConcurrency: 1,
      Iterator: {
        StartAt: 'AtlasMonthlySNCashMovementProcessBatch',
        States: {
          AtlasMonthlySNCashMovementProcessBatch: {
            Type: 'Task',
            Resource: {
              'Fn::GetAtt': ['atlasGenSNCashMovementProcessBatch', 'Arn'],
            },
            End: true,
          },
        },
      },
      End: true,
    },
  },
};

export const AtlasMonthlyCashMovementsStateMachine = {
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
  name: `pi-${process.env.AWS_ENVIRONMENT}-AtlasMonthlyCashMovementsStateMachine`,
  definition,
};
