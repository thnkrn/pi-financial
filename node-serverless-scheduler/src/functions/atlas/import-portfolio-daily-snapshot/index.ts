import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';

const commonConfigs = {
  timeout: 900,
  runtime: runtime.node16,
};

export const importPortfolioDailySnapshot = {
  ...commonConfigs,
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [],
};
