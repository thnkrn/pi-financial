import axios from 'axios';
import { RegisterPrepareResponse } from '@cgs-bank/external-api-services/bay-api/response/register-prepare-response';
import { v4 as uuidv4 } from 'uuid';
import { RegisterPrepareRequest } from '@cgs-bank/external-api-services/bay-api/request/register-prepare-request';
import { getConfigFromSsm } from '@libs/ssm-config';

const functions = {
  registerPrepare: async (
    request: RegisterPrepareRequest,
    accessToken: string
  ): Promise<RegisterPrepareResponse> => {
    try {
      const body = {
        'direct-debit-registration': {
          ref1: request.ref1,
          ref2: request.ref2,
        },
      };
      const [baseUrl] = await getConfigFromSsm('cgs/bank-services/bay', [
        'base-api',
      ]);

      const uuid = uuidv4();
      console.info(`accessToken: ${accessToken}, uuid: ${uuid}`);

      const result = await axios.post(
        `${baseUrl}/rest/api/v1/authorization/prepareOAuthContext`,
        body,
        {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${accessToken}`,
            'X-Client-Transaction-ID': uuid,
          },
        }
      );

      return result.data as RegisterPrepareResponse;
    } catch (error) {
      console.log(error);
    }
  },
};

export default functions;
