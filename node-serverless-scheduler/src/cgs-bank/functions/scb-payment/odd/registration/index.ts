import { handlerPath } from '@cgs-bank/libs/handlerResolver';
import schema from '@cgs-bank/models/common/basic-request';

const deprecated_env =
  process.env.AWS_ENVIRONMENT == 'staging' ? 'uat' : 'prod';

export default {
  handler: `${handlerPath(__dirname)}/handler.main`,
  iamRoleStatements: [
    {
      Effect: 'Allow',
      Action: ['ssm:GetParameters', 'ssm:GetParameter'],
      Resource: [
        `arn:aws:ssm:$\{aws:region}:*:parameter/${process.env.AWS_ENVIRONMENT}/pi/functions/cgs/*`,
        `arn:aws:ssm:$\{aws:region}:*:parameter/${deprecated_env}/cgs/*`,
      ],
    },
    {
      Effect: 'Allow',
      Action: ['secretsmanager:GetSecretValue'],
      Resource: `*`,
    },
  ],
  events: [
    {
      http: {
        method: 'post',
        path: 'SCBPayment/Registration',
        cors: true,
        authorizer: {
          name: 'jwtAuth',
        },
        request: {
          schemas: {
            'application/json': schema,
          },
        },
      },
    },
  ],
};
