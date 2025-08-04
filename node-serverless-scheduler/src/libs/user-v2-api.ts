import got from 'got';

const timeoutConfig = { request: Number(process.env.API_TIMEOUT || 100000) };

export type InternalUserInfoByCustCodeResponse = {
  code: string;
  data: [UserInfo];
  msg: string;
};
export type UserInfo = {
  citizenId: string;
  custCodes: [string];
  dateOfBirth: string;
  devices: [
    {
      deviceId: string;
      deviceIdentifier: string;
      deviceToken: string;
      language: string;
      notificationPreference: {
        important: boolean;
        market: boolean;
        order: boolean;
        portfolio: boolean;
        wallet: boolean;
      };
      platform: string;
    }
  ];
  email: string;
  firstnameEn: string;
  firstnameTh: string;
  id: string;
  lastnameEn: string;
  lastnameTh: string;
  phoneNumber: string;
  placeOfBirthCity: string;
  placeOfBirthCountry: string;
  tradingAccounts: [string];
  wealthType: string;
};

export type InternalUserAddressResponse = {
  code: string;
  data: {
    building: string;
    country: string;
    countryCode: string;
    district: string;
    floor: string;
    homeNo: string;
    place: string;
    province: string;
    provinceCode: string;
    road: string;
    soi: string;
    subDistrict: string;
    town: string;
    village: string;
    zipCode: string;
  };
  msg: string;
};

export type InternalMarketingResponse = {
  code: string;
  data: [MarketingInfo];
  msg: string;
};
export type MarketingInfo = {
  accountType: string;
  accountTypeCode: string;
  endDate: string;
  exchangeMarketId: string;
  id: string;
  marketingId: string;
  tradingAccountNo: string;
};

export type InternalUserAccountResponse = {
  code: string;
  data: CustomerType;
  msg: string;
};
export type CustomerType = {
  customerType: string;
  customerSubType: string;
};

export type InternalCustomerGroupByIdentificationNo = {
  data: {
    identificationNo: string;
    identificationHash: string;
    marketings: InternalMarketingGroup[];
  }[];
  page: number;
  total: number;
};

export type InternalCustomerByIdentificationNo = {
  data: {
    identificationNo: string;
    marketings: InternalMarketingGroup[];
  };
};

export type InternalMarketingGroup = {
  marketingId: string;
  custCodes: InternalCustCodeGroup[];
};

export type InternalCustCodeGroup = {
  custCode: string;
  customerType: string;
  customerSubType: string;
  basicInfo: {
    name: {
      firstnameEn: string;
      lastnameEn: string;
      firstnameTh: string;
      lastnameTh: string;
    };
    dateOfBirth: string;
  };
  customerCodeInfo: {
    email: string;
  };
  tradingAccounts: InternalTradingAccount[];
};

export type InternalTradingAccount = {
  tradingAccountNo: string;
  accountType: string;
  accountTypeCode: string;
  exchangeMarketId: string;
};

export async function getUserIdByCustcode(
  customerCode: string,
  userServiceHost: string
) {
  const url = new URL(
    `/internal/v1/users?accountId=${customerCode}`,
    userServiceHost
  );
  console.log(`Path: {UserV2}/internal/v1/users?accountId=${customerCode}`);
  try {
    return await got
      .get(url, { timeout: timeoutConfig })
      .json<InternalUserInfoByCustCodeResponse>();
  } catch (e) {
    console.error(
      `Failed to get users info from customerCode: ${customerCode}. Exception: ${e}`
    );
    throw e;
  }
}

export async function getUserAddressByUserId(
  userId: string,
  userServiceHost: string
) {
  const url = new URL(`/internal/v1/address`, userServiceHost);
  console.log(`Path: {UserV2}/internal/v1/address`);
  try {
    return await got
      .get(url, {
        timeout: timeoutConfig,
        headers: {
          'user-id': userId,
        },
      })
      .json<InternalUserAddressResponse>();
  } catch (e) {
    console.error(
      `Failed to get customer from userId: ${userId}. Exception: ${e}`
    );
    throw e;
  }
}

export async function getTradingAccountMarketingInfo(
  custCode: string,
  userServiceHost: string
) {
  const url = new URL(
    `/internal/v1/trading-accounts/marketing-infos?customerCodes=${custCode}`,
    userServiceHost
  );
  console.log(
    `Path: {UserV2}/internal/v1/trading-accounts/marketing-infos?customerCodes=${custCode}`
  );
  try {
    return await got
      .get(url, { timeout: timeoutConfig })
      .json<InternalMarketingResponse>();
  } catch (e) {
    console.error(
      `Failed to get trading account marketing info from custCode: ${custCode}. Exception: ${e}`
    );
    throw e;
  }
}

export async function getCustomerType(
  custCode: string,
  userServiceHost: string
) {
  const url = new URL(
    `internal/v1/user-accounts/customer-info/${custCode}`,
    userServiceHost
  );
  console.log(
    `Path: {UserV2}/internal/v1/user-accounts/customer-info/${custCode}`
  );
  try {
    return await got
      .get(url, { timeout: timeoutConfig })
      .json<InternalUserAccountResponse>();
  } catch (e) {
    console.error(
      `Failed to get customer type from custCode: ${custCode}. Exception: ${e}`
    );
    throw e;
  }
}

export async function getCustomerGroupByIdentificationNo(
  page: number,
  pageSize: number,
  includeTradingAccount: boolean,
  onboardServiceHost: string,
  marketingIds: string[] = [],
  customerSubTypes: string[] = [],
  cardTypes: string[] = []
) {
  const url = new URL(
    '/internal/customer/group-by-id-card-no',
    onboardServiceHost
  );
  console.log(`Path: {Onboard}/internal/customer/group-by-id-card-no`);
  return got
    .post(url, {
      headers: {
        accept: 'application/json',
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        page: page,
        pageSize: pageSize,
        IncludeTradingAccount: includeTradingAccount,
        marketingIds: marketingIds,
        customerSubType: customerSubTypes,
        cardTypes: cardTypes,
      }),
      timeout: timeoutConfig,
    })
    .json<InternalCustomerGroupByIdentificationNo>()
    .catch((e) => {
      console.error(
        `Failed to get customer group by id card no: ${page} - ${pageSize}. Exception: ${e}`
      );
      throw e;
    });
}
