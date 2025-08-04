import { handlerPath } from '@cgs-bank/libs/handlerResolver';
import schema from '@cgs-bank/models/common/basic-request';
export default {
  handler: `${handlerPath(__dirname)}/handler.main`,
  iamRoleStatements: [
    {
      Effect: 'Allow',
      Action: ['ssm:GetParameters'],
      Resource: `arn:aws:ssm:$\{aws:region}:*:parameter/${process.env.AWS_ENVIRONMENT}/pi/functions/cgs/*`,
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
        path: 'BAYPayment/Registration',
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
