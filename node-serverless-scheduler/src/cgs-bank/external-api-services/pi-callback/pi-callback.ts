import axios from 'axios';
import { BankServiceResponseWrapper } from '@cgs-bank/models/common/bank-service-response-wrapper';
import { RegistrationCallbackResponse } from '@cgs-bank/models/common/registration';
import { WalletQrPaymentCallbackRequest } from '@cgs-bank/models/common/qr-payment';
import { WalletBillPaymentRequest } from '@cgs-bank/models/common/bill-payment';
import { getConfigFromSsm } from '@libs/ssm-config';

const functions = {
  postODDRegistrationCallback: async (
    response: BankServiceResponseWrapper<RegistrationCallbackResponse>
  ): Promise<string> => {
    const [baseUrl] = await getConfigFromSsm('cgs/bank-services', [
      'pi-backend-host',
    ]);
    return axios.post(`${baseUrl}/public/odd/registration/callback`, response);
  },

  postDepositQrPaymentCallback: async (
    response: BankServiceResponseWrapper<WalletQrPaymentCallbackRequest>
  ): Promise<string> => {
    const [baseUrl] = await getConfigFromSsm('cgs/bank-services', [
      'pi-backend-host',
    ]);
    return axios.post(
      `${baseUrl}/internal/wallet/deposit/qr-payment/callback`,
      response
    );
  },

  postDepositBillPayment: async (
    request: WalletBillPaymentRequest
  ): Promise<string> => {
    const [baseUrl] = await getConfigFromSsm('cgs/bank-services', [
      'pi-backend-host',
    ]);
    return axios.post(`${baseUrl}/internal/deposit/bill-payment`, request);
  },
};

export default functions;
