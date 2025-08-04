import { handlerPath } from '@cgs-bank/libs/handlerResolver';
import schema from '@cgs-bank/models/common/basic-request';

export default {
  handler: `${handlerPath(__dirname)}/handler.main`,
  iamRoleStatements: [
    {
      Effect: 'Allow',
      Action: [
        'dynamodb:GetItem',
        'dynamodb:Scan',
        'dynamodb:PutItem',
        'dynamodb:UpdateItem',
      ],
      Resource: `arn:aws:dynamodb:$\{self:provider.region}:*:table/$\{self:provider.environment.AWS_DYNAMODB_API_CACHE_TABLE_NAME}`,
    },
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
        path: 'KKPPayment/PaymentInquiry',
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
