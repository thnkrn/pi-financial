import { handlerPath } from '@libs/handler-resolver';

export const sampleProcessBatch = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [],
};
