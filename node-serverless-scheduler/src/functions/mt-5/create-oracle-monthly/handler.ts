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

const postCreateOracleMonthly = async () => {
  try {
    const { userSubscriptionServiceHost } = await getConfig();
    const now = nowBangkok().startOf('month');
    const from = now
      .add(-1, 'year')
      .startOf('year')
      .set('hour', 0)
      .set('minute', 0);

    console.log(`Query at ${now} from ${from}`);

    const bodyData = {
      fromDate: from.format('YYYYMMDDHHmm'),
    };
    const response = await got
      .post(
        `${userSubscriptionServiceHost}/internal/tasks/oracle-monthly-report`,
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
    console.error('Failed to create Oracle Monthly\n' + JSON.stringify(e));
  }
};

export const main = middyfy(postCreateOracleMonthly);
