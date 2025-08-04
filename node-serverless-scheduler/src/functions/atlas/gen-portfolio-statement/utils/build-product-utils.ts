import { Model, Op, Sequelize } from 'sequelize';

import dayjs from 'dayjs';
import { PortfolioBase } from 'db/portfolio-summary/models/PortfolioBase';
import { PortfolioBondDailySnapshot } from 'db/portfolio-summary/models/PortfolioBondDailySnapshot';
import { PortfolioBondOffshoreDailySnapshot } from 'db/portfolio-summary/models/PortfolioBondOffshoreDailySnapshot';
import { PortfolioCashDailySnapshot } from 'db/portfolio-summary/models/PortfolioCashDailySnapshot';
import { PortfolioGlobalEquityDailySnapshot } from 'db/portfolio-summary/models/PortfolioGlobalEquityDailySnapshot';
import { PortfolioGlobalEquityOtcDailySnapshot } from 'db/portfolio-summary/models/PortfolioGlobalEquityOtcDailySnapshot';
import { PortfolioMutualFundDailySnapshot } from 'db/portfolio-summary/models/PortfolioMutualFundDailySnapshot';
import { PortfolioStructuredProductDailySnapshot } from 'db/portfolio-summary/models/PortfolioStructuredProductDailySnapshot';
import { PortfolioStructuredProductOnshoreDailySnapshot } from 'db/portfolio-summary/models/PortfolioStructuredProductOnshoreDailySnapshot';
import { PortfolioTfexDailySnapshot } from 'db/portfolio-summary/models/PortfolioTfexDailySnapshot';
import { PortfolioTfexSummaryDailySnapshot } from 'db/portfolio-summary/models/PortfolioTfexSummaryDailySnapshot';
import { PortfolioThaiEquityDailySnapshot } from 'db/portfolio-summary/models/PortfolioThaiEquityDailySnapshot';
import {
  AccountTypeCondition,
  BondCondition,
  GlobalEquityCondition,
  MutualFundCondition,
  StructuredProductCondition,
  TfexCondition,
  ThaiEquityCashBalanceCondition,
  ThaiEquityCashCondition,
  ThaiEquityCreditBalanceCondition,
} from 'src/constants/account-type-condition';
import { Currencies } from 'src/constants/currencies';
import {
  GroupedProductType,
  PortfolioItem,
  ProductItem,
  TradingAccount,
} from '../process-batch/internal-model';
import {
  PortfolioBond,
  PortfolioBondOffshore,
  PortfolioCash,
  PortfolioExchangeRate,
  PortfolioGlobalEquity,
  PortfolioGlobalEquityOtc,
  PortfolioMutualFund,
  PortfolioStructuredProduct,
  PortfolioTfex,
  PortfolioThaiEquity,
} from '../process-batch/response-model';

export async function buildPortfolioItem<T extends Model<PortfolioBase>>(
  model: { new (): T } & typeof Model<PortfolioBase>,
  productTypes: GroupedProductType[],
  lastDate: string,
  accountTypeCondition: AccountTypeCondition
): Promise<PortfolioItem<T>> {
  const tradingAccounts = productTypes
    .filter(
      (productType) =>
        (accountTypeCondition.accountCondition.length === 0 ||
          accountTypeCondition.accountCondition.some(
            (filter) =>
              productType.exchangeMarketId == filter.exchangeMarketId &&
              productType.accountTypeCode == filter.accountTypeCode &&
              productType.accountType == filter.accountType
          )) &&
        (accountTypeCondition.customerCondition.length === 0 ||
          accountTypeCondition.customerCondition.some(
            (condition) =>
              productType.customerSubType === condition.customerSubType &&
              condition.customerType.includes(productType.customerType)
          ))
    )
    .flatMap((productType) => productType.tradingAccounts);

  let resultSets: PortfolioItem<T> = {
    products: [],
    dateKey: null,
  };
  if (tradingAccounts.length != 0) {
    resultSets = (await buildProduct(
      model,
      tradingAccounts,
      lastDate,
      accountTypeCondition
    )) as PortfolioItem<T>;
  }

  return resultSets;
}

