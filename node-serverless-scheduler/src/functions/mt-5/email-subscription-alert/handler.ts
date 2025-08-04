import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';
import dayjs from 'dayjs';
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

const postSendEmail = async () => {
  try {
    const { userSubscriptionServiceHost } = await getConfig();

    const bodyData = { toDate: dayjs().endOf('month').format('YYYY-MM-DD') };
    const result = await got
      .post(
        `${userSubscriptionServiceHost}/internal/tasks/remind-user-subscription-schedule`,
        {
          json: bodyData,
          headers: {
            'Content-Type': 'application/json',
          },
        }
      )
      .json();
    console.info('Send reminder user subscription.\n');

    return result;
  } catch (e) {
    console.error(
      'Failed to send reminder user subscription\n' + JSON.stringify(e)
    );
    throw e;
  }
};

export const main = middyfy(postSendEmail);
