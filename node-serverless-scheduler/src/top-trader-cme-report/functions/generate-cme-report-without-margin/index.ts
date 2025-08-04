import { handlerPath } from '@libs/handler-resolver';
import { runtime } from '@libs/lambda';

export const generateCmeReportWithoutMargin = {
  handler: `${handlerPath(__dirname)}/handler.execute`,
  events: [],
  runtime: runtime.node20,
  timeout: 600,
};
