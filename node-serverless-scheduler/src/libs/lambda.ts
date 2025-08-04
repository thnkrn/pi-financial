import middy from '@middy/core';
import doNotWaitForEmptyEventLoop from '@middy/do-not-wait-for-empty-event-loop';
import middyJsonBodyParser from '@middy/http-json-body-parser';
import { AwsLambdaRuntime } from '@serverless/typescript';

export const middyfy = (handler) => {
  return middy()
    .use(doNotWaitForEmptyEventLoop())
    .use(middyJsonBodyParser())
    .handler(handler);
};

const node16: AwsLambdaRuntime = 'nodejs16.x';
const node20: AwsLambdaRuntime = 'nodejs20.x';

export const runtime = {
  node16,
  node20,
};
