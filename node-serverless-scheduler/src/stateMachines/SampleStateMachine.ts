import { Definition } from '../../serverless';

const settings = {
  batchSize: '2',
};

const definition: Definition = {
  Comment: 'A sampling statemachine which using for testing purpose',
  StartAt: 'GetSampling',
  States: {
    GetSampling: {
      Type: 'Task',
      Parameters: {
        body: {
          batchSize: settings.batchSize,
        },
      },
      Resource: {
        'Fn::GetAtt': ['sampleGetSampling', 'Arn'],
      },
      Next: 'BatchSample',
    },
    BatchSample: {
      Type: 'Map',
      ItemsPath: '$.body',
      MaxConcurrency: 1,
      Iterator: {
        StartAt: 'SampleProcessBatch',
        States: {
          SampleProcessBatch: {
            Type: 'Task',
            Resource: {
              'Fn::GetAtt': ['sampleProcessBatch', 'Arn'],
            },
            End: true,
          },
        },
      },
      End: true,
    },
  },
};

export const sample = {
  events: [{}],
  iamRoleStatements: [
    {
      Effect: 'Allow',
      Action: ['ssm:GetParameter'],
      Resource: [
        `arn:aws:ssm:ap-southeast-1:*:parameter/${process.env.AWS_ENVIRONMENT}/pi/functions/sample/*`,
      ],
    },
  ],
  name: `pi-${process.env.AWS_ENVIRONMENT}-SampleStateMachine`,
  definition,
};
