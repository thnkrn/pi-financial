import type { ValidatedEventAPIGatewayProxyEvent } from '@cgs-bank/libs/apiGateway';
import { formatJSONResponse } from '@cgs-bank/libs/apiGateway';
import { middyfy } from '@cgs-bank/libs/lambda';
import scbOddApi from '@cgs-bank/external-api-services/scb-api/odd';
import schema from '@cgs-bank/models/common/basic-request';
import * as aes from '@cgs-bank/utils/aes-coder';
import validateJson from '@cgs-bank/utils/jsonValidator';
import { SCBRegistrationInquiryRequest } from '@cgs-bank/external-api-services/scb-api/request/scb-registration-inquiry-request';
import {
  RegistrationCallbackResponse,
  RegistrationInquiryRequest,
  RegistrationInquiryRequestScheme,
} from '@cgs-bank/models/common/registration';
import { BankServiceResponseWrapper } from '@cgs-bank/models/common/bank-service-response-wrapper';
import { exceptionLogger } from '@cgs-bank/utils/exceptionLogger';
import { ssmWrapper } from '@cgs-bank/external-api-services/scb-api/shared-instance';
import { createScbDDRegisterResult } from '@cgs-bank/database-services/scb-db-services';
import { SCBDDRegisterResultInput } from '@cgs-bank/models/database/scb-dd-register-result';
import { REGISTRATION_RESPONSE_SUCCESS_CODE } from '@cgs-bank/external-api-services/scb-api/constant';

const handler: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  try {
    const data = JSON.parse(aes.decrypt(event.body.data));
    validateJson(RegistrationInquiryRequestScheme, data);

    const input = data as RegistrationInquiryRequest;
    console.log('RegistrationInquiryRequest');
    console.log(input);

    const rawData: SCBRegistrationInquiryRequest = {
      regRef: input.registrationRefCode,
      citizenId: input.citizenId,
    };
    console.log('SCBRegistrationInquiryRequest');
    console.log(rawData);

    const publicKey = await ssmWrapper.getValue();
    console.log(publicKey);

    const resultToken = await scbOddApi.getToken();
    const result = await scbOddApi.ddRegistrationInquiry(
      resultToken.accessToken,
      rawData,
      publicKey
    );

    console.log('SCBRegistrationInquiryResponse');
    console.log(result);

    const output: BankServiceResponseWrapper<RegistrationCallbackResponse> = {
      status: {
        status:
          result.data.statusCode.toString() ==
          REGISTRATION_RESPONSE_SUCCESS_CODE,
        externalStatusCode: result.data.statusCode,
        externalStatusDescription: result.data.statusDesc,
        internalStatusCode: '200',
        internalStatusDescription: 'Success',
      },
      data: {
        registrationRefCode: result.data.regRef,
        bankAccountNo: result?.data?.accountNo ?? '',
      },
    };

    const registerResult: SCBDDRegisterResultInput = {
      bankAccountNo: result.data.accountNo,
      registrationRefCode: result.data.regRef,
    };

    await createScbDDRegisterResult(registerResult);

    console.log('RegistrationCallbackResponse');
    console.log(output);

    return formatJSONResponse({
      data: aes.encrypt(JSON.stringify(output)),
    });
  } catch (error) {
    exceptionLogger(error);
  }
};

export const main = middyfy(handler);
