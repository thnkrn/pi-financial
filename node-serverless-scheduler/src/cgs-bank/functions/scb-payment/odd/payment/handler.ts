import type { ValidatedEventAPIGatewayProxyEvent } from '@cgs-bank/libs/apiGateway';
import { formatJSONResponse } from '@cgs-bank/libs/apiGateway';
import { middyfy } from '@cgs-bank/libs/lambda';
import scbOddApi from '@cgs-bank/external-api-services/scb-api/odd';
import schema from '@cgs-bank/models/common/basic-request';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import * as aes from '@cgs-bank/utils/aes-coder';
import validateJson from '@cgs-bank/utils/jsonValidator';
import { SCBDDPaymentRequest } from '@cgs-bank/external-api-services/scb-api/request/scb-dd-payment-request';
import { exceptionLogger } from '@cgs-bank/utils/exceptionLogger';
import { ssmWrapper } from '@cgs-bank/external-api-services/scb-api/shared-instance';

const handler: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  try {
    const data = JSON.parse(aes.decrypt(event.body.data));
    validateJson(schema, data);
    dayjs.extend(timezone);

    const publicKey = await ssmWrapper.getValue();
    console.log(publicKey);

    const rawData: SCBDDPaymentRequest = {
      refNumber: data.TranNo,
      refDateTime: dayjs.tz('Asia/Bangkok').format('yyyyMMDDHHmmss'),
      amount: data.Amount,
      currency: 'THB',
      accountNumber: data.AcountNumber,
      customerRef: data.CustomerRef,
    };

    const resultToken = await scbOddApi.getToken();
    const response = await scbOddApi.ddPay(
      resultToken.accessToken,
      rawData,
      publicKey
    );
    console.log(response);
    return formatJSONResponse({
      message: 'Success',
    });
  } catch (error) {
    exceptionLogger(error);
  }
};

export const main = middyfy(handler);
