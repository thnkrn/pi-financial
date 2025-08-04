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

const postCreateOracleDaily: Handler = async () => {
  try {
    const { userSubscriptionServiceHost } = await getConfig();
    const now = nowBangkok();
    // now.day() === 2 is Tuesday, the service will trigger at Tue 12:05AM GMT+7
    // so, this process will retrieve data from Sat 12:01AM GMT+7 to Mon 11:59PM GMT+7
    const from = now.subtract(now.day() === 2 ? 3 : 1, 'day').set('minute', 1);
    const to = now.set('minute', 0);

    console.log(`Query at ${now} from ${from} to ${to}`);

    const bodyData = {
      fromDate: from.format('YYYYMMDDHHmm'),
      toDate: to.format('YYYYMMDDHHmm'),
    };
    const response = await got
      .post(
        `${userSubscriptionServiceHost}/internal/tasks/oracle-daily-report`,
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
    console.error('Failed to create Oracle Daily\n' + JSON.stringify(e));
  }
};

export const main = middyfy(postCreateOracleDaily);
