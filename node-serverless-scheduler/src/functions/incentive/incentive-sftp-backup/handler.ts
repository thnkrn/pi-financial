import { formatJSONResponse } from '@libs/api-gateway';
import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';
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

const incentiveSftpBackup: Handler = async () => {
  try {
    const { userSubscriptionServiceHost } = await getConfig();
    console.log(userSubscriptionServiceHost);

    const response = await got
      .get(
        `${userSubscriptionServiceHost}/internal/tasks/temp/backup-incentive`,
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
    console.error('Failed to backup sftp incentive \n' + JSON.stringify(e));
  }
};

export const main = middyfy(incentiveSftpBackup);
