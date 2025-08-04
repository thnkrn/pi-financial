import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';

const commonConfigs = {
  timeout: 600,
  runtime: runtime.node20,
};

export const dwSuccessfetchAndBuildReport = {
  ...commonConfigs,
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [],
};
