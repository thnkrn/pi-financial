import got from 'got';

export type SyncInitialMarginRequest = {
  bucketName: string;
  fileKey: string;
};

export const SendSyncInitialMarginRequest = async (
  serviceHost: string,
  requestData: SyncInitialMarginRequest
) => {
  const url = new URL('internal/sync/initial-margin', serviceHost);
  return got
    .post(url, { json: requestData })
    .json()
    .catch((e) => {
      console.error(`Failed to sync set initial margins. Exception: ${e}`);
      throw e;
    });
};
