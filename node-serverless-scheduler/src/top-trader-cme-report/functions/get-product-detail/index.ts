import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';

export const getProductDetail = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  iamRoleStatements: [
    {
      Effect: 'Allow',
      Action: ['secretsmanager:GetSecretValue'],
      Resource: `*`,
    },
  ],
  events: [],
  runtime: runtime.node20,
};
