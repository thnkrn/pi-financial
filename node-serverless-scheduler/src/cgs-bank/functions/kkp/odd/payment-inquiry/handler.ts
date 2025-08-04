import {
  formatJSONResponse,
  ValidatedEventAPIGatewayProxyEvent,
} from '@cgs-bank/libs/apiGateway';
import * as aes from '@cgs-bank/utils/aes-coder';
import validateJson from '@cgs-bank/utils/jsonValidator';
import schema from '@cgs-bank/models/common/basic-request';
import kkpApiTokenRetriever from '@cgs-bank/utils/kkp-api-token-retriever';
import kkpOddApi from '@cgs-bank/external-api-services/kkp-api/odd';
import { getTransactionId } from '@cgs-bank/utils/kkp-transaction-generator';
import {
  getTimestamp,
  nowUTC,
  TimestampFormat,
} from '@cgs-bank/utils/timestamp';
import { middyfy } from '@cgs-bank/libs/lambda';
import { createKKPDDPaymentInquiryResult } from '@cgs-bank/database-services/kkp-db-services';
import { KKPLookupConfirmRequest } from '@cgs-bank/external-api-services/kkp-api/form/kkp-lookup-confirm';
import { exceptionLogger } from '@cgs-bank/utils/exceptionLogger';
import {
  DDPaymentInquiry,
  DDPaymentInquiryRequestScheme,
  DDPaymentResponse,
} from '@cgs-bank/models/common/dd-payment';
import { BankServiceResponseWrapper } from '@cgs-bank/models/common/bank-service-response-wrapper';

const handler: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  try {
    const data = JSON.parse(aes.decrypt(event.body.data));
    validateJson(DDPaymentInquiryRequestScheme, data);

    const input = data as DDPaymentInquiry;
    console.log('input');
    console.log(input);

    const now = nowUTC();
    const resToken = await kkpApiTokenRetriever.retrieveApiKey();
    const request: KKPLookupConfirmRequest = {
      txnReferenceNo: input.externalRefCode,
    };

    const response = await kkpOddApi.getInquiryResult(resToken, {
      Header: {
        TransactionID: getTransactionId(now),
        TransactionDateTime: getTimestamp(TimestampFormat.Normal, now),
      },
      Data: request,
    });

    console.log('KKPOddPaymentInquiryResponse');
    console.log(response);

    await createKKPDDPaymentInquiryResult({
      ...input,
      ...request,
      ...response.Header,
      ...response.Data,
      ...response.ResponseStatus,
      ...response.fault,
    });

    const output: BankServiceResponseWrapper<DDPaymentResponse> = {
      status: {
        externalStatusCode: response.ResponseStatus.ResponseCode,
        externalStatusDescription: response.ResponseStatus.ResponseMessage,
        internalStatusCode: '200',
        internalStatusDescription: 'Success',
        status: response.ResponseStatus.ResponseCode == 'ATS-I-1000',
      },
      data: {
        amount: input.amount,
        externalRefCode: input.externalRefCode,
        externalRefTime: input.externalRefTime,
        transactionNo: input.transactionNo,
        transactionRefCode: '',
      },
    };

    console.log('DDPaymentResponse');
    console.log(output);

    return formatJSONResponse({
      data: aes.encrypt(JSON.stringify(output)),
    });
  } catch (error) {
    exceptionLogger(error);
    return formatJSONResponse({
      error: error,
    });
  }
};

export const main = middyfy(handler);
