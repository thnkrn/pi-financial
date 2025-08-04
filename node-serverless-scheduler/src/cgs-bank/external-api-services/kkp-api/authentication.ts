import axios from 'axios';
import { KKPTokenResponse } from '@cgs-bank/external-api-services/kkp-api/common';
import { URLSearchParams } from 'url';
import { getConfigFromSsm } from '@libs/ssm-config';

const functions = {
  getToken: async (): Promise<KKPTokenResponse> => {
    const [baseUrl, authorization] = await getConfigFromSsm(
      'cgs/bank-services/kkp',
      ['base-api', 'authorization']
    );
    const params = new URLSearchParams();
    params.append('grant_type', 'client_credentials');

    const result = await axios.post(
      `${baseUrl}/microgw-authen/oauth/v1/token/`,
      params,
      {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
          Authorization: `Basic ${authorization}`,
        },
      }
    );
    return result.data as KKPTokenResponse;
  },
};

export default functions;
