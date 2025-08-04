import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';

export const snapshotPendingDepositTransactions = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [],
  runtime: runtime.node20,
};
