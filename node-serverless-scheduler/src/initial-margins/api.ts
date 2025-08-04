import got from 'got';

export type InitialMargin = {
  symbol: string;
  productType: 'FUT' | 'PHY' | 'OOP';
  im: string;
};

export type UpsertInitialMarginRequest = {
  asOfDate: Date;
  data: InitialMargin[];
};

export type UpsertInitialMarginResponse = {
  data: boolean;
};

export const upsertInitialMargin = async (
  url: string,
  requestData: UpsertInitialMarginRequest
) => {
  const u = new URL(url);
  return got
    .post(u, { json: requestData })
    .json<UpsertInitialMarginResponse>()
    .catch((e) => {
      console.error(`Failed to upsert initial margins. Exception: ${e}`);
      throw e;
    });
};
