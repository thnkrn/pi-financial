import { handlerPath } from '@cgs-bank/libs/handlerResolver';

export default {
  handler: `${handlerPath(__dirname)}/handler.main`,
  iamRoleStatements: [
    {
      Effect: 'Allow',
      Action: ['secretsmanager:GetSecretValue'],
      Resource: `*`,
    },
    {
      Effect: 'Allow',
      Action: ['ssm:GetParameters'],
      Resource: `arn:aws:ssm:$\{aws:region}:*:parameter/${process.env.AWS_ENVIRONMENT}/pi/functions/cgs/*`,
    },
  ],
  events: [
    {
      http: {
        method: 'post',
        path: 'KKPPayment/BilledPaymentCallback',
        cors: true,
      },
    },
  ],
};
