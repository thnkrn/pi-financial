import {
  formatJSONResponse,
  ValidatedEventAPIGatewayProxyEvent,
} from '@cgs-bank/libs/apiGateway';
import { middyfy } from '@cgs-bank/libs/lambda';
import schema from '@cgs-bank/models/common/basic-request';
import bayAuthApi from '@cgs-bank/external-api-services/bay-api/authentication';
import bayRegistrationApi from '@cgs-bank/external-api-services/bay-api/registration';
import { RegisterPrepareRequest } from '@cgs-bank/external-api-services/bay-api/request/register-prepare-request';
import { exceptionLogger } from '@cgs-bank/utils/exceptionLogger';
import { BankServiceResponseWrapper } from '@cgs-bank/models/common/bank-service-response-wrapper';
import {
  RegistrationRequest,
  RegistrationRequestScheme,
  RegistrationResponse,
} from '@cgs-bank/models/common/registration';
import * as aes from '@cgs-bank/utils/aes-coder';
import validateJson from '@cgs-bank/utils/jsonValidator';
import { AuthenticationRequest } from '@cgs-bank/external-api-services/bay-api/request/authentication-request';
import { getConfigFromSsm } from '@libs/ssm-config';

const handler: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  const [baseUrl, clientId, clientSecret] = await getConfigFromSsm(
    'cgs/bank-services/bay',
    ['base-api', 'client-id', 'client-secret']
  );
  try {
    const data = JSON.parse(aes.decrypt(event.body.data));
    validateJson(RegistrationRequestScheme, data);

    const input = data as RegistrationRequest;
    console.log('RegistrationRequest');
    console.log(input);

    // auth
    const authenticationRequest: AuthenticationRequest = {
      grantType: 'client_credentials',
      clientId: clientId,
      clientSecret: clientSecret,
    };
    console.info('AuthenticationRequest');
    console.info(authenticationRequest);

    const authenticationResponse = await bayAuthApi.getToken(
      'client_credentials',
      clientId,
      clientSecret
    );
    console.info('AuthenticationResponse');
    console.info(authenticationResponse);

    // register prepare api
    const registerPrepareRequest: RegisterPrepareRequest = {
      ref1: input.registrationRefCode,
      ref2: input.citizenId,
    };
    console.info('RegisterPrepareRequest');
    console.info(registerPrepareRequest);

    const registerPrepareResponse = await bayRegistrationApi.registerPrepare(
      registerPrepareRequest,
      authenticationResponse.access_token
    );
    console.info('registerPrepareResponse');
    console.info(registerPrepareResponse);

    // register authorization api url
    const bayUrl = `${baseUrl}/auth/oauth/v2/authorize?response_type=code&client_id=${clientId}&redirect_uri=${input.redirectUrl}&scope=direct-debit-registration&oauth_context_id=${registerPrepareResponse.oauth_context_id}`;
    const output: BankServiceResponseWrapper<RegistrationResponse> = {
      status: {
        externalStatusCode: null,
        externalStatusDescription: null,
        internalStatusCode: '200',
        internalStatusDescription: 'Success',
        status: true,
      },
      data: {
        webUrl: bayUrl,
      },
    };

    console.log('RegistrationResponse');
    console.info(output);

    return formatJSONResponse({
      data: aes.encrypt(JSON.stringify(output)),
    });
  } catch (error) {
    exceptionLogger(error);
  }
};

export const main = middyfy(handler);
