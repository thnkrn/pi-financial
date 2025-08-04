import { requestEquityClosePrice } from '@libs/bloomberg-api';
import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';
import { SecId } from 'src/constants/bloomberg';

async function getBloomBergConfig() {
  const [bloombergServiceHost] = await getConfigFromSsm('report', [
    'bloomberg-srv-host',
  ]);

  return {
    bloombergServiceHost,
  };
}

interface Event {
  body: Payload;
}

interface Payload {
  securityIdentifiers: SecurityIdentifier[];
}

interface SecurityIdentifier {
  key: SecId;
  value: string[];
}

/**
 * Runs the function with the provided event.
 * @param event The event object.
 * @returns A promise that resolves to an object with the identifier property.
 */
const run = async (event: Event) => {
  console.info(event);
  const payload: Payload = event.body;
  console.info(payload);

  try {
    const bloombergConfig = await getBloomBergConfig();
    const response = await requestEquityClosePrice(
      bloombergConfig.bloombergServiceHost,
      payload.securityIdentifiers
    );

    return {
      body: {
        identifier: response?.data?.request?.identifier,
      },
    };
  } catch (error) {
    console.error(
      'Error requesting Bloomberg equity close price. Exception: ',
      +JSON.stringify(error)
    );
    throw error;
  }
};
export const main = middyfy(run);