export async function buildThaiEquity(
  productTypes: GroupedProductType[],
  lastDate: string,
  accountTypeCondition: AccountTypeCondition
) {
  const portfolioItem = await buildPortfolioItem(
    PortfolioThaiEquityDailySnapshot,
    productTypes,
    lastDate,
    accountTypeCondition
  );

  const products: PortfolioThaiEquity[] = portfolioItem.products.map(
    (record) => ({
      custcode: record.custcode,
      accountNo: record.tradingAccountNo,
      sharecode: record.sharecode,
      unit: record.unit,
      avgPrice: record.avgPrice,
      marketPrice: record.marketPrice,
      totalCost: record.totalCost,
      marketValue: record.marketValue,
      gainLoss: record.gainLoss,
      dateKey: record.dateKey,
      accountName: accountTypeCondition.accountName,
    })
  );
  return {
    products: products,
    dateKey: portfolioItem.dateKey,
    accountName: accountTypeCondition.accountName,
  };
}

export async function buildMutualFund(
  productTypes: GroupedProductType[],
  lastDate: string,
  accountTypeCondition: AccountTypeCondition
) {
  const portfolioItem = await buildPortfolioItem(
    PortfolioMutualFundDailySnapshot,
    productTypes,
    lastDate,
    accountTypeCondition
  );

  const products: PortfolioMutualFund[] = portfolioItem.products.map(
    (record) => ({
      custcode: record.custcode,
      accountNo: record.tradingAccountNo,
      fundCategory: record.fundCategory,
      amccode: record.amccode,
      fundName: record.fundName,
      navDate: record.navDate,
      unit: record.unit,
      avgNavCost: record.avgNavCost,
      marketNav: record.marketNav,
      totalCost: record.totalCost,
      marketValue: record.marketValue,
      gainLoss: record.gainLoss,
      dateKey: record.dateKey,
      accountName: accountTypeCondition.accountName,
      currency: null,
    })
  );
  return {
    products: products,
    dateKey: portfolioItem.dateKey,
    accountName: accountTypeCondition.accountName,
  };
}

export async function buildMutualFundOffshore(
  productTypes: GroupedProductType[],
  lastDate: string,
  exchangeRates: PortfolioExchangeRate[],
  accountTypeCondition: AccountTypeCondition
) {
  const portfolioItem = await buildPortfolioItem(
    PortfolioMutualFundDailySnapshot,
    productTypes,
    lastDate,
    accountTypeCondition
  );
  const products: PortfolioMutualFund[] = portfolioItem.products.map(
    (record) => {
      const exchangeRate = findExchangeRate(exchangeRates, record.currency);

      return {
        custcode: record.custcode,
        accountNo: record.tradingAccountNo,
        fundCategory: record.fundCategory,
        amccode: record.amccode,
        fundName: record.fundName,
        navDate: record.navDate,
        unit: record.unit,
        avgNavCost: record.avgNavCost,
        marketNav: record.marketNav,
        totalCost: record.totalCost * exchangeRate,
        marketValue: record.marketValue * exchangeRate,
        gainLoss: record.gainLoss * exchangeRate,
        dateKey: record.dateKey,
        accountName: accountTypeCondition.accountName,
        currency: record.currency,
      };
    }
  );
  return {
    products: products,
    dateKey: portfolioItem.dateKey,
    accountName: accountTypeCondition.accountName,
  };
}

export async function buildBond(
  productTypes: GroupedProductType[],
  lastDate: string,
  accountTypeCondition: AccountTypeCondition
) {
  const portfolioItem = await buildPortfolioItem(
    PortfolioBondDailySnapshot,
    productTypes,
    lastDate,
    {
      accountCondition: [],
      customerCondition: [],
      accountName: 'Bond to filter', //As bond data is only on single table, no need to filter by account type, this to absorb case where bond not has account
    }
  );

  const products: PortfolioBond[] = portfolioItem.products.map((record) => ({
    custcode: record.custcode,
    accountNo: record.tradingAccountNo,
    marketType: record.marketType,
    assetName: record.assetName,
    issuer: record.issuer,
    maturityDate: record.maturityDate
      ? new Date(record.maturityDate).toLocaleDateString('en-GB')
      : '-',
    tenureDays: record.maturityDate
      ? new Intl.NumberFormat('en-GB').format(
          dayjs(record.maturityDate).diff(dayjs(record.initialDate), 'day')
        )
      : '-',
    initialDate: record.initialDate,
    couponRate:
      record.couponRate !== null
        ? (Number(record.couponRate) * 100).toFixed(2) + '%'
        : '-',
    totalCost: record.totalCost,
    marketValue:
      record.marketType == 'Self Custodized' ? 0 : record.marketValue,
    dateKey: record.dateKey,
    accountName: accountTypeCondition.accountName,
  }));

  return {
    products: products.sort((a, b) => a.marketType.localeCompare(b.marketType)),
    dateKey: portfolioItem.dateKey,
    accountName: accountTypeCondition.accountName,
  };
}

