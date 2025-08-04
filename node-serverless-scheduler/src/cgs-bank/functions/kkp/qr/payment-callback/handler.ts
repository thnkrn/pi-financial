import {
  formatJSONResponse,
  ValidatedEventAPIGatewayProxyEvent,
} from '@cgs-bank/libs/apiGateway';
import { middyfy } from '@cgs-bank/libs/lambda';
import { QRPaymentCallbackResponse } from '@cgs-bank/models/common/qr-payment';
import { exceptionLogger } from '@cgs-bank/utils/exceptionLogger';
import {
  createKKPQRPaymentResult,
  findKKPGenerateQrResultByTransactionNo,
} from '@cgs-bank/database-services/kkp-db-services';
import { DepositNotification } from '@cgs-bank/models/deposit-noti';
import { sendMessageToSNS } from '@cgs-bank/libs/snsClient';
import { MySqlKKPGenerateQrResult } from '@cgs-bank/models/database/generate-qr-result';
import { TransactionStatus } from '@cgs-bank/external-api-services/kkp-api/response/kkp-qr-payment-inquiry-response';
import piCallback from '@cgs-bank/external-api-services/pi-callback/pi-callback';
import schema from './schema';
import { BankServiceResponseWrapper } from '@cgs-bank/models/common/bank-service-response-wrapper';
import { WalletQrPaymentCallbackRequest } from '@cgs-bank/models/common/qr-payment';
import { getConfigFromSsm } from '@libs/ssm-config';

const handler: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  const [
    kkpBillerId,
    kkpPaymentBillerId,
    kkpDepositTopic,
    kkpSuccessPaymentTopic,
    kkpCallbackViaWalletFlag,
  ] = await getConfigFromSsm('cgs/bank-services/kkp', [
    'biller-id',
    'biller-id-2',
    'deposit/topic',
    'payment/topic',
    'flag/globaleq',
    'flag/should-callback-via-wallet',
  ]);
  let qrResult: MySqlKKPGenerateQrResult;
  let response: QRPaymentCallbackResponse;
  let notiData: DepositNotification;
  try {
    const rawValue = event.body;
    const data =
      typeof rawValue === 'string' || rawValue instanceof String
        ? JSON.parse(rawValue.toString())
        : rawValue;
    console.log(data);

    response = {
      amount: data.body.paymentInfo.paymentAmount,
      paymentTimestamp: data.body.paymentInfo.paymentDate,
      payerAccountNumber: data.body.paymentInfo.accountNo,
      payerBankCode: data.body.paymentInfo.accontBank,
      payerName: data.body.paymentInfo.customerName,
      transactionNo: data.body.referenceInfo.referenceNo1,
      transactionRefCode: data.body.referenceInfo.referenceNo3,
    };

    await createKKPQRPaymentResult({
      AccountBank: data.body.paymentInfo.accontBank,
      AccountNo: data.body.paymentInfo.accountNo,
      BillerId: data.body.paymentInfo.billerID,
      BillerReferenceNo: data.body.referenceInfo.referenceNo1,
      PaymentAmount: data.body.paymentInfo.paymentAmount.toFixed(2),
      PaymentDate: data.body.paymentInfo.paymentDate,
      PaymentType: data.body.paymentInfo.paymentType,
      CustomerName: data.body.paymentInfo.customerName,
      TransactionStatus: TransactionStatus.PAID,
    });

    console.log('QRPaymentCallbackResponse');
    console.log(response);

    qrResult = await findKKPGenerateQrResultByTransactionNo(
      response.transactionNo
    );

    console.log('KKPGenerateQrResult: ', qrResult);
    notiData = {
      isSuccess: true,
      amount: response.amount,
      customerCode: qrResult.customerCode,
      product: qrResult.product,
      transactionNo: response.transactionNo,
      transactionRefCode: response.transactionRefCode,
      paymentDateTime: response.paymentTimestamp,
      payerName: response.payerName,
      payerBankCode: response.payerBankCode,
      payerAccountNo: response.payerAccountNumber,
    };

    const paymentBillerId = data.body.paymentInfo.billerID;
    let topicName: string;
    switch (paymentBillerId) {
      case kkpBillerId:
        if (kkpCallbackViaWalletFlag.toLowerCase() === 'on') {
          topicName = '';
          await sendCallbackViaWallet(notiData, true);
        } else {
          topicName = kkpDepositTopic;
        }
        break;
      case kkpPaymentBillerId:
        topicName = kkpSuccessPaymentTopic;
        break;
      default:
        topicName = '';
        break;
    }

    if (topicName.length > 0) {
      const snsResponse = await sendMessageToSNS({
        data: notiData,
        topicName,
        groupId: qrResult.customerCode,
      });

      console.log('snsResponse: ', snsResponse);
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
    if (response && qrResult && notiData) {
      if (kkpCallbackViaWalletFlag.toLowerCase() === 'on') {
        await sendCallbackViaWallet(notiData, false);
      } else {
        const failNotiData: DepositNotification = {
          ...notiData,
          isSuccess: false,
        };
        const snsResponse = await sendMessageToSNS({
          data: failNotiData,
          topicName: kkpDepositTopic,
          groupId: qrResult.customerCode,
        });

        console.log('snsResponse deposit callback failed: ', snsResponse);
      }
    }
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

async function sendCallbackViaWallet(
  notiData: DepositNotification,
  isSuccess: boolean
) {
  const request: BankServiceResponseWrapper<WalletQrPaymentCallbackRequest> = {
    status: {
      status: isSuccess,
      internalStatusCode: isSuccess ? '200' : '500',
      internalStatusDescription: isSuccess ? 'Success' : 'Failed',
      externalStatusCode: '',
      externalStatusDescription: '',
    },
    data: {
      isSuccess: isSuccess,
      amount: notiData.amount,
      customerCode: notiData.customerCode.slice(0, -1),
      product: notiData.product,
      transactionNo: notiData.transactionNo,
      transactionRefCode: notiData.transactionRefCode,
      paymentDateTime: notiData.paymentDateTime,
      payerName: notiData.payerName,
      payerBankCode: notiData.payerBankCode,
      payerAccountNo: notiData.payerAccountNo,
    },
  };
  console.log('DepositQrPaymentCallbackResponse');
  console.log(request);

  const piResponse = await piCallback.postDepositQrPaymentCallback(request);
  console.log('PiCallback');
  console.log(piResponse);
}

export const main = middyfy(handler);
