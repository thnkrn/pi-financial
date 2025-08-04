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

const getYesterdayOrLastFriday = () => {
  const today = new Date();
  const yesterday = new Date(today);
  yesterday.setDate(today.getDate() - 1);

  // Check if yesterday is a weekend (Saturday or Sunday)
  const isWeekend = yesterday.getDay() === 0 || yesterday.getDay() === 6;

  // If yesterday is a weekend, adjust to the previous Friday
  if (isWeekend) {
    yesterday.setDate(yesterday.getDate() - (yesterday.getDay() === 0 ? 2 : 1));
  }

  return yesterday;
};

const resyncDataFromFreewill: Handler = async (event, ctx) => {
  try {
    const { onboardServiceHost } = await getConfig();
    console.log(event);
    console.log(ctx);
    const targetDate = getYesterdayOrLastFriday().toLocaleDateString('en-US', {
      timeZone: 'Asia/Bangkok',
    });
    const response = await got
      .post(
        `${onboardServiceHost}/internal/freewill-sync/failed-date?targetDate=${targetDate}`,
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

export const main = middyfy(resyncDataFromFreewill);