//TODO go to bond offshore table
export async function buildBondOffshore(
  productTypes: GroupedProductType[],
  lastDate: string,
  exchangeRates: PortfolioExchangeRate[],
  accountTypeCondition: AccountTypeCondition
) {
  const portfolioItem = await buildPortfolioItem(
    PortfolioBondOffshoreDailySnapshot,
    productTypes,
    lastDate,
    {
      accountCondition: [],
      customerCondition: [],
      accountName: 'Bond to filter', //As bond data is only on single table, no need to filter by account type, this to absorb case where bond not has account
    }
  );

  const products: PortfolioBondOffshore[] = portfolioItem.products.map(
    (record) => {
      const exchange = findExchangeRate(exchangeRates, record.currency);
      const marketValue =
        record.marketType == 'Self Custodized' ? 0 : record.marketValue;
      return {
        custcode: record.custcode,
        accountNo: record.tradingAccountNo,
        marketType: record.marketType,
        assetName: record.assetName,
        issuer: record.issuer,
        maturityDate: record.maturityDate,
        initialDate: record.initialDate,
        nextCallDate: record.nextCallDate,
        couponRate: record.couponRate,
        units: record.units,
        currency: record.currency,
        avgCost: record.avgCost,
        totalCost: record.totalCost,
        marketValueOriginalCurrency: marketValue,
        marketValue: marketValue * exchange,
        dateKey: record.dateKey,
        accountName: accountTypeCondition.accountName,
      };
    }
  );

  return {
    products: products.sort((a, b) => a.marketType.localeCompare(b.marketType)),
    dateKey: portfolioItem.dateKey,
    accountName: accountTypeCondition.accountName,
  };
}

export async function buildTfex(
  productTypes: GroupedProductType[],
  lastDate: string,
  accountTypeCondition: AccountTypeCondition
) {
  const portfolioItem = await buildPortfolioItem(
    PortfolioTfexDailySnapshot,
    productTypes,
    lastDate,
    accountTypeCondition
  );

  const products: PortfolioTfex[] = portfolioItem.products.map((record) => ({
    custcode: record.custcode,
    accountNo: record.tradingAccountNo,
    sharecode: record.sharecode,
    multiplier: record.multiplier,
    unit: record.unit,
    avgPrice: record.avgPrice,
    marketPrice: record.marketPrice,
    totalCost: record.totalCost,
    marketValue: record.marketValue,
    gainLoss: record.gainLoss,
    dateKey: record.dateKey,
    accountName: accountTypeCondition.accountName,
  }));
  return {
    products: products,
    dateKey: portfolioItem.dateKey,
    accountName: accountTypeCondition.accountName,
  };
}

export async function buildStructuredProduct(
  productTypes: GroupedProductType[],
  lastDate: string,
  exchangeRates: PortfolioExchangeRate[],
  accountTypeCondition: AccountTypeCondition
) {
  const portfolioItem = await buildPortfolioItem(
    PortfolioStructuredProductDailySnapshot,
    productTypes,
    lastDate,
    accountTypeCondition
  );

  const products: PortfolioStructuredProduct[] = portfolioItem.products.map(
    (record) => ({
      custcode: record.custcode,
      accountNo: record.tradingAccountNo,
      productType: record.productType,
      issuer: record.issuer,
      note: record.note,
      underlying: record.underlying,
      tradeDate: record.tradeDate,
      maturityDate: record.maturityDate,
      tenor: record.tenor,
      capitalProtection: record.capitalProtection,
      yield: record.yield,
      originalCurrency: record.currency,
      currency: record.currency,
      exchangeRate: record.exchangeRate,
      notionalValue: record.notionalValue,
      marketValueOriginalCurrency: record.marketValue,
      marketValue:
        record.marketValue * findExchangeRate(exchangeRates, record.currency),
      dateKey: record.dateKey,
      accountName: accountTypeCondition.accountName,
    })
  );
  return {
    products: products,
    dateKey: portfolioItem.dateKey,
    accountName: accountTypeCondition.accountName,
  };
}

