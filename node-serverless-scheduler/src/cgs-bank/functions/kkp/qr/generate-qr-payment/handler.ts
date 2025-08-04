import type { ValidatedEventAPIGatewayProxyEvent } from '@cgs-bank/libs/apiGateway';
import { formatJSONResponse } from '@cgs-bank/libs/apiGateway';
import { middyfy } from '@cgs-bank/libs/lambda';
import kkpApiTokenRetriever from '@cgs-bank/utils/kkp-api-token-retriever';
import qrApi from '@cgs-bank/external-api-services/kkp-api/qr';
import schema from '@cgs-bank/models/common/basic-request';
import {
  generateJulianDateFormatWithExpiredTime,
  getTimestamp,
  nowBangkok,
  nowUTC,
  TimestampFormat,
} from '@cgs-bank/utils/timestamp';
import { createKKPQRPayment } from '@cgs-bank/database-services/kkp-db-services';
import { KKPQRRequest } from '@cgs-bank/external-api-services/kkp-api/form/qr-request';
import { KKPRequestWrapper } from '@cgs-bank/external-api-services/kkp-api/common';
import { getTransactionId } from '@cgs-bank/utils/kkp-transaction-generator';
import * as aes from '@cgs-bank/utils/aes-coder';
import validateJson from '@cgs-bank/utils/jsonValidator';
import {
  QRPaymentRequestExtended,
  QRPaymentRequestScheme,
  QRPaymentResponse,
  QRPropose,
} from '@cgs-bank/models/common/qr-payment';
import { exceptionLogger } from '@cgs-bank/utils/exceptionLogger';
import { BankServiceResponseWrapper } from '@cgs-bank/models/common/bank-service-response-wrapper';
import { getConfigFromSsm } from '@libs/ssm-config';

const handler: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  try {
    const data = JSON.parse(aes.decrypt(event.body.data));
    validateJson(QRPaymentRequestScheme, data);

    const input = data as QRPaymentRequestExtended;
    console.log(`1. QRPaymentRequest - ${input.transactionNo}`);
    console.log(input);

    const now = nowUTC();
    const nowWithTimeZone = nowBangkok();
    let expiredTime: string;
    if (data.expiredTimeInMinute != null) {
      expiredTime = generateJulianDateFormatWithExpiredTime(
        nowWithTimeZone.add(data.expiredTimeInMinute, 'minute')
      );
    } else {
      expiredTime = generateJulianDateFormatWithExpiredTime(
        nowWithTimeZone.add(1, 'day').set('hour', 0).set('minute', 0)
      );
    }

    let propose: QRPropose;
    if (data.propose != null) {
      propose = QRPropose[data.propose] ?? QRPropose.DEPOSIT;
    } else {
      propose = QRPropose.DEPOSIT;
    }

    const [kkpBillerId, kkpPaymentBillerId] = await getConfigFromSsm(
      'cgs/bank-services/kkp',
      ['biller-id', 'biller-id-2']
    );
    const billerId =
      propose === QRPropose.DEPOSIT ? kkpBillerId : kkpPaymentBillerId;
    const taxId = billerId.substr(0, 13);
    const suffix = billerId.substr(billerId.length - 2, 2);

    const request: KKPRequestWrapper<KKPQRRequest> = {
      Header: {
        TransactionID: getTransactionId(now),
        TransactionDateTime: getTimestamp(TimestampFormat.Normal, now),
      },
      Data: {
        billPaymentBillerId: billerId,
        billPaymentSuffix: suffix,
        billPaymentTaxId: taxId,
        billPaymentReference1: input.transactionNo,
        billPaymentReference2: expiredTime,
        billPaymentReference3: input.transactionRefCode,
        transactionAmount: input.amount.toFixed(2),
      },
    };

    console.log(`2. KKPQRRequest - ${input.transactionNo}`);
    console.log(request);

    const resToken = await kkpApiTokenRetriever.retrieveApiKey();
    const response = await qrApi.getThaiQRPayment(resToken, request);
    console.log(`3. KKPQRResponse - ${input.transactionNo}`);
    console.log(response);

    await createKKPQRPayment({
      ...input,
      ...input.internalRef,
      ...request.Data,
      ...response.Header,
      ...response.Data,
      ...response.ResponseStatus,
      ...response.fault,
    });

    const output: BankServiceResponseWrapper<QRPaymentResponse> = {
      status: {
        externalStatusCode: response.ResponseStatus.ResponseCode,
        externalStatusDescription: response.ResponseStatus.ResponseMessage,
        internalStatusCode: '200',
        internalStatusDescription: 'Success',
        status: true,
      },
      data: {
        QRValue: response.Data.QRValue,
      },
    };
    console.log(`4. QRPaymentResponse - ${input.transactionNo}`);
    console.log(output);

    return formatJSONResponse({
      data: aes.encrypt(JSON.stringify(output)),
    });
  } catch (error) {
    exceptionLogger(error);
  }
};

export const main = middyfy(handler);
