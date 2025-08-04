import { handlerPath } from '@cgs-bank/libs/handlerResolver';

export default {
  handler: `${handlerPath(__dirname)}/jwt.jwtAuth`,
};
