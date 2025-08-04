import { handlerPath } from '@libs/handler-resolver';
import schema, { geSchema } from './schema';

export const atlasGenPortfolioStatementProcessBatch = {
  handler: `${handlerPath(__dirname)}/handler.portfolioPost`,
  package: {
    include: ['resources/atlas'],
  },
  runtime: 'nodejs18.x',
  events: [
    {
      http: {
        method: 'post',
        path: 'atlasGenPortfolioStatementProcessBatch',
        request: {
          schemas: {
            'application/json': schema,
          },
        },
      },
    },
  ],
  timeout: 60,
};

export const atlasGenPortfolioStatementProcessBatchGet = {
  handler: `${handlerPath(__dirname)}/handler.portfolioGet`,
  package: {
    include: ['resources/atlas'],
  },
  runtime: 'nodejs18.x',
  events: [
    {
      http: {
        method: 'get',
        path: 'atlasGenPortfolioStatementProcessBatch',
      },
    },
  ],
  timeout: 60,
};

export const atlasGenSNCashMovementProcessBatch = {
  handler: `${handlerPath(__dirname)}/handler.snCashMovementPost`,
  package: {
    include: ['resources/atlas'],
  },
  runtime: 'nodejs18.x',
  events: [
    {
      http: {
        method: 'post',
        path: 'atlasGenSNCashMovementProcessBatch',
        request: {
          schemas: {
            'application/json': schema,
          },
        },
      },
    },
  ],
  timeout: 60,
};

export const atlasGenSNCashMovementProcessBatchGet = {
  handler: `${handlerPath(__dirname)}/handler.snCashMovementGet`,
  package: {
    include: ['resources/atlas'],
  },
  runtime: 'nodejs18.x',
  events: [
    {
      http: {
        method: 'get',
        path: 'atlasGenSNCashMovementProcessBatch',
      },
    },
  ],
  timeout: 60,
};

export const atlasGenGlobalEquityStatementProcessBatchGet = {
  handler: `${handlerPath(__dirname)}/handler.genGlobalEquityStatementGet`,
  package: {
    include: ['resources/atlas'],
  },
  runtime: 'nodejs18.x',
  events: [
    {
      http: {
        method: 'get',
        path: 'atlasGenGlobalEquityStatementProcessBatch',
      },
    },
  ],
  timeout: 60,
};

export const atlasGenGlobalEquityStatementProcessBatch = {
  handler: `${handlerPath(__dirname)}/handler.genGlobalEquityStatementPost`,
  package: {
    include: ['resources/atlas'],
  },
  runtime: 'nodejs18.x',
  events: [
    {
      http: {
        method: 'post',
        path: 'atlasGenGlobalEquityStatementProcessBatch',
        request: {
          schemas: {
            'application/json': geSchema,
          },
        },
      },
    },
  ],
  timeout: 60,
};
