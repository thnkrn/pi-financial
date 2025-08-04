import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';

const commonConfigs = {
  timeout: 600,
  runtime: runtime.node16,
};

export const calculateDateRange = {
  ...commonConfigs,
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [],
};
