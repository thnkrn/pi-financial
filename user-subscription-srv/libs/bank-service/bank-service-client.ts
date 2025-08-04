import { AESCoder } from '@libs/bank-service/aes-coder';
import { BankServiceResponseWrapper } from '@libs/bank-service/model/common';
import { GenerateQrRequest } from '@libs/bank-service/model/generate-qr-request';
import { GenerateQrResponse } from '@libs/bank-service/model/generate-qr-response';
import { GetTokenResponse } from '@libs/bank-service/model/get-token-response';
import Logger from '@utils/datadog-utils';
import axios from 'axios';

export class BankServiceClient {
  private readonly aesCoder = new AESCoder();

  baseUrl: string;
  apiKey: string;

  constructor(
    baseUrl: string = process.env.BANK_SERVICE_URL,
    apiKey: string = process.env.BANK_SERVICE_API_KEY,
  ) {
    this.baseUrl = baseUrl;
    this.apiKey = apiKey;
  }

  async getToken() {
    Logger.log(`getToken (${this.baseUrl}/GetToken)`);
    return axios
      .post(
        `${this.baseUrl}/GetToken`,
        {},
        {
          headers: {
            'content-type': 'application/json',
            'x-api-key': this.apiKey,
          },
        },
      )
      .then((result) => result.data as GetTokenResponse);
  }

  async generateQrPayment(token: string, request: GenerateQrRequest) {
    Logger.log(`generateQrPayment (${this.baseUrl}/KKPPayment/GenerateQR)`);
    return axios
      .post(
        `${this.baseUrl}/KKPPayment/GenerateQR`,
        JSON.stringify({
          data: this.aesCoder.encrypt(JSON.stringify(request)),
        }),
        {
          headers: {
            'content-type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
        },
      )
      .then((result) => {
        const { data } = result.data;
        const content = JSON.parse(
          this.aesCoder.decrypt(data),
        ) as BankServiceResponseWrapper<GenerateQrResponse>;

        if (!content.status.status) {
          throw content.status.internalStatusCode;
        }
        return content.data;
      });
  }
}
