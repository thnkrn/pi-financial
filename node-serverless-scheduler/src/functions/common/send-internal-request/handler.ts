import { middyfy } from '@libs/lambda';
import got from 'got';
import { ValidatedEventAPIGatewayProxyEvent } from '@libs/api-gateway';
import schema from '@functions/common/send-internal-request/schema';

interface Headers {
  [key: string]: string | string[];
}

const timeoutConfig = { request: Number(process.env.API_TIMEOUT || 100000) };

export const handler: ValidatedEventAPIGatewayProxyEvent<
  typeof schema
> = async (event) => {
  const { url, method, headers, body } = event.body;

  const options = {
    method: method,
    headers: (headers as Headers) || {},
    body: body ? JSON.stringify(body) : undefined,
    timeout: timeoutConfig,
  };
  console.info(`Send a request to endpoint: ${url}`);

  const response = await got(url, options);
  console.info(`Response status: ${response.statusCode}`);

  return {
    statusCode: response.statusCode,
    body: JSON.stringify(response.body),
  };
};

export const main = middyfy(handler);
