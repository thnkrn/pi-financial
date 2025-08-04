import {
  formatJSONResponse,
  ValidatedEventAPIGatewayProxyEvent,
} from '@cgs-bank/libs/apiGateway';
import * as aes from '@cgs-bank/utils/aes-coder';
import validateJson from '@cgs-bank/utils/jsonValidator';
import schema from '@cgs-bank/models/common/basic-request';
import { middyfy } from '@libs/lambda';
import kkpApiTokenRetriever from '@cgs-bank/utils/kkp-api-token-retriever';
import kkpOddApi from '@cgs-bank/external-api-services/kkp-api/odd';
import { getTransactionId } from '@cgs-bank/utils/kkp-transaction-generator';
import {
  getTimestamp,
  nowBangkok,
  TimestampFormat,
} from '@cgs-bank/utils/timestamp';
import { createKKPConfirmPaymentResult } from '@cgs-bank/database-services/kkp-db-services';
import { KKPLookupRequest } from '@cgs-bank/external-api-services/kkp-api/form/kkp-lookup-request';
import { exceptionLogger } from '@cgs-bank/utils/exceptionLogger';
import {
  DDPaymentRequest,
  DDPaymentRequestScheme,
  DDPaymentResponse,
} from '@cgs-bank/models/common/dd-payment';
import { KKPLookupConfirmRequest } from '@cgs-bank/external-api-services/kkp-api/form/kkp-lookup-confirm';
import { BankServiceResponseWrapper } from '@cgs-bank/models/common/bank-service-response-wrapper';

const handler: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  let input: DDPaymentRequest;
  try {
    const data = JSON.parse(aes.decrypt(event.body.data));
    validateJson(DDPaymentRequestScheme, data);

    input = data as DDPaymentRequest;
    console.log(`1. DDPaymentRequest - ${input.transactionNo}`);
    console.log(input);

    const lookupTime = nowBangkok();
    const requestLookup: KKPLookupRequest = {
      effectiveDate: getTimestamp(TimestampFormat.OnlyDate, lookupTime),
      receivingAccountNo: input.accountNo,
      receivingBankCode: input.destinationBankCode,
      transferAmount: input.amount,
    };

    console.log(`2. KKPLookupRequest - ${input.transactionNo}`);
    console.log(requestLookup);

    const resToken = await kkpApiTokenRetriever.retrieveApiKey();
    const lookupResponse = await kkpOddApi.lookupRequest(resToken, {
      Header: {
        TransactionID: getTransactionId(lookupTime, input.transactionNo),
        TransactionDateTime: getTimestamp(TimestampFormat.Normal, lookupTime),
      },
      Data: requestLookup,
    });

    console.log(`3. LookupResponse - ${input.transactionNo}`);
    console.log(lookupResponse);

    const confirmTime = nowBangkok();

    const requestLookupConfirm: KKPLookupConfirmRequest = {
      txnReferenceNo: lookupResponse.Data.TxnReferenceNo,
    };

    console.log(`4. KKPLookupConfirmRequest - ${input.transactionNo}`);
    console.log(requestLookupConfirm);

    const confirmResponse = await kkpOddApi.confirmLookupRequest(resToken, {
      Header: {
        TransactionID: getTransactionId(confirmTime, input.transactionNo),
        TransactionDateTime: getTimestamp(TimestampFormat.Normal, confirmTime),
      },
      Data: requestLookupConfirm,
    });

    console.log(`5. ConfirmResponse - ${input.transactionNo}`);
    console.log(confirmResponse);

    await createKKPConfirmPaymentResult({
      ...input,
      ...input.internalRef,
      ...requestLookup,
      ...requestLookupConfirm,
      ...confirmResponse.Header,
      ...confirmResponse.Data,
      ...confirmResponse.ResponseStatus,
      ...confirmResponse.fault,
    });

    const output: BankServiceResponseWrapper<DDPaymentResponse> = {
      status: {
        externalStatusCode: confirmResponse.ResponseStatus.ResponseCode,
        externalStatusDescription:
          confirmResponse.ResponseStatus.ResponseMessage,
        internalStatusCode: '200',
        internalStatusDescription: 'Success',
        status: confirmResponse.ResponseStatus.ResponseCode == 'ATS-I-1000',
      },
      data: {
        amount: input.amount,
        externalRefCode: lookupResponse.Data.TxnReferenceNo,
        externalRefTime: confirmResponse.Header.TransactionDateTime,
        transactionNo: input.transactionNo,
        transactionRefCode: input.transactionRefCode,
      },
    };

    console.log(`6. DDPaymentResponse - ${input.transactionNo}`);
    console.log(output);

    return formatJSONResponse({
      data: aes.encrypt(JSON.stringify(output)),
    });
  } catch (error) {
    exceptionLogger(error);
  }
};

export const main = middyfy(handler);
