import { formatJSONResponse } from '@libs/api-gateway';
import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';
import { nowBangkok } from '@libs/time-utils';
import { Handler } from 'aws-lambda';
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

const postUpdateMT5SubscriptionsDaily: Handler = async () => {
  try {
    const now = nowBangkok();
    const fromDate = now
      .subtract(now.day() === 1 ? 3 : 1, 'day')
      .set('minute', 1);
    const toDate = now.set('minute', 0);

    console.log(`Query at ${now} from ${fromDate} to ${toDate}`);

    const { userSubscriptionServiceHost } = await getConfig();
    const bodyData = {
      fromDate: fromDate.format('YYYYMMDDHHmm'),
      toDate: toDate.format('YYYYMMDDHHmm'),
    };
    const response = await got
      .post(
        `${userSubscriptionServiceHost}/internal/tasks/update-subscriptions`,
        {
          json: bodyData,
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
    console.error(
      'Failed to update MT5 Subscriptions Daily\n' + JSON.stringify(e)
    );
  }
};

export const main = middyfy(postUpdateMT5SubscriptionsDaily);
