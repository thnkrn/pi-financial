import * as https from 'https';

import axios from 'axios';
import { generateAuthParams } from '@cgs-bank/utils/kbank-auth';
import { getTimestamp, TimestampFormat } from '@cgs-bank/utils/timestamp';
import { getConfigFromSsm } from '@libs/ssm-config';

export interface KBANKRegistrationRequest {
  citizenId: string;
  externalReference: string;
  payeeShortName?: string;
  userEmail?: string;
  userMobileNo?: string;
}

export interface KBANKRegistrationResponse {
  reg_id: string;
  return_status: string;
  return_code: string;
  return_message: string;
}

const functions = {
  registrationToken: async (
    input: KBANKRegistrationRequest
  ): Promise<KBANKRegistrationResponse> => {
    const [
      baseUrl,
      externalSystemName,
      payeeShortName,
      passphrase,
      serviceName,
      cert,
    ] = await getConfigFromSsm('cgs/bank-services/kbank', [
      'base-api',
      'external-system-short-name',
      'payee-short-name',
      'passphrase',
      'service-name',
      'cert',
    ]);

    const httpsAgent = new https.Agent({
      cert: cert,
    });

    return axios
      .post(
        `${baseUrl}/ws/v1/registerinit`,
        {
          transaction_type: '0620',
          encoding: 'UTF8',
          ...(input.userMobileNo ? { user_mobile_no: input.userMobileNo } : {}),
          ...(input.userEmail ? { user_email: input.userEmail } : {}),
          external_system: externalSystemName,
          payee_short_name: payeeShortName,
          id: input.citizenId,
          external_reference: input.externalReference,
          service_name: serviceName,
          timestamp: getTimestamp(TimestampFormat.Normal),
          auth_parameter: generateAuthParams(
            passphrase,
            externalSystemName,
            payeeShortName,
            input.externalReference
          ),
        },
        {
          httpsAgent: httpsAgent,
          headers: {
            'Content-Type': 'application/json',
          },
        }
      )
      .then((result) => {
        console.log(result);
        try {
          const content = result.data;
          if (content.return_status != '0') {
            console.log(content);
            throw 'Invalid Response';
          }

          return content as KBANKRegistrationResponse;
        } catch (error) {
          console.log(error);
          throw error;
        }
      });
  },
};

export default functions;
