import got from 'got';

const timeoutConfig = { request: Number(process.env.API_TIMEOUT || 100000) };

export type UserBulkResponse = {
  data: {
    id: string;
    custCodes: Array<string>;
    firstnameTh: string;
    lastnameTh: string;
    firstnameEn: string;
    lastnameEn: string;
  }[];
};

export async function resolveCustomerIds(
  customerCodes: Array<string>,
  userServiceHost: string
) {
  const url = new URL('/internal/user/bulk', userServiceHost);
  url.searchParams.append('isCustCode', 'true');
  customerCodes.forEach((cc) => url.searchParams.append('ids', cc));

  return got
    .get(url, { timeout: timeoutConfig })
    .json<UserBulkResponse>()
    .catch((e) => {
      console.error(
        `Failed to get user from custCode. custCodes: ${customerCodes}. Exception: ${e}`
      );
      throw e;
    });
}
