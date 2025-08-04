import { formatJSONResponse } from '@libs/api-gateway';
import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';
import { nowBangkok } from '@libs/time-utils';
import * as console from 'console';
import got from 'got';

async function getConfig() {
  const [userSubscriptionServiceHost] = await getConfigFromSsm(
    'user-subscription',
    ['user-subscription-host']
  );

  return {
    userSubscriptionServiceHost,
  };
}

const postCreateTaxDaily = async () => {
  try {
    const { userSubscriptionServiceHost } = await getConfig();
    const now = nowBangkok();
    const from = now.subtract(now.day() === 1 ? 3 : 1, 'day').set('minute', 1);
    const to = now.set('minute', 0);

    console.log(`Query at ${now} from ${from} to ${to}`);

    const bodyData = {
      fromDate: from.format('YYYYMMDDHHmm'),
      toDate: to.format('YYYYMMDDHHmm'),
    };
    const response = await got
      .post(`${userSubscriptionServiceHost}/internal/tasks/tks-daily-report`, {
        json: bodyData,
        headers: {
          'Content-Type': 'application/json',
        },
      })
      .json();

    console.log(response);
    return formatJSONResponse({
      message: 'Send Completed',
    });
  } catch (e) {
    console.error('Failed to create Tax Daily\n' + JSON.stringify(e));
  }
};

export const main = middyfy(postCreateTaxDaily);
