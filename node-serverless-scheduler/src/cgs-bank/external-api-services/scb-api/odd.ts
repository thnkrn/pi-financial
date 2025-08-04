import axios from 'axios';
import * as uuid from 'uuid';
import {
  decrypt,
  encrypt,
} from '@cgs-bank/external-api-services/scb-api/scb-rsa';
import { SCBRegistrationRequest } from '@cgs-bank/external-api-services/scb-api/request/scb-registration-request';
import { SCBRegistrationResponse } from '@cgs-bank/external-api-services/scb-api/response/scb-registration-response';
import { SCBRegistrationInquiryRequest } from '@cgs-bank/external-api-services/scb-api/request/scb-registration-inquiry-request';
import { SCBRegistrationInquiryResponse } from '@cgs-bank/external-api-services/scb-api/response/scb-registration-inquiry-response';
import { SCBDDPaymentRequest } from '@cgs-bank/external-api-services/scb-api/request/scb-dd-payment-request';
import { SCBDDPaymentResponse } from '@cgs-bank/external-api-services/scb-api/response/scb-dd-payment-response';
import { SCBDDPaymentInquiryRequest } from '@cgs-bank/external-api-services/scb-api/request/scb-dd-payment-inquiry-request';
import { SCBDDPaymentInquiryResponse } from '@cgs-bank/external-api-services/scb-api/response/scb-dd-payment-inquiry-response';
import {
  SCBResponseWrapper,
  SCBTokenOutput,
} from '@cgs-bank/external-api-services/scb-api/common';
import { getConfigFromSsm } from '@libs/ssm-config';

