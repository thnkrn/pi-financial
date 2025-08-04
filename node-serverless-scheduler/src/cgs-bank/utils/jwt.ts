import jwt from 'jsonwebtoken';
import { APIGatewayAuthorizerEvent, Callback, Context } from 'aws-lambda';

const signature = process.env.JWT_SIGNATURE;
export const jwtExpiredTime = 1800;

const getToken = (event: APIGatewayAuthorizerEvent): string => {
  if (!event.type || event.type !== 'TOKEN') {
    throw new Error('Expected "event.type" parameter to have value "TOKEN"');
  }

  const tokenString = event.authorizationToken;
  if (!tokenString) {
    throw new Error('Expected "event.authorizationToken" parameter to be set');
  }

  const match = tokenString.match(/^Bearer (.*)$/);
  if (!match || match.length < 2) {
    throw new Error(
      `Invalid Authorization token - ${tokenString} does not match "Bearer .*"`
    );
  }
  console.log(`tokenString = ${tokenString}`);
  return tokenString && tokenString.split(' ')[1];
};

export const jwtAuth = async (
  event: APIGatewayAuthorizerEvent,
  _context: Context,
  callback: Callback
) => {
  const methodArn = event.methodArn;
  try {
    const token = getToken(event);
    console.log(`token = ${token}`);

    jwt.verify(token, signature, (err, payload) => {
      const username: string = (payload as jwt.JwtPayload)?.username ?? payload;
      if (err) {
        console.log(`err = ${err}`);
        callback(
          null,
          AwsPolicyGeneratorService.generate('user', 'Deny', methodArn)
        );
      }
      console.log(`unwrapped user ${username}`);
      callback(
        null,
        AwsPolicyGeneratorService.generate(username, 'Allow', methodArn)
      );
    });
  } catch (err) {
    callback('Error: Invalid token'); // Return a 500 Invalid token response
  }
};

export const generateAccessToken = (username: { username: string }) => {
  return jwt.sign(username, signature, { expiresIn: jwtExpiredTime });
};

type IGeneric<T> = {
  [index in string | number]: T;
};

class AwsPolicyGeneratorService {
  static generate(
    principalId: string,
    effect: string,
    methodArn: string,
    context?: unknown
  ): unknown {
    const authResponse: IGeneric<unknown> = {};

    authResponse.principalId = principalId;
    if (effect && methodArn) {
      authResponse.policyDocument = {
        Version: '2012-10-17',
        Statement: [
          {
            Action: 'execute-api:Invoke',
            Effect: effect,
            Resource: methodArn,
          },
        ],
      };
    }

    if (context) {
      authResponse.context = context;
    }

    return authResponse;
  }
}
