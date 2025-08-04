import type { ValidatedEventAPIGatewayProxyEvent } from '@cgs-bank/libs/apiGateway';
import { formatJSONResponse } from '@cgs-bank/libs/apiGateway';
import { middyfy } from '@cgs-bank/libs/lambda';
import scbOddApi from '@cgs-bank/external-api-services/scb-api/odd';
import schema from '@cgs-bank/models/common/basic-request';
import * as aes from '@cgs-bank/utils/aes-coder';
import validateJson from '@cgs-bank/utils/jsonValidator';
import { SCBDDPaymentInquiryRequest } from '@cgs-bank/external-api-services/scb-api/request/scb-dd-payment-inquiry-request';
import { exceptionLogger } from '@cgs-bank/utils/exceptionLogger';
import { ssmWrapper } from '@cgs-bank/external-api-services/scb-api/shared-instance';

const handler: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  try {
    const data = JSON.parse(aes.decrypt(event.body.data));
    validateJson(schema, data);

    console.log(data);

    const publicKey = await ssmWrapper.getValue();
    console.log(publicKey);

    const rawData: SCBDDPaymentInquiryRequest = {
      refNumber: data.TranNo,
      refDateTime: data.TranDateTime,
      ...(Object.prototype.hasOwnProperty.call(data, 'CustomerRef') && {
        customerRef: data.CustomerRef,
      }),
    };
    console.log('SCBDDPaymentInquiryRequest');
    console.log(rawData);

    const resultToken = await scbOddApi.getToken();
    const response = await scbOddApi.ddPaymentInquiry(
      resultToken.accessToken,
      rawData,
      publicKey
    );

    console.log('SCBDDPaymentInquiryResponse');
    console.log(response);

    return formatJSONResponse({
      message: 'Success',
    });
  } catch (error) {
    exceptionLogger(error);
  }
};

export const main = middyfy(handler);
