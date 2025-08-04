import type { ValidatedEventAPIGatewayProxyEvent } from '@cgs-bank/libs/apiGateway';
import { formatJSONResponse } from '@cgs-bank/libs/apiGateway';
import { middyfy } from '@cgs-bank/libs/lambda';
import { decrypt } from '@cgs-bank/external-api-services/scb-api/scb-rsa';
import { RegistrationCallbackResponse } from '@cgs-bank/models/common/registration';
import { BankServiceResponseWrapper } from '@cgs-bank/models/common/bank-service-response-wrapper';
import { exceptionLogger } from '@cgs-bank/utils/exceptionLogger';
import { ssmWrapper } from '@cgs-bank/external-api-services/scb-api/shared-instance';
import { SCBRegistrationInformation } from '@cgs-bank/external-api-services/scb-api/response/scb-dd-registration-information';
import { createScbDDRegisterResult } from '@cgs-bank/database-services/scb-db-services';
import { REGISTRATION_RESPONSE_SUCCESS_CODE } from '@cgs-bank/external-api-services/scb-api/constant';
import piCallback from '@cgs-bank/external-api-services/pi-callback/pi-callback';
import schema from './schema';

const handler: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  try {
    const data = event.body;
    console.log('Body');
    console.log(data);

    const { encryptedValue } = data.body;
    console.log('encryptedValue');
    console.log(encryptedValue);

    const publicKey = await ssmWrapper.getValue();
    console.log(publicKey);

    const decryptedMessage = await decrypt(encryptedValue, publicKey);
    const regisInfo = JSON.parse(
      decryptedMessage
    ) as SCBRegistrationInformation;
    console.log('SCBRegistrationInformation');
    console.log(regisInfo);

    const request: BankServiceResponseWrapper<RegistrationCallbackResponse> = {
      status: {
        status: regisInfo.statusCode === REGISTRATION_RESPONSE_SUCCESS_CODE,
        internalStatusCode: '200',
        internalStatusDescription: 'Success',
        externalStatusCode: regisInfo.statusCode,
        externalStatusDescription: regisInfo.statusDesc,
      },
      data: {
        registrationRefCode: regisInfo.regRef,
        bankAccountNo: regisInfo?.accountNo ?? '',
      },
    };
    console.log('RegistrationCallbackResponse');
    console.log(request);

    await createScbDDRegisterResult({
      ...regisInfo,
      ...request.data,
    });

    const piResponse = await piCallback.postODDRegistrationCallback(request);
    console.log('PiCallback');
    console.log(piResponse);

    return formatJSONResponse({
      message: 'Success',
    });
  } catch (error) {
    exceptionLogger(error);
    return formatJSONResponse({
      message: 'Success',
    });
  }
};

export const main = middyfy(handler);
