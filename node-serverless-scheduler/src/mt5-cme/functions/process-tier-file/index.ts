import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';

export const processTierFile = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [],
  runtime: runtime.node20,
};
