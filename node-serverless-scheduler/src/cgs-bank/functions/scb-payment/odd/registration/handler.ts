import type { ValidatedEventAPIGatewayProxyEvent } from '@cgs-bank/libs/apiGateway';
import { formatJSONResponse } from '@cgs-bank/libs/apiGateway';
import { middyfy } from '@cgs-bank/libs/lambda';
import scbOddApi from '@cgs-bank/external-api-services/scb-api/odd';
import * as aes from '@cgs-bank/utils/aes-coder';
import schema from '@cgs-bank/models/common/basic-request';

import validateJson from '@cgs-bank/utils/jsonValidator';
import { SCBRegistrationRequest } from '@cgs-bank/external-api-services/scb-api/request/scb-registration-request';
import {
  RegistrationRequest,
  RegistrationRequestScheme,
  RegistrationResponse,
} from '@cgs-bank/models/common/registration';
import { BankServiceResponseWrapper } from '@cgs-bank/models/common/bank-service-response-wrapper';
import { createScbDDRegister } from '@cgs-bank/database-services/scb-db-services';
import { exceptionLogger } from '@cgs-bank/utils/exceptionLogger';
import { ssmWrapper } from '@cgs-bank/external-api-services/scb-api/shared-instance';
import { REGISTRATION_REQUEST_SUCCESS_CODE } from '@cgs-bank/external-api-services/scb-api/constant';

const handler: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  try {
    const data = JSON.parse(aes.decrypt(event.body.data));
    validateJson(RegistrationRequestScheme, data);

    const input = data as RegistrationRequest;
    console.log('RegistrationRequest');
    console.log(input);

    const publicKey = await ssmWrapper.getValue();
    console.log(publicKey);

    const rawData: SCBRegistrationRequest = {
      regRef: input.registrationRefCode,
      ref1: input.registrationRefCode,
      ref2: input.citizenId,
      backUrl: data.redirectUrl,
      citizenId: data.citizenId,
      remarks: data.remarks,
    };

    console.log('SCBRegistrationRequest');
    console.log(rawData);

    const resultToken = await scbOddApi.getToken();
    console.log('SCBTokenOutput');
    console.log(resultToken);
    const response = await scbOddApi.ddRegistration(
      resultToken.accessToken,
      rawData,
      publicKey
    );
    console.log('SCBRegistrationResponse');
    console.log(response);

    const status = response.status;
    const output: BankServiceResponseWrapper<RegistrationResponse> = {
      status: {
        status: status.code == REGISTRATION_REQUEST_SUCCESS_CODE,
        internalStatusCode: '200',
        internalStatusDescription: 'Success',
        externalStatusCode: response.status.code,
        externalStatusDescription: response.status.description,
      },
      data: {
        webUrl: response.data.webURL,
      },
    };
    console.log('RegistrationResponse');
    console.log(output);

    await createScbDDRegister({
      merchantId: response.merchantId,
      subAccountId: response.subAccountId,
      ...input,
      ...response.data,
      ...response.status,
    });

    return formatJSONResponse({
      data: aes.encrypt(JSON.stringify(output)),
    });
  } catch (error) {
    exceptionLogger(error);
  }
};

export const main = middyfy(handler);