const functions = {
  getToken: async (): Promise<SCBTokenOutput> => {
    const [baseUrl, applicationKey, applicationSecret] = await getConfigFromSsm(
      'cgs/bank-services/scb/partner',
      ['base-api', 'api-key', 'api-secret']
    );
    const result = await axios.post(
      `${baseUrl}/v1/oauth/token`,
      {
        applicationKey: applicationKey,
        applicationSecret: applicationSecret,
      },
      {
        headers: {
          'content-type': 'application/json',
          resourceOwnerId: applicationKey,
          requestUId: uuid.v4(),
          'accept-language': 'EN',
        },
      }
    );
    try {
      const content = result.data;
      if (content.status.code != '1000') {
        throw 'Invalid Request';
      }

      return content.data as SCBTokenOutput;
    } catch (error) {
      console.log(error);
      throw error;
    }
  },

  ddRegistration: async (
    accessToken,
    data: SCBRegistrationRequest,
    publicKey: string
  ): Promise<SCBResponseWrapper<SCBRegistrationResponse>> => {
    const [baseUrl, applicationKey, merchantId, billerId] =
      await getConfigFromSsm('cgs/bank-services/scb/partner', [
        'base-api',
        'api-key',
        'merchant-id',
        'biller-id',
      ]);
    console.log('Before Encrypt');
    console.log(JSON.stringify(data));
    console.log(encrypt(JSON.stringify(data), publicKey));
    const result = await axios.post(
      `${baseUrl}/v1/registration/web/init`,
      {
        merchantId: merchantId,
        subAccountId: billerId,
        encryptedValue: encrypt(JSON.stringify(data), publicKey),
      },
      {
        headers: {
          'content-type': 'application/json',
          authorization: `Bearer ${accessToken}`,
          resourceOwnerId: applicationKey,
          requestUId: uuid.v4(),
          'accept-language': 'EN',
        },
      }
    );
    try {
      const content = result.data;
      if (content.status.code != '1000') {
        throw 'Invalid Request';
      }
      console.log(content);
      return {
        status: content.status,
        merchantId: content.data.merchantId,
        data: {
          webURL: content.data.registrationResponse.webUrl,
        },
        validationMessages: content.data.validationMessages,
      };
    } catch (error) {
      console.log(error);
      throw error;
    }
  },

  ddRegistrationInquiry: async (
    accessToken,
    data: SCBRegistrationInquiryRequest,
    publicKey: string
  ): Promise<SCBResponseWrapper<SCBRegistrationInquiryResponse>> => {
    const [baseUrl, applicationKey, merchantId] = await getConfigFromSsm(
      'cgs/bank-services/scb/partner',
      ['base-api', 'api-key', 'merchant-id']
    );
    const result = await axios.post(
      `${baseUrl}/v1/registration/inquiry`,
      {
        merchantId: merchantId,
        encryptedValue: encrypt(JSON.stringify(data), publicKey),
      },
      {
        headers: {
          'content-type': 'application/json',
          authorization: `Bearer ${accessToken}`,
          resourceOwnerId: applicationKey,
          requestUId: uuid.v4(),
          'accept-language': 'EN',
        },
      }
    );
    try {
      const content = result.data;
      if (content.status.code != '1000') {
        throw 'Invalid Request';
      }
      return {
        status: content.status,
        merchantId: content.data.merchantId,
        data: JSON.parse(
          decrypt(content.data.encryptedValue, publicKey)
        ) as SCBRegistrationInquiryResponse,
        validationMessages: content.data.validationMessages,
      };
    } catch (error) {
      console.log(error);
      throw error;
    }
  },

  ddPay: async (
    accessToken,
    data: SCBDDPaymentRequest,
    publicKey: string
  ): Promise<SCBResponseWrapper<SCBDDPaymentResponse>> => {
    const [baseUrl, applicationKey, merchantId, billerId] =
      await getConfigFromSsm('cgs/bank-services/scb/partner', [
        'base-api',
        'api-key',
        'merchant-id',
        'biller-id',
      ]);
    const result = await axios.post(
      `${baseUrl}/v1/payment/direct/ddpay`,
      {
        merchantId: merchantId,
        subAccountId: billerId,
        encryptedValue: encrypt(JSON.stringify(data), publicKey),
      },
      {
        headers: {
          'content-type': 'application/json',
          authorization: `Bearer ${accessToken}`,
          resourceOwnerId: applicationKey,
          requestUId: uuid.v4(),
          'accept-language': 'EN',
        },
      }
    );

    const content = result.data;
    if (content.status.code != '1000') {
      throw 'Invalid Request';
    }

    return {
      status: content.status,
      merchantId: content.data.merchantId,
      subAccountId: content.data.subAccountId,
      data: JSON.parse(
        decrypt(content.data.encryptedValue, publicKey)
      ) as SCBDDPaymentResponse,
      validationMessages: content.data.validationMessages,
    };
  },

  ddPaymentInquiry: async (
    accessToken,
    data: SCBDDPaymentInquiryRequest,
    publicKey: string
  ): Promise<SCBResponseWrapper<SCBDDPaymentInquiryResponse>> => {
    const [baseUrl, applicationKey, merchantId] = await getConfigFromSsm(
      'cgs/bank-services/scb/partner',
      ['base-api', 'api-key', 'merchant-id', 'biller-id']
    );
    const result = await axios.post(
      `${baseUrl}/v1/payment/inquiry`,
      {
        merchantId: merchantId,
        encryptedValue: encrypt(JSON.stringify(data), publicKey),
      },
      {
        headers: {
          'content-type': 'application/json',
          authorization: `Bearer ${accessToken}`,
          resourceOwnerId: applicationKey,
          requestUId: uuid.v4(),
          'accept-language': 'EN',
        },
      }
    );

    const content = result.data;
    return {
      status: content.status,
      merchantId: content.data.merchantId,
      subAccountId: content.data.subAccountId,
      data: JSON.parse(
        decrypt(content.data.encryptedValue, publicKey)
      ) as SCBDDPaymentInquiryResponse,
      validationMessages: content.data.validationMessages,
    };
  },
};

export default functions;
