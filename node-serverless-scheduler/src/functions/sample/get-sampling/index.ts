import { handlerPath } from '@libs/handler-resolver';

export const sampleGetSampling = {
  handler: `${handlerPath(__dirname)}/handler.main`,
  events: [],
};
