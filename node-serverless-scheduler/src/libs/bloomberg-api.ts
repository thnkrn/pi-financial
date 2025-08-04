import got from 'got';
import { SecId } from 'src/constants/bloomberg';

const timeoutConfig = { request: Number(process.env.API_TIMEOUT || 100000) };

type RequestEquityClosePrice = {
  context: {
    vocab: string;
    base: string;
  };
  type: string;
  title: string;
  description: string;
  statusCode: number;
  request: {
    identifier: string;
  };
};

export type RequestEquityClosePriceResponse = {
  data: RequestEquityClosePrice;
};

interface SecurityIdentifier {
  key: SecId;
  value: string[];
}

/**
 * Requests the close price of equities from the Bloomberg service.
 *
 * @param {string} bloombergServiceHost - The host address of the Bloomberg service.
 * @param {SecurityIdentifier[]} securityIdentifiers - An array of pairs of SecId and FIGI or ISIN for the equities.
 * @return {Promise<RequestEquityClosePriceResponse>} A promise that resolves to the response containing the close prices of the equities.
 */
export async function requestEquityClosePrice(
  bloombergServiceHost: string,
  securityIdentifiers: SecurityIdentifier[]
) {
  const url = new URL('requests/reconcile', bloombergServiceHost);

  return got
    .post(url, {
      headers: {
        accept: 'application/json',
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        securityIdentifiers: securityIdentifiers,
        mediaType: 'textCsv',
      }),
      timeout: timeoutConfig,
      hooks: {
        beforeRequest: [
          (request) =>
            console.info(
              `HTTP Request URL: ${request.url} Body: ${request.body}`
            ),
        ],
      },
    })
    .json<RequestEquityClosePriceResponse>()
    .catch((e) => {
      console.error(
        `Failed to request global equity close price. Exception: ${e}`
      );
      throw e;
    });
}

type GetRequestStatus = {
  title: string;
  description: string;
  identifier: string;
  contains: [
    {
      id: string;
      type: string;
      identifier: string;
      contentType: string;
      accessible: boolean;
      sampleOf: string;
    }
  ];
};

export type GetRequestStatusResponse = {
  data: GetRequestStatus;
};

/**
 * Retrieves the status of a request from the Bloomberg service.
 *
 * @param {string} bloombergServiceHost - The host URL of the Bloomberg service.
 * @param {string} identifier - The identifier of the request.
 * @return {Promise<Response>} A promise that resolves to the response from the Bloomberg service.
 */
export async function getRequestStatus(
  bloombergServiceHost: string,
  identifier: string
) {
  const url = new URL(
    `/distributions/${identifier}/distributions`,
    bloombergServiceHost
  );
  console.info('url', url);

  return got.get(url, {
    timeout: timeoutConfig,
    throwHttpErrors: false,
    responseType: 'json',
  });
}

/**
 * Downloads the equity close price from the Bloomberg service.
 *
 * @param {string} bloombergServiceHost - The host of the Bloomberg service.
 * @param {string} identifier - The identifier of the equity.
 * @return {Promise<AxiosResponse<any>>} A promise that resolves to the response from the Bloomberg service.
 */
export async function downloadEquityClosePrice(
  bloombergServiceHost: string,
  identifier: string
) {
  const url = new URL(
    `/distributions/${identifier}/distributions/${identifier}/csv`,
    bloombergServiceHost
  );
  console.info('url', url);

  return got
    .get(url, { timeout: timeoutConfig, responseType: 'buffer' })
    .catch((e) => {
      console.error(`Failed to download equity close price. Exception: ${e}`);
      throw e;
    });
}
