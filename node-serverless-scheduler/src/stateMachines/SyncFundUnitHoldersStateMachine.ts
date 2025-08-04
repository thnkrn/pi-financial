import { Definition } from '../../serverless';

const definition: Definition = {
  StartAt: 'GetParameter',
  States: {
    GetParameter: {
      Type: 'Task',
      Resource: 'arn:aws:states:::aws-sdk:ssm:getParameter',
      Next: 'SendRequest',
      Parameters: {
        Name: `/${process.env.AWS_ENVIRONMENT}/pi/functions/fund/fund-srv-host`,
      },
      ResultSelector: {
        'host.$': `$.Parameter.Value`,
      },
    },
    SendRequest: {
      Type: 'Task',
      Resource: { 'Fn::GetAtt': ['sendInternalRequest', 'Arn'] },
      Parameters: {
        body: {
          'url.$': `States.Format('{}/internal/sync/unitHolders', $.host)`,
          method: 'POST',
        },
      },
      End: true,
    },
  },
};

export const syncFundUnitHolders = {
  events: [
    {
      schedule: {
        rate: 'cron(0 19 ? * * *)', // Run at 2AM BKK
        enabled: true,
      },
    },
  ],
  iamRoleStatements: [
    {
      Effect: 'Allow',
      Action: ['ssm:GetParameter'],
      Resource: [
        `arn:aws:ssm:ap-southeast-1:*:parameter/${process.env.AWS_ENVIRONMENT}/pi/functions/fund/*`,
      ],
    },
  ],
  name: `pi-${process.env.AWS_ENVIRONMENT}-SyncFundUnitHolders`,
  definition,
};
