import got from 'got';

const timeoutConfig = { request: Number(process.env.API_TIMEOUT || 100000) };

export type ResponseCodes = {
  id: string;
  machine: string;
  productType: string;
  suggestion: string;
  description: string;
  state: string;
};

export type ResponseCodesResults = {
  data: ResponseCodes[];
};

export async function getResponseCodes(
  backOfficeServiceHost: string,
  machine: string
) {
  const url = new URL('response_codes', backOfficeServiceHost);

  url.searchParams.append('Machine', machine);

  console.log('url', url);
  return got
    .get(url, { timeout: timeoutConfig })
    .json<ResponseCodesResults>()
    .catch((e) => {
      console.error(`Failed to get response codes. Exception: ${e}`);
      throw e;
    });
}
