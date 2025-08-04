import { getRequestStatus } from '@libs/bloomberg-api';
import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';

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
  identifier: string;
}

/**
 * Run function that retrieves the status of a request from Bloomberg equity close price.
 * @param event - The event triggering the function.
 * @returns The status of the request.
 */
const run = async (event: Event) => {
  console.info(event);
  const payload: Payload = event.body;
  console.info(payload);

  try {
    const bloombergConfig = await getBloomBergConfig();
    const response = await getRequestStatus(
      bloombergConfig.bloombergServiceHost,
      payload?.identifier
    );

    const status =
      response.statusCode === 200
        ? 'SUCCEEDED'
        : response.statusCode === 404
        ? 'PENDING'
        : 'FAILED';

    return {
      body: {
        status,
      },
    };
  } catch (error) {
    console.error(
      'Error getting Bloomberg equity close price request status. Exception: ',
      +JSON.stringify(error)
    );
    throw error;
  }
};
export const main = middyfy(run);
