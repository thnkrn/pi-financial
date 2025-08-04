import { formatJSONResponse } from '@cgs-bank/libs/apiGateway';
import { middyfy } from '@cgs-bank/libs/lambda';
import { exceptionLogger } from '@cgs-bank/utils/exceptionLogger';
import piCallback from '@cgs-bank/external-api-services/pi-callback/pi-callback';
import { KbankRegistrationCallback } from '@cgs-bank/external-api-services/kbank-api/kbank-registration-callback';
import { createKBANKDDRegisterResult } from '@cgs-bank/database-services/kbank-db-services';
import { BankServiceResponseWrapper } from '@cgs-bank/models/common/bank-service-response-wrapper';
import { RegistrationCallbackResponse } from '@cgs-bank/models/common/registration';
import { KBANK_REGISTRATION_SUCCESS } from '@cgs-bank/external-api-services/kbank-api/constant';

const handler = async (event) => {
  try {
    console.log(event.body);

    const message = `${event.body}`;
    console.log('message');
    console.log(message);

    const response = new KbankRegistrationCallback(message);
    console.log('KbankRegistrationCallback');
    console.log(response);

    await createKBANKDDRegisterResult({
      ...response,
    });

    const callbackRequest: BankServiceResponseWrapper<RegistrationCallbackResponse> =
      {
        status: {
          externalStatusCode: response.returnStatus,
          externalStatusDescription: response.returnMessage,
          internalStatusCode: '200',
          internalStatusDescription: 'Success',
          status: response.returnStatus === KBANK_REGISTRATION_SUCCESS,
        },
        data: {
          registrationRefCode: response.externalReference,
          bankAccountNo: response.accountNo,
        },
      };

    console.log('RegistrationCallbackResponse');
    console.log(callbackRequest);

    const piResponse = await piCallback.postODDRegistrationCallback(
      callbackRequest
    );
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
