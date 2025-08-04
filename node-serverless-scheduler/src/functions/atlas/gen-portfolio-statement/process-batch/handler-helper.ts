import { DBConfigType, getSequelize, SecretManagerType } from '@libs/db-utils';
import { getEmployeeByIds } from '@libs/employee-api';
import { getConfigFromSsm } from '@libs/ssm-config';
import {
  CustomerType,
  getCustomerType,
  getTradingAccountMarketingInfo,
  getUserAddressByUserId,
  getUserIdByCustcode,
  InternalCustomerByIdentificationNo,
  MarketingInfo,
  UserInfo,
} from '@libs/user-v2-api';
import { writeFile } from 'fs';
import {
  GroupedMarketingProductType,
  GroupedProductType,
} from './internal-model';
import { CustomerStatementData } from './response-model';

export async function getUserIdByCustomerCode(customerCode: string) {
  console.log(
    `[API][USERV2] getUserIdByCustomerCode: ${customerCode} - (AWS_ENVIRONMENT:${process.env.AWS_ENVIRONMENT})`
  );
  const userServiceV2Host =
    process.env.DEBUG_LOCAL === 'true'
      ? 'http://user-api-v2.nonprod.pi.internal'
      : (await getConfig()).userServiceV2Host;
  const userInfos = await getUserIdByCustcode(customerCode, userServiceV2Host);
  console.log(`[API] getUserIdByCustomerCode success`);
  return userInfos.data;
}

function mergeAccountAndMarketing(
  custCodes: string[],
  marketingId: string,
  userInfo: UserInfo,
  user: Record<string, CustomerType>,
  marketing: Record<string, MarketingInfo[]>
): InternalCustomerByIdentificationNo {
  const custCodeEntries = custCodes
    .map((custCode) => {
      return {
        custCode,
        customerType: user[custCode].customerType,
        customerSubType: user[custCode].customerSubType,
        tradingAccounts: (marketing[custCode] ?? []).map((acc) => ({
          tradingAccountId: acc.id,
          tradingAccountNo: acc.tradingAccountNo,
          accountType: acc.accountType,
          accountTypeCode: acc.accountTypeCode,
          exchangeMarketId: acc.exchangeMarketId,
        })),
        basicInfo: {
          name: {
            firstnameEn: userInfo.firstnameEn,
            lastnameEn: userInfo.lastnameEn,
            firstnameTh: userInfo.firstnameTh,
            lastnameTh: userInfo.lastnameTh,
          },
          dateOfBirth: userInfo.dateOfBirth,
        },
        customerCodeInfo: {
          email: userInfo.email,
        },
      };
    })
    .filter(Boolean);

  return {
    data: {
      identificationNo: userInfo.citizenId,
      marketings: [
        {
          marketingId,
          custCodes: custCodeEntries,
        },
      ],
    },
  };
}

export async function getCustomerTradingInfo(
  userInfo: UserInfo,
  marketingId: string
) {
  console.log(
    `[API][USERV2] getCustomerTradingInfo: ${JSON.stringify(
      userInfo.custCodes
    )}`
  );
  const userServiceV2Host =
    process.env.DEBUG_LOCAL === 'true'
      ? 'http://user-api-v2.nonprod.pi.internal'
      : (await getConfig()).userServiceV2Host;

  const allMarketingInfo: Record<string, MarketingInfo[]> = {};
  const allCustomerTypeInfo: Record<string, CustomerType> = {};
  const matchedCustCodes: string[] = [];
  for (const custCode of userInfo.custCodes) {
    try {
      const customerType = await getCustomerType(custCode, userServiceV2Host);
      if (customerType?.data) {
        allCustomerTypeInfo[custCode] = customerType.data;
      }
      const response = await getTradingAccountMarketingInfo(
        custCode,
        userServiceV2Host
      );
      if (response?.data?.length) {
        const matchedData = (response.data as MarketingInfo[]).filter(
          (item) =>
            item.marketingId === marketingId && item.endDate === '9999-12-31'
        );
        if (matchedData.length) {
          allMarketingInfo[custCode] = matchedData;
          matchedCustCodes.push(custCode);
        }
      }
    } catch (error) {
      console.error(`Error fetching marketing info for ${custCode}:`, error);
    }
  }

  const customerIdentification = mergeAccountAndMarketing(
    matchedCustCodes,
    marketingId,
    userInfo,
    allCustomerTypeInfo,
    allMarketingInfo
  );

  console.log(`[API] getCustomerTradingInfo success`);
  return customerIdentification;
}

export async function getCustomerAddress(userId: string) {
  const userServiceV2Host =
    process.env.DEBUG_LOCAL === 'true'
      ? 'http://user-api-v2.nonprod.pi.internal'
      : (await getConfig()).userServiceV2Host;

  const userAddress = await getUserAddressByUserId(userId, userServiceV2Host);
  console.log(`[API] getCustomerAddress success`);
  return userAddress.data;
}

export async function getEmployeeInfos(employeeIds: string[]) {
  console.log(`[API][Employee] getEmployeeInfos: ${employeeIds.toString()}`);
  const employeeServiceHost =
    process.env.DEBUG_LOCAL === 'true'
      ? 'http://employee.prod.pi.internal/'
      : (await getConfig()).employeeServiceHost;
  const tradingAccounts = await getEmployeeByIds(
    employeeIds,
    employeeServiceHost
  );
  console.log(`[API] getEmployeeInfos success`);
  return tradingAccounts;
}

export type GroupedByEmail = {
  customerEmail: string;
  dateOfBirth: string;
  customerInfo: InternalCustomerByIdentificationNo;
};