export async function buildStructuredProductOnshore(
  productTypes: GroupedProductType[],
  lastDate: string,
  exchangeRates: PortfolioExchangeRate[],
  accountTypeCondition: AccountTypeCondition
) {
  const portfolioItem = await buildPortfolioItem(
    PortfolioStructuredProductOnshoreDailySnapshot,
    productTypes,
    lastDate,
    accountTypeCondition
  );

  const products: PortfolioStructuredProduct[] = portfolioItem.products.map(
    (record) => ({
      custcode: record.custcode,
      accountNo: record.tradingAccountNo,
      productType: record.productType,
      issuer: record.issuer,
      note: record.note,
      underlying: record.underlying,
      tradeDate: record.tradeDate,
      maturityDate: record.maturityDate,
      tenor: record.tenor,
      capitalProtection: record.capitalProtection,
      yield: record.yield,
      originalCurrency: record.currency,
      currency: record.currency,
      exchangeRate: record.exchangeRate,
      notionalValue: record.notionalValue,
      marketValueOriginalCurrency: record.marketValue,
      marketValue:
        record.marketValue * findExchangeRate(exchangeRates, record.currency),
      dateKey: record.dateKey,
      accountName: accountTypeCondition.accountName,
    })
  );
  return {
    products: products,
    dateKey: portfolioItem.dateKey,
    accountName: accountTypeCondition.accountName,
  };
}

export async function buildGlobalEquityOtc(
  productTypes: GroupedProductType[],
  lastDate: string,
  exchangeRates: PortfolioExchangeRate[],
  accountTypeCondition: AccountTypeCondition
) {
  const portfolioItem = await buildPortfolioItem(
    PortfolioGlobalEquityOtcDailySnapshot,
    productTypes,
    lastDate,
    accountTypeCondition
  );

  const products: PortfolioGlobalEquityOtc[] = portfolioItem.products.map(
    (record) => {
      const exchange = findExchangeRate(exchangeRates, record.currency);
      return {
        custcode: record.custcode,
        accountNo: record.tradingAccountNo,
        sharecode: record.sharecode,
        equityCategory: record.stockExchangeMarkets,
        originalCurrency: record.currency,
        currency: 'THB',
        units: record.units,
        avgCost: record.avgCost,
        marketPrice: record.marketPrice,
        totalCost: record.totalCost,
        marketValueOriginalCurrency: record.marketValue,
        marketValue: record.marketValue * exchange,
        gainLoss: record.gainLoss,
        dateKey: record.dateKey,
        accountName: accountTypeCondition.accountName,
      };
    }
  );
  return {
    products: products,
    dateKey: portfolioItem.dateKey,
    accountName: accountTypeCondition.accountName,
  };
}

export async function buildGlobalEquity(
  productTypes: GroupedProductType[],
  lastDate: string,
  exchangeRates: PortfolioExchangeRate[],
  accountTypeCondition: AccountTypeCondition
) {
  const portfolioItem = await buildPortfolioItem(
    PortfolioGlobalEquityDailySnapshot,
    productTypes,
    lastDate,
    accountTypeCondition
  );

  const products: PortfolioGlobalEquity[] = portfolioItem.products.map(
    (record) => {
      const exchange = findExchangeRate(exchangeRates, record.currency);
      return {
        custcode: record.custcode,
        accountNo: record.tradingAccountNo,
        equityCategory: record.stockExchangeMarkets,
        sharecode: record.sharecode,
        originalCurrency: record.currency,
        currency: 'THB',
        units: record.units,
        avgCost: record.avgCost,
        marketPrice: record.marketPrice,
        totalCost: record.totalCost,
        marketValueOriginalCurrency: record.marketValue,
        marketValue: record.marketValue * exchange,
        gainLoss: record.gainLoss,
        dateKey: record.dateKey,
        accountName: accountTypeCondition.accountName,
      };
    }
  );
  return {
    products: products,
    dateKey: portfolioItem.dateKey,
    accountName: accountTypeCondition.accountName,
  };
}

