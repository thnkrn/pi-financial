import { handlerPath } from '@libs/handler-resolver';

export const atlasGenPortfolioStatementGetCustomers = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [],
};
