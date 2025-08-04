import { formatJSONResponse } from '@libs/api-gateway';
import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';
import { Handler } from 'aws-lambda';
import console from 'console';
import got from 'got';

async function getConfig() {
  const [dcaServiceHost] = await getConfigFromSsm('dca-service', [
    'dca-service-host',
  ]);

  return {
    dcaServiceHost,
  };
}

const fundTriggerCronjobHandler: Handler = async () => {
  console.log('Starting trigger DCA cron (Funds)');
  try {
    const { dcaServiceHost } = await getConfig();

    const response = await got
      .post(`${dcaServiceHost}/internal/tasks/trigger/funds`, {
        headers: {
          'Content-Type': 'application/json',
        },
      })
      .json();

    console.log(`response: ${JSON.stringify(response)}`);
    return formatJSONResponse({
      message: 'Send Completed',
    });
  } catch (e) {
    console.error('Failed to create Dca Trigger Daily\n' + JSON.stringify(e));
  }
};

export const fundTriggerCronjob = middyfy(fundTriggerCronjobHandler);
