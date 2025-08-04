import {
  KKPRequestWrapper,
  KKPResponseWrapper,
} from '@cgs-bank/external-api-services/kkp-api/common';
import { KKPQrPaymentInquiryRequest } from '@cgs-bank/external-api-services/kkp-api/form/payment-inquiry-request';
import { KKPQRRequest } from '@cgs-bank/external-api-services/kkp-api/form/qr-request';
import { KKPQrPaymentInquiryResponse } from '@cgs-bank/external-api-services/kkp-api/response/kkp-qr-payment-inquiry-response';
import { KKPQRResponse } from '@cgs-bank/external-api-services/kkp-api/response/kkp-qr-response';
import axios from 'axios';
import { getConfigFromSsm } from '@libs/ssm-config';

const partnerName = process.env.KKP_PARTNER_NAME;

const functions = {
  getThaiQRPayment: async (
    accessToken,
    input: KKPRequestWrapper<KKPQRRequest>
  ): Promise<KKPResponseWrapper<KKPQRResponse>> => {
    console.log(accessToken);
    const [baseUrl, consumerKey] = await getConfigFromSsm(
      'cgs/bank-services/kkp',
      ['base-api', 'consumer-key']
    );
    const data = {
      Header: {
        TransactionID: input.Header.TransactionID,
        TransactionDateTime: input.Header.TransactionDateTime,
        ServiceName: 'GenThaiQRPayment',
        SystemCode: 'API',
        ChannelCode: 'API',
      },
      Data: {
        QRType: 'BillPayment',
        BillPayment: {
          BillerID: input.Data.billPaymentBillerId,
          TaxID: input.Data.billPaymentTaxId,
          Suffix: input.Data.billPaymentSuffix,
          Reference1: input.Data.billPaymentReference1,
          Reference2: input.Data.billPaymentReference2,
          Reference3: input.Data.billPaymentReference3,
          TransactionAmount: input.Data.transactionAmount,
        },
      },
    };

    console.log(data);

    const result = await axios.post(
      `${baseUrl}/microgw-secure/payment/v2/${partnerName}/genthaiqrpayment`,
      data,
      {
        headers: {
          'content-type': 'application/json',
          ConsumerKey: consumerKey,
          Authorization: `Bearer ${accessToken}`,
        },
      }
    );
    console.log(result);
    const content = result.data as KKPResponseWrapper<KKPQRResponse>;
    if (content.ResponseStatus.ResponseCode != 'BCA-I-0000') {
      throw 'Invalid Request';
    }
    return content;
  },

  getQrPaymentInquiryResult: async (
    accessToken: string,
    input: KKPRequestWrapper<KKPQrPaymentInquiryRequest>
  ): Promise<KKPResponseWrapper<KKPQrPaymentInquiryResponse>> => {
    const [baseUrl, consumerKey] = await getConfigFromSsm(
      'cgs/bank-services/kkp',
      ['base-api', 'consumer-key']
    );

    const data = {
      Header: {
        TransactionID: input.Header.TransactionID,
        TransactionDateTime: input.Header.TransactionDateTime,
        ServiceName: 'InquiryPaymentResult',
        SystemCode: 'API',
        ChannelCode: 'API',
      },
      Data: {
        BillerID: input.Data.billerId,
        BillReference1: input.Data.billReference1,
        BillReference2: input.Data.billReference2,
      },
    };

    const result = await axios.post(
      `${baseUrl}/microgw-secure/payment/v2/${partnerName}/inquirypaymentresult`,
      data,
      {
        headers: {
          'content-type': 'application/json',
          ConsumerKey: consumerKey,
          Authorization: `Bearer ${accessToken}`,
        },
      }
    );

    const content =
      result.data as KKPResponseWrapper<KKPQrPaymentInquiryResponse>;
    console.log('KKPResponseWrapper<KKPQrPaymentInquiryResponse>: ', content);

    if (content.ResponseStatus.ResponseCode != 'BCA-I-0000') {
      throw Error(content.ResponseStatus.ResponseCode);
    }

    return content;
  },
};

export default functions;
