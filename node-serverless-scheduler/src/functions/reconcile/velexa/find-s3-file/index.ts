import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';

export const findS3File = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [],
  runtime: runtime.node20,
};
