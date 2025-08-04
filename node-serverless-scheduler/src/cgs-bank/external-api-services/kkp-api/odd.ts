import { KKPLookupRequest } from '@cgs-bank/external-api-services/kkp-api/form/kkp-lookup-request';
import axios from 'axios';
import { KKPLookupConfirmRequest } from '@cgs-bank/external-api-services/kkp-api/form/kkp-lookup-confirm';
import {
  KKPRequestWrapper,
  KKPResponseWrapper,
} from '@cgs-bank/external-api-services/kkp-api/common';
import { KKPLookupResponse } from '@cgs-bank/external-api-services/kkp-api/response/kkp-lookup-response';
import { KKPConfirmLookupResponse } from '@cgs-bank/external-api-services/kkp-api/response/kkp-confirm-lookup-response';
import { KKPOddPaymentInquiryResponse } from '@cgs-bank/external-api-services/kkp-api/response/kkp-odd-payment-inquiry-response';
import { getConfigFromSsm } from '@libs/ssm-config';

const partnerName = process.env.KKP_PARTNER_NAME;

const functions = {
  lookupRequest: async (
    accessToken: string,
    input: KKPRequestWrapper<KKPLookupRequest>
  ): Promise<KKPResponseWrapper<KKPLookupResponse>> => {
    const [baseUrl, consumerKey, cgsBankAccount] = await getConfigFromSsm(
      'cgs/bank-services/kkp',
      ['base-api', 'consumer-key', 'cgs-bank-account']
    );
    console.log(baseUrl);
    console.log(consumerKey);
    console.log(cgsBankAccount);

    const result = await axios.post(
      `${baseUrl}/microgw-secure/transfer/v2/${partnerName}/lookup`,
      {
        Header: {
          TransactionID: input.Header.TransactionID,
          TransactionDateTime: input.Header.TransactionDateTime,
          ServiceName: 'Lookup',
          SystemCode: 'API',
          ChannelCode: 'API',
        },
        Data: {
          EffectiveDate: input.Data.effectiveDate,
          TransferAmount: input.Data.transferAmount,
          SendingAccountNo: cgsBankAccount,
          ScanFlag: 'N',
          ReceivingType: 'BANKAC',
          ReceivingID: input.Data.receivingAccountNo,
          ReceivingBankCode: input.Data.receivingBankCode,
          Currency: 'THB',
        },
      },
      {
        headers: {
          'content-type': 'application/json',
          Authorization: `Bearer ${accessToken}`,
          ConsumerKey: consumerKey,
        },
      }
    );

    console.log(result);
    const content = result.data as KKPResponseWrapper<KKPLookupResponse>;

    if (content.ResponseStatus.ResponseCode != 'ATS-I-1000') {
      throw 'Invalid Request';
    }

    return content;
  },

  confirmLookupRequest: async (
    accessToken: string,
    input: KKPRequestWrapper<KKPLookupConfirmRequest>
  ): Promise<KKPResponseWrapper<KKPConfirmLookupResponse>> => {
    const [baseUrl, consumerKey, cgsBankAccount] = await getConfigFromSsm(
      'cgs/bank-services/kkp',
      ['base-api', 'consumer-key', 'cgs-bank-account']
    );
    const result = await axios.post(
      `${baseUrl}/microgw-secure/transfer/v2/${partnerName}/confirm`,
      {
        Header: {
          TransactionID: input.Header.TransactionID,
          TransactionDateTime: input.Header.TransactionDateTime,
          ServiceName: 'Confirm',
          SystemCode: 'API',
          ChannelCode: 'API',
        },
        Data: {
          TxnReferenceNo: input.Data.txnReferenceNo,
          SendingAccountNo: cgsBankAccount,
        },
      },
      {
        headers: {
          'content-type': 'application/json',
          Authorization: `Bearer ${accessToken}`,
          ConsumerKey: consumerKey,
        },
      }
    );

    console.log(result);
    const content = result.data as KKPResponseWrapper<KKPConfirmLookupResponse>;

    if (content.ResponseStatus.ResponseCode != 'ATS-I-1000') {
      throw 'Invalid Request';
    }

    return content;
  },

  getInquiryResult: async (
    accessToken: string,
    input: KKPRequestWrapper<KKPLookupConfirmRequest>
  ): Promise<KKPResponseWrapper<KKPOddPaymentInquiryResponse>> => {
    const [baseUrl, consumerKey, cgsBankAccount] = await getConfigFromSsm(
      'cgs/bank-services/kkp',
      ['base-api', 'consumer-key', 'cgs-bank-account']
    );
    const result = await axios.post(
      `${baseUrl}/microgw-secure/transfer/v2/${partnerName}/inquiryresult`,
      {
        Header: {
          TransactionID: input.Header.TransactionID,
          TransactionDateTime: input.Header.TransactionDateTime,
          ServiceName: 'InquiryResult',
          SystemCode: 'API',
          ChannelCode: 'API',
        },
        Data: {
          TxnReferenceNo: input.Data.txnReferenceNo,
          SendingAccountNo: cgsBankAccount,
        },
      },
      {
        headers: {
          'content-type': 'application/json',
          Authorization: `Bearer ${accessToken}`,
          ConsumerKey: consumerKey,
        },
      }
    );

    const content =
      result.data as KKPResponseWrapper<KKPOddPaymentInquiryResponse>;

    if (content.ResponseStatus.ResponseCode != 'ATS-I-1000') {
      throw {
        ResponseStatus: content.ResponseStatus,
      };
    }

    return content;
  },
};

export default functions;
