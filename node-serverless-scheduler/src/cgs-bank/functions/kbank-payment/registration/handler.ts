import type { ValidatedEventAPIGatewayProxyEvent } from '@cgs-bank/libs/apiGateway';
import { formatJSONResponse } from '@cgs-bank/libs/apiGateway';
import { middyfy } from '@cgs-bank/libs/lambda';
import kBankApi, {
  KBANKRegistrationRequest,
} from '@cgs-bank/external-api-services/kbank-api/registration';
import * as aes from '@cgs-bank/utils/aes-coder';
import schema from '@cgs-bank/models/common/basic-request';
import validateJson from '@cgs-bank/utils/jsonValidator';
import {
  RegistrationRequest,
  RegistrationRequestScheme,
  RegistrationResponse,
} from '@cgs-bank/models/common/registration';
import { exceptionLogger } from '@cgs-bank/utils/exceptionLogger';
import { BankServiceResponseWrapper } from '@cgs-bank/models/common/bank-service-response-wrapper';
import { createKBANKDDRegister } from '@cgs-bank/database-services/kbank-db-services';
import {
  emailFormatValidator,
  thaiPhoneNumberValidator,
} from '@cgs-bank/utils/validator';
import { KBANK_REGISTRATION_SUCCESS } from '@cgs-bank/external-api-services/kbank-api/constant';
import { getConfigFromSsm } from '@libs/ssm-config';

const handler: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  try {
    const data = JSON.parse(aes.decrypt(event.body.data));
    validateJson(RegistrationRequestScheme, data);

    const input = data as RegistrationRequest;
    console.log('RegistrationRequest');
    console.log(input);

    if (input.email) {
      emailFormatValidator(input.email);
    }

    if (input.mobileNo) {
      thaiPhoneNumberValidator(input.mobileNo);
    }

    const rawData: KBANKRegistrationRequest = {
      citizenId: input.citizenId,
      externalReference: input.registrationRefCode,
      userEmail: input.email,
      userMobileNo: input.mobileNo,
    };

    console.log('KBANKRegistrationTokenInput');
    console.log(rawData);

    const response = await kBankApi.registrationToken(rawData);
    console.log('KBANKRegistrationResponse');
    console.log(response);

    const [kbankRedirectDomain] = await getConfigFromSsm(
      'cgs/bank-services/kbank',
      ['redirect-domain']
    );
    const redirectURL = `${kbankRedirectDomain}?reg_id=${response.reg_id}&langLocale=TH`;

    await createKBANKDDRegister({
      ...input,
      regId: response.reg_id,
      returnCode: response.return_code,
      returnMessage: response.return_message,
      returnStatus: response.return_status,
    });

    console.log(redirectURL);

    const output: BankServiceResponseWrapper<RegistrationResponse> = {
      status: {
        externalStatusCode: response.return_code,
        externalStatusDescription: response.return_message,
        internalStatusCode: '200',
        internalStatusDescription: 'Success',
        status: response.return_status == KBANK_REGISTRATION_SUCCESS,
      },
      data: {
        webUrl: redirectURL,
      },
    };

    console.log('RegistrationResponse');
    console.log(output);

    return formatJSONResponse({
      data: aes.encrypt(JSON.stringify(output)),
    });
  } catch (error) {
    exceptionLogger(error);
  }
};

export const main = middyfy(handler);