export function splitByEmail(
  input: InternalCustomerByIdentificationNo
): GroupedByEmail[] {
  const groupedByEmailMap: {
    [email: string]: {
      dateOfBirth: string;
      customerInfo: InternalCustomerByIdentificationNo;
    };
  } = {};

  input.data.marketings.forEach((marketing) => {
    marketing.custCodes.forEach((custCode) => {
      const email = custCode.customerCodeInfo.email;
      const dateOfBirth = custCode.basicInfo.dateOfBirth;
      if (email) {
        if (!groupedByEmailMap[email]) {
          groupedByEmailMap[email] = {
            dateOfBirth,
            customerInfo: {
              data: {
                identificationNo: input.data.identificationNo,
                marketings: [],
              },
            },
          };
        }

        const emailMarketings =
          groupedByEmailMap[email].customerInfo.data.marketings;
        let marketingGroup = emailMarketings.find(
          (m) => m.marketingId === marketing.marketingId
        );

        if (!marketingGroup) {
          marketingGroup = {
            marketingId: marketing.marketingId,
            custCodes: [],
          };
          emailMarketings.push(marketingGroup);
        }

        marketingGroup.custCodes.push(custCode);
      }
    });
  });

  const groupedByEmail: GroupedByEmail[] = Object.keys(groupedByEmailMap).map(
    (email) => ({
      customerEmail: email,
      dateOfBirth: groupedByEmailMap[email].dateOfBirth,
      customerInfo: groupedByEmailMap[email].customerInfo,
    })
  );

  return groupedByEmail;
}

export function buildGroupedMarketingProductTypes(
  customerInfo: InternalCustomerByIdentificationNo
) {
  try {
    const result: GroupedMarketingProductType[] =
      customerInfo.data.marketings.map((marketing) => {
        const marketingGroup: GroupedMarketingProductType = {
          marketingId: marketing.marketingId,
          productTypes: [],
        };

        marketing.custCodes.forEach((custcode) => {
          custcode.tradingAccounts.forEach((curr) => {
            const customerInfo = marketing.custCodes.find(
              (data) => data.custCode == custcode.custCode
            );
            // Find or create the customer code group within the marketing group
            let productTypeGroup: GroupedProductType =
              marketingGroup.productTypes.find(
                (productType) =>
                  productType.exchangeMarketId === curr.exchangeMarketId &&
                  productType.accountType === curr.accountType &&
                  productType.accountTypeCode === curr.accountTypeCode &&
                  productType.customerType === customerInfo.customerType &&
                  productType.customerSubType === customerInfo.customerSubType
              );
            if (!productTypeGroup) {
              productTypeGroup = {
                exchangeMarketId: curr.exchangeMarketId,
                accountType: curr.accountType,
                accountTypeCode: curr.accountTypeCode,
                customerType: customerInfo.customerType,
                customerSubType: customerInfo.customerSubType,
                tradingAccounts: [],
              };
              marketingGroup.productTypes.push(productTypeGroup);
            }
            // Add trading account details to the customer code group
            productTypeGroup.tradingAccounts.push({
              custcode: custcode.custCode,
              tradingAccountNo: curr.tradingAccountNo,
            });
          });
        });
        return marketingGroup;
      });

    return result;
  } catch (error) {
    console.log(error);
    return [];
  }
}
export async function getConfig() {
  const [
    onboardServiceHost,
    employeeServiceHost,
    userServiceHost,
    userServiceV2Host,
  ] = await getConfigFromSsm('atlas', [
    'onboard-srv-host',
    'employee-srv-host',
    'user-srv-host',
    'user-srv-v2-host',
  ]);

  return {
    onboardServiceHost,
    employeeServiceHost,
    userServiceHost,
    userServiceV2Host,
  };
}

export function writeToFile(
  customerStatementData: CustomerStatementData,
  filename: string
) {
  writeFile(
    `./tmp/${filename}.json`,
    JSON.stringify(customerStatementData),
    (err) => {
      if (err) {
        console.error(err);
      } else {
        console.log('File written successfully');
      }
    }
  );
}

export function getLastDateToQuery(sendDate: Date, isOndemand: boolean) {
  function getLastDateOfTheMonth() {
    const now = new Date();

    const result = new Date(now.getFullYear(), now.getMonth(), 1);
    result.setDate(result.getDate() - 1);
    return result;
  }
  const lastDateToQuery = (
    isOndemand ? new Date(sendDate) : getLastDateOfTheMonth()
  )
    .toISOString()
    .split('T')[0];

  return lastDateToQuery;
}

export async function loadSequelize() {
  const sequelize = await getSequelize({
    parameterName: 'atlas',
    dbHost: 'PORTFOLIOSUMMARYDATABASE_HOST',
    dbPassword: 'PORTFOLIOSUMMARYDATABASE_PASSWORD',
    dbUsername: 'PORTFOLIOSUMMARYDATABASE_USERNAME',
    dbName: 'portfolio_summary_db',
    dbConfigType: DBConfigType.SecretManager,
    secretManagerType: SecretManagerType.Public,
    pool: {
      max: 2,
      min: 0, //similar to default
      idle: 0, //https://dev.to/dvddpl/event-loops-and-idle-connections-why-is-my-lambda-not-returning-and-then-timing-out-2oo7
      evict: 100,
    },
  });

  // or `sequelize.sync()`
  await sequelize.authenticate();

  return sequelize;
}

export function getFormattedDate(): string {
  const now = new Date();
  const year = now.getFullYear().toString();
  const month = (now.getMonth() + 1).toString().padStart(2, '0');
  const day = now.getDate().toString().padStart(2, '0');

  return `${year}${month}${day}`;
}
