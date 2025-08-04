import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';

export const sendInternalRequest = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  runtime: runtime.node16,
  events: [],
};
