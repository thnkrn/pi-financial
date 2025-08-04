import { formatJSONResponse } from '@cgs-bank/libs/apiGateway';
import { middyfy } from '@libs/lambda';
import { generateAccessToken, jwtExpiredTime } from '@cgs-bank/utils/jwt';

const handler = async (event: any) => {
  console.log(event);

  let user: string;
  switch (event.headers['x-api-key']) {
    case process.env.INTERNAL_API_KEY:
      user = 'Internal';
      break;
    default:
      user = '';
  }

  const token = generateAccessToken({
    username: user,
  });

  const response = {
    result: 'success',
    body: {
      token: token,
      type: 'Bearer',
      expires_in: jwtExpiredTime,
    },
  };

  return formatJSONResponse(response);
};

export const main = middyfy(handler);