export async function buildCash(
  productTypes: GroupedProductType[],
  exchangeRates: PortfolioExchangeRate[],
  productBases: ProductItem[]
) {
  try {
    const accountTypeList = [
      ThaiEquityCashBalanceCondition,
      ThaiEquityCashCondition,
      ThaiEquityCreditBalanceCondition,
      MutualFundCondition,
      BondCondition,
      TfexCondition,
      StructuredProductCondition,
      GlobalEquityCondition,
    ];

    console.log(
      'buildCash(productBases):',
      productBases.map((productBase) => {
        return {
          accountName: productBase.accountName,
          dateKey: productBase.dateKey,
        };
      })
    );

    const result = await Promise.all(
      accountTypeList.map(async (condition) => {
        const lastDate = productBases
          .find(
            (productBase) => productBase.accountName == condition.accountName
          )
          ?.dateKey?.toString();
        if (lastDate == null) return [];

        const portfolioItem = await buildPortfolioItem(
          PortfolioCashDailySnapshot,
          productTypes,
          lastDate,
          condition
        );

        const cashes: PortfolioCash[] = portfolioItem.products.map(
          (record) => ({
            custcode: record.custcode,
            accountNo: record.tradingAccountNo,
            currency: record.currency,
            currencyFullName: Currencies.find(
              (currency) =>
                currency.ISO3.toLowerCase() == record.currency.toLowerCase()
            ).Name,
            cashBalance: record.cashBalance,
            dateKey: record.dateKey,
            marketValue:
              record.cashBalance *
              findExchangeRate(exchangeRates, record.currency),
            accountName: condition.accountName,
          })
        );

        return cashes;
      })
    );

    return result.flat();
  } catch (error) {
    console.error('Error buildCash:', error);
    throw error;
  }
}

export async function buildTfexSummary(
  productTypes: GroupedProductType[],
  lastDate: string,
  accountTypeCondition: AccountTypeCondition
) {
  const result = await buildPortfolioItem(
    PortfolioTfexSummaryDailySnapshot,
    productTypes,
    lastDate,
    accountTypeCondition
  );

  return result;
}

async function buildProduct<T extends Model<PortfolioBase>>(
  model: { new (): T } & typeof Model<PortfolioBase>,
  tradingAccounts: TradingAccount[],
  lastDate: string,
  accountTypeCondition: AccountTypeCondition
): Promise<PortfolioItem<T>> {
  try {
    const customerConditionClauses = accountTypeCondition.customerCondition.map(
      (condition) => ({
        [Op.and]: [
          { customerSubType: condition.customerSubType },
          { customerType: { [Op.in]: condition.customerType } },
        ],
      })
    );

    const accountConditionClauses = accountTypeCondition.accountCondition.map(
      (condition) => ({
        [Op.and]: [
          { exchangeMarketId: condition.exchangeMarketId },
          { accountType: condition.accountType },
          { accountTypeCode: condition.accountTypeCode },
        ],
      })
    );

    console.log(`[DB](3) - ${model.name} buildProduct (1)`);
    const lastDateKeyRecord = await model.findOne({
      attributes: [[Sequelize.fn('MAX', Sequelize.col('date_key')), 'dateKey']],
      where: {
        [Op.and]: [
          {
            dateKey: {
              [Op.lte]: lastDate,
            },
          },
        ],
      },
    });

    const lastDateKey: Date = lastDateKeyRecord
      ? lastDateKeyRecord.dataValues.dateKey
      : null;
    if (!lastDateKey) {
      return {
        products: [],
        dateKey: null,
      };
    }

    const custcodes = [
      ...new Set(tradingAccounts.map((account) => account.custcode)),
    ];
    const tradingAccountNos = [
      ...new Set(
        tradingAccounts
          .map((account) => [
            account.tradingAccountNo, // Original tradingAccountNo
            `${account.custcode}-M`, // TradingAccountNo with '-M' suffix for mutual fund
          ])
          .flat()
      ),
    ];

    console.log(`[DB](3) - ${model.name} buildProduct (2)`);
    const resultSets = await model.findAll({
      where: {
        [Op.and]: [
          {
            custcode: {
              [Op.in]: custcodes,
            },
          },
          {
            tradingAccountNo: {
              [Op.in]: tradingAccountNos,
            },
          },
          {
            ...(customerConditionClauses.length > 0 && {
              [Op.or]: customerConditionClauses,
            }),
          },
          {
            ...(accountConditionClauses.length > 0 && {
              [Op.or]: accountConditionClauses,
            }),
          },
          {
            dateKey: lastDateKey,
          },
        ],
      },
      raw: true,
      logging: (sql: string, timing?: number) => {
        console.log('Raw SQL:', model.name, sql, `(${timing ?? 0``}ms)`);
      },
    });

    return {
      products: resultSets as T[],
      dateKey: lastDateKey,
    };
  } catch (error) {
    console.error('Error buildProduct:', error);
    throw error;
  }
}

function findExchangeRate(
  exchangeRates: PortfolioExchangeRate[],
  currency: string
): number {
  try {
    return currency == 'THB'
      ? 1
      : exchangeRates.find((rate) => rate.currency == currency).exchangeRate;
  } catch (error) {
    console.error('findExchangeRate: ' + currency, error);
    throw error;
  }
}
