import { formatJSONResponse } from '@libs/api-gateway';
import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';
import { Handler } from 'aws-lambda';
import * as console from 'console';
import got from 'got';

async function getConfig() {
  const [onboardServiceHost] = await getConfigFromSsm('onboard', [
    'onboard-host',
  ]);

  return { onboardServiceHost };
}

const onboardCheckOpenAccountRequest: Handler = async () => {
  try {
    const { onboardServiceHost } = await getConfig();

    const response = await got
      .post(
        `${onboardServiceHost}/internal/tasks/check-open-account-request?days=1`,
        {
          headers: {
            'Content-Type': 'application/json',
          },
        }
      )
      .json();

    console.log(response);
    return formatJSONResponse({
      message: 'Send Completed',
    });
  } catch (e) {
    console.error(`Error has occurred: ${e}`);
  }
};

export const main = middyfy(onboardCheckOpenAccountRequest);
