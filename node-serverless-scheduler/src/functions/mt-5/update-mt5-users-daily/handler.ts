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

const postUpdateMT5UsersDaily = async () => {
  try {
    const { userSubscriptionServiceHost } = await getConfig();

    const now = nowBangkok();
    // fromDate = yesterday, but if today is monday, from = Friday
    const bodyData = {
      fromDate: now
        .subtract(now.day() === 1 ? 3 : 1, 'day')
        .set('minute', 1)
        .format('YYYYMMDD'),
    };
    const response = await got
      .post(
        `${userSubscriptionServiceHost}/internal/tasks/sync-mt5-user-from-freeview`,
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
    console.error('Failed to update MT5 Users Daily\n' + JSON.stringify(e));
  }
};

export const main = middyfy(postUpdateMT5UsersDaily);
