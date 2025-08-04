import {
  formatJSONResponse,
  ValidatedEventAPIGatewayProxyEvent,
} from '@cgs-bank/libs/apiGateway';
import { middyfy } from '@cgs-bank/libs/lambda';
import { exceptionLogger } from '@cgs-bank/utils/exceptionLogger';
import * as console from 'console';
import schema from './schema';
import { getConfigFromSsm } from '@libs/ssm-config';
import { parseDate } from '@libs/date-utils';
import {
  KkpBillPaymentCallback,
  WalletBillPaymentRequest,
} from '@cgs-bank/models/common/bill-payment';
import piCallback from '@cgs-bank/external-api-services/pi-callback/pi-callback';
import { createKkpBillPaymentResult } from '@cgs-bank/database-services/kkp-db-services';

const handler: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  try {
    console.log('Event');
    console.log(event);
    const rawValue = event.body;
    console.log('Raw value');
    console.log(rawValue);
    const data: KkpBillPaymentCallback =
      typeof rawValue === 'string' || rawValue instanceof String
        ? JSON.parse(rawValue.toString())
        : rawValue;
    console.log('Data');
    console.log(data);

    // save to database
    console.log('Insert data to database');
    await createKkpBillPaymentResult({
      TransactionId: data.Header.TransactionID,
      AccountBank: data.body.paymentInfo.accontBank,
      AccountNo: data.body.paymentInfo.accountNo,
      BillerId: data.body.paymentInfo.billerID,
      ChannelId: data.Header.ChannelID,
      ReferenceNo1: data.body.referenceInfo.referenceNo1,
      ReferenceNo2: data.body.referenceInfo.referenceNo2,
      PaymentAmount: data.body.paymentInfo.paymentAmount,
      PaymentDate: data.body.paymentInfo.paymentDate,
      PaymentType: data.body.paymentInfo.paymentType,
      CustomerName: data.body.paymentInfo.customerName,
    });

    const [crossBankBillerId] = await getConfigFromSsm(
      'cgs/bank-services/kkp',
      ['cross-bank-biller-id']
    );

    if (data.body.paymentInfo.billerID == crossBankBillerId) {
      console.log('Send bill payment request to wallet');
      const request = buildDepositBillPaymentRequest(data);
      await sendBillPaymentRequestToWallet(request);
    }

    return formatJSONResponse({
      Header: data.Header,
      ResponseStatus: {
        ResponseCode: 'BGW-I-0000',
        ResponseMessage: 'Biller acknowledged',
        OriginalResponseCode: null,
        OriginalResponseMessage: null,
      },
    });
  } catch (error) {
    exceptionLogger(error);
    return formatJSONResponse({
      ResponseStatus: {
        ResponseCode: 'BGW-I-0000',
        ResponseMessage: 'Biller acknowledged',
        OriginalResponseCode: null,
        OriginalResponseMessage: null,
      },
    });
  }
};

function buildDepositBillPaymentRequest(
  data: KkpBillPaymentCallback
): WalletBillPaymentRequest {
  const paymentReceivedDate = parseDate(
    data.body.paymentInfo.paymentDate,
    'YYYYMMDDHHmmss',
    true
  ).toISOString();
  return {
    bankTransactionReference: data.Header.TransactionID ?? '',
    requestAmount: data.body.paymentInfo.paymentAmount,
    reference1: data.body.referenceInfo.referenceNo1 ?? '',
    reference2: data.body.referenceInfo.referenceNo2 ?? '',
    customerPaymentName: data.body.paymentInfo.customerName ?? '',
    customerPaymentBankCode: data.body.paymentInfo.accountBank ?? '',
    customerPaymentBankAccountNo: data.body.paymentInfo.accountNo ?? '',
    paymentChannel: data.Header.ChannelID ?? '',
    paymentReceivedDate: paymentReceivedDate,
  };
}

async function sendBillPaymentRequestToWallet(
  request: WalletBillPaymentRequest
) {
  console.log('Deposit Wallet Bill Payment Request');
  console.log(request);
  const piResponse = await piCallback.postDepositBillPayment(request);
  console.log('PiCallback');
  console.log(piResponse);
}

export const main = middyfy(handler);
