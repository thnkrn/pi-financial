import { AuthenticationResponse } from '@cgs-bank/external-api-services/bay-api/response/authentication-response';
import axios from 'axios';
import { getConfigFromSsm } from '@libs/ssm-config';

const functions = {
  getToken: async (
    grantType: string,
    clientId: string,
    clientSecret: string
  ): Promise<AuthenticationResponse> => {
    const params = new URLSearchParams();
    params.append('grant_type', grantType);
    params.append('client_id', clientId);
    params.append('client_secret', clientSecret);
    const [baseUrl] = await getConfigFromSsm('cgs/bank-services/bay', [
      'base-api',
    ]);

    const result = await axios.post(`${baseUrl}/auth/oauth/v2/token`, params, {
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
    });

    return result.data as AuthenticationResponse;
  },
};

export default functions;
