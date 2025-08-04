import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';
import { Handler } from 'aws-lambda';
import got from 'got';

async function getConfig() {
  const [contentServiceHost] = await getConfigFromSsm('content', [
    'content-srv-host',
  ]);
  return {
    contentServiceHost,
  };
}

const syncYouTubeVideos: Handler = async () => {
  const { contentServiceHost } = await getConfig();
  const response = await got
    .post(`${contentServiceHost}/internal/videos/sync-all-videos`)
    .json();
  console.log(response);
  return response;
};

export const main = middyfy(syncYouTubeVideos);
