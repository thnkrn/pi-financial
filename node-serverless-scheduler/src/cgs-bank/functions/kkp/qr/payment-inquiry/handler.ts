import { findKKPGenerateQrResultByTransactionNo } from '@cgs-bank/database-services/kkp-db-services';
import {
  KKPRequestWrapper,
  KKPResponseWrapper,
} from '@cgs-bank/external-api-services/kkp-api/common';
import { KKPQrPaymentInquiryRequest } from '@cgs-bank/external-api-services/kkp-api/form/payment-inquiry-request';
import qrApi from '@cgs-bank/external-api-services/kkp-api/qr';
import { KKPQrPaymentInquiryResponse } from '@cgs-bank/external-api-services/kkp-api/response/kkp-qr-payment-inquiry-response';
import type { ValidatedEventAPIGatewayProxyEvent } from '@cgs-bank/libs/apiGateway';
import { formatJSONResponse } from '@cgs-bank/libs/apiGateway';
import { middyfy } from '@cgs-bank/libs/lambda';
import { BankServiceResponseWrapper } from '@cgs-bank/models/common/bank-service-response-wrapper';
import schema from '@cgs-bank/models/common/basic-request';
import {
  QRPaymentInquiry,
  QRPaymentInquiryResponse,
  QRQRPaymentInquiryScheme,
} from '@cgs-bank/models/common/qr-payment';
import * as aes from '@cgs-bank/utils/aes-coder';
import { exceptionLogger } from '@cgs-bank/utils/exceptionLogger';
import validateJson from '@cgs-bank/utils/jsonValidator';
import kkpApiTokenRetriever from '@cgs-bank/utils/kkp-api-token-retriever';
import { getTransactionId } from '@cgs-bank/utils/kkp-transaction-generator';
import {
  getTimestamp,
  nowUTC,
  TimestampFormat,
} from '@cgs-bank/utils/timestamp';
import { getConfigFromSsm } from '@libs/ssm-config';

export const qrInquiryProcess = async (
  input: QRPaymentInquiry,
  billerId: string
): Promise<KKPResponseWrapper<KKPQrPaymentInquiryResponse>> => {
  const query = await findKKPGenerateQrResultByTransactionNo(
    input.transactionNo
  );
  const billRef2 = query.billPaymentReference2;

  const now = nowUTC();
  const request: KKPRequestWrapper<KKPQrPaymentInquiryRequest> = {
    Header: {
      TransactionID: getTransactionId(now),
      TransactionDateTime: getTimestamp(TimestampFormat.Normal, now),
    },
    Data: {
      billerId: billerId,
      billReference1: input.transactionNo,
      billReference2: billRef2,
    },
  };
  console.log('KKPQrPaymentInquiryRequest: ', request);

  const resToken = await kkpApiTokenRetriever.retrieveApiKey();
  const response = await qrApi.getQrPaymentInquiryResult(resToken, request);

  console.log('KKPQrPaymentInquiryResponse: ', response);

  if (response.Data.ResultList.length == 0) {
    throw Error('Inquiry result list is empty');
  }

  return response;
};

const handler: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  try {
    const data = JSON.parse(aes.decrypt(event.body.data));
    validateJson(QRQRPaymentInquiryScheme, data);

    const input = data as QRPaymentInquiry;
    console.log('QRPaymentInquiry Input: ', input);

    const [kkpBillerId] = await getConfigFromSsm('cgs/bank-services/kkp', [
      'biller-id',
    ]);
    const response = await qrInquiryProcess(input, kkpBillerId);

    const inquiryResponseItem = response.Data.ResultList[0];
    console.log('inquiryResponseItem: ', inquiryResponseItem);

    const output: BankServiceResponseWrapper<QRPaymentInquiryResponse> = {
      status: {
        externalStatusCode: response.ResponseStatus.ResponseCode,
        externalStatusDescription: response.ResponseStatus.ResponseMessage,
        internalStatusCode: '200',
        internalStatusDescription: 'Success',
        status: true,
      },
      data: {
        transactionNo: inquiryResponseItem.BillerReferenceNo,
        paymentAmount: inquiryResponseItem.PaymentAmount.toString(),
        paymentDateTime: inquiryResponseItem.PaymentDate,
        bankCode: inquiryResponseItem.BankCode,
        accountNo: inquiryResponseItem.AccountNo,
        customerName: inquiryResponseItem.CustomerName,
      },
    };

    console.log('QRPaymentInquiryResponse Output: ', output);

    return formatJSONResponse({
      data: aes.encrypt(JSON.stringify(output)),
    });
  } catch (error) {
    exceptionLogger(error);
    throw error;
  }
};

export const main = middyfy(handler);
