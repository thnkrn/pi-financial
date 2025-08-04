import { Op, Sequelize } from 'sequelize';

import { InternalEmployeesResponse } from '@libs/employee-api';
import { InternalCustomerByIdentificationNo } from '@libs/user-v2-api';
import { initModel as initModelPortfolioBondDailySnapshot } from 'db/portfolio-summary/models/PortfolioBondDailySnapshot';
import { initModel as initModelPortfolioBondOffshoreDailySnapshot } from 'db/portfolio-summary/models/PortfolioBondOffshoreDailySnapshot';
import { initModel as initModePortfolioCashDailySnapshot } from 'db/portfolio-summary/models/PortfolioCashDailySnapshot';
import {
  initModel as initModelPortfolioExchangeRateDailySnapshot,
  PortfolioExchangeRateDailySnapshot,
} from 'db/portfolio-summary/models/PortfolioExchangeRateDailySnapshot';
import { initModel as initModelPortfolioGlobalEquityDailySnapshot } from 'db/portfolio-summary/models/PortfolioGlobalEquityDailySnapshot';
import { initModel as initModelPortfolioGlobalEquityOtcDailySnapshot } from 'db/portfolio-summary/models/PortfolioGlobalEquityOtcDailySnapshot';
import { initModel as initModelPortfolioMutualFundDailySnapshot } from 'db/portfolio-summary/models/PortfolioMutualFundDailySnapshot';
import { initModel as initModelPortfolioStructuredProductDailySnapshot } from 'db/portfolio-summary/models/PortfolioStructuredProductDailySnapshot';
import { initModel as initModelPortfolioStructuredProductOnshoreDailySnapshot } from 'db/portfolio-summary/models/PortfolioStructuredProductOnshoreDailySnapshot';
import { initModel as initModelPortfolioSummaryDailySnapshot } from 'db/portfolio-summary/models/PortfolioSummaryDailySnapshot';
import { initModel as initModelPortfolioTfexDailySnapshot } from 'db/portfolio-summary/models/PortfolioTfexDailySnapshot';
import {
  initModel as initModelPortfolioTfexSummaryDailySnapshot,
  PortfolioTfexSummaryDailySnapshot,
} from 'db/portfolio-summary/models/PortfolioTfexSummaryDailySnapshot';
import { initModel as initModelPortfolioThaiEquityDailySnapshot } from 'db/portfolio-summary/models/PortfolioThaiEquityDailySnapshot';
import ejs from 'ejs';
import path from 'path';
import { JSHandle, Page } from 'puppeteer';
import {
  AllAccountTypeConditions,
  BondCondition,
  BondOffshoreCondition,
  GlobalEquityCondition,
  GlobalEquityOtcCondition,
  MutualFundCondition,
  MutualFundOffshoreCondition,
  OTCOffshoreCondition,
  StructuredProductCondition,
  StructuredProductOnshoreCondition,
  TfexCondition,
  ThaiEquityCashBalanceCondition,
  ThaiEquityCashCondition,
  ThaiEquityCreditBalanceCondition,
} from 'src/constants/account-type-condition';
import { PrivateWealthTeamIds } from 'src/constants/marketing-team';
import {
  GroupedMarketingProductType,
  GroupedProductType,
  ProductItem,
} from '../process-batch/internal-model';
import {
  BalanceSummary,
  CustcodeProductMapping,
  CustomerStatementData,
  MarketingCustcodeData,
  PortfolioCash,
  PortfolioExchangeRate,
  ProductAccount,
  ProductBase,
} from '../process-batch/response-model';
import { themeConfig } from '../theme/config';
import {
  buildPortfolioAllocation,
  buildPortfolioSummary,
  nullToZero,
} from './build-portfolio-utils';
import {
  buildBond,
  buildBondOffshore,
  buildCash,
  buildGlobalEquity,
  buildGlobalEquityOtc,
  buildMutualFund,
  buildMutualFundOffshore,
  buildStructuredProduct,
  buildStructuredProductOnshore,
  buildTfex,
  buildTfexSummary,
  buildThaiEquity,
} from './build-product-utils';
import { generatePdfBuffer } from './generate-pdf';
import { concatenateNames } from './process-batch-utils';
import { processData } from './process-data';

export async function buildCustomerStatementData(
  customerInfo: InternalCustomerByIdentificationNo,
  marketingInfos: InternalEmployeesResponse,
  groupedMarketings: GroupedMarketingProductType[],
  marketingId: string,
  lastDateToQuery: string,
  sequelize: Sequelize
) {
  initModelPortfolioExchangeRateDailySnapshot(sequelize);
  initModelPortfolioThaiEquityDailySnapshot(sequelize);
  initModelPortfolioMutualFundDailySnapshot(sequelize);
  initModelPortfolioBondDailySnapshot(sequelize);
  initModelPortfolioBondOffshoreDailySnapshot(sequelize);
  initModelPortfolioTfexDailySnapshot(sequelize);
  initModelPortfolioTfexSummaryDailySnapshot(sequelize);
  initModePortfolioCashDailySnapshot(sequelize);
  initModelPortfolioStructuredProductDailySnapshot(sequelize);
  initModelPortfolioGlobalEquityOtcDailySnapshot(sequelize);
  initModelPortfolioGlobalEquityDailySnapshot(sequelize);
  initModelPortfolioSummaryDailySnapshot(sequelize);
  initModelPortfolioStructuredProductOnshoreDailySnapshot(sequelize);

  try {
    const uniqueMarketingIdCustcode = Array.from(
      new Set(
        groupedMarketings
          .filter((marketing) =>
            marketingId === '' ? true : marketing.marketingId === marketingId
          )
          .flatMap((marketing) =>
            marketing.productTypes.flatMap((product) =>
              product.tradingAccounts.map(
                (account) => `${marketing.marketingId}-${account.custcode}`
              )
            )
          )
      )
    ).map((uniqueString) => {
      const [marketingId, custcode] = uniqueString.split('-');
      return { marketingId, custcode };
    });

    const exchangeRate = await buildExchangeRate(lastDateToQuery);

    const marketings: MarketingCustcodeData[] = await Promise.all(
      groupedMarketings
        .filter((marketing) =>
          marketingId == '' ? true : marketing.marketingId == marketingId
        )
        .map(async (marketing) => {
          const uniqueMarketingIdCustcode = Array.from(
            new Set(
              marketing.productTypes.flatMap((product) =>
                product.tradingAccounts.map(
                  (account) => `${marketing.marketingId}-${account.custcode}`
                )
              )
            )
          ).map((uniqueString) => {
            const [marketingId, custcode] = uniqueString.split('-');
            return { marketingId, custcode };
          });

          const marketingInfo = marketingInfos.data.find(
            (marketingInfo) => marketingInfo.id == marketing.marketingId
          );

          const thaiEquity = await buildAggregatedThaiEquity(
            marketing.productTypes,
            lastDateToQuery
          );
          const mutualFund = await buildMutualFund(
            marketing.productTypes,
            lastDateToQuery,
            MutualFundCondition
          );
          const mutualFundOffshore = await buildMutualFundOffshore(
            marketing.productTypes,
            lastDateToQuery,
            exchangeRate,
            MutualFundOffshoreCondition
          );
          const bond = await buildBond(
            marketing.productTypes,
            lastDateToQuery,
            BondCondition
          );
          const bondOffshore = await buildBondOffshore(
            marketing.productTypes,
            lastDateToQuery,
            exchangeRate,
            BondOffshoreCondition
          );
          const tfex = await buildTfex(
            marketing.productTypes,
            lastDateToQuery,
            TfexCondition
          );
          const structedProduct = await buildStructuredProduct(
            marketing.productTypes,
            lastDateToQuery,
            exchangeRate,
            StructuredProductCondition
          );
          const structedProductOnshore = await buildStructuredProductOnshore(
            marketing.productTypes,
            lastDateToQuery,
            exchangeRate,
            StructuredProductOnshoreCondition
          );
          const globalEquityOtc = await buildGlobalEquityOtc(
            marketing.productTypes,
            lastDateToQuery,
            exchangeRate,
            GlobalEquityOtcCondition
          );
          const globalEquity = await buildGlobalEquity(
            marketing.productTypes,
            lastDateToQuery,
            exchangeRate,
            GlobalEquityCondition
          );
          const tfexSummary = await buildTfexSummary(
            marketing.productTypes,
            lastDateToQuery,
            TfexCondition
          );

          const products: ProductItem[] = [
            ...thaiEquity,
            mutualFund,
            mutualFundOffshore,
            bond,
            tfex,
            structedProduct,
            structedProductOnshore,
            globalEquityOtc,
            globalEquity,
          ];

          const cash = await buildCash(
            marketing.productTypes,
            exchangeRate,
            products
          );
          const balanceSummary = buildBalanceSummary(
            cash,
            tfexSummary.products
          );
          const portfolioAllocation = buildPortfolioAllocation(
            thaiEquity.flatMap((item) => item.products),
            mutualFund.products,
            mutualFundOffshore.products,
            bond.products,
            bondOffshore.products,
            tfex.products,
            cash,
            structedProduct.products,
            structedProductOnshore.products,
            globalEquityOtc.products,
            globalEquity.products,
            balanceSummary
          );

          const marketingCustcodeData: MarketingCustcodeData = {
            marketingId: marketing.marketingId,
            marketingName: marketingInfo.nameTh,
            marketingPhoneNo: '', //todo find phone number
            portfoliosummary: await buildPortfolioSummary(
              uniqueMarketingIdCustcode,
              portfolioAllocation,
              lastDateToQuery
            ),
            custcodeProductMapping: buildCustcodeProductMappings(
              customerInfo,
              marketing.marketingId
            ),

            balanceSummary: balanceSummary,
            privateFundSummary: [],

            thaiEquity: thaiEquity.flatMap((item) => item.products),
            mutualFund: mutualFund.products,
            mutualFundOffshore: mutualFundOffshore.products,
            bond: bond.products,
            bondOffshore: bondOffshore.products,
            tfex: tfex.products,
            cash: cash,
            structuredProduct: structedProduct.products,
            structuredProductOnshore: structedProductOnshore.products,
            globalEquityOtc: globalEquityOtc.products,
            globalEquity: globalEquity.products,
            alternativePrivateEquity: [],
            alternativePrivateCredit: [],
            privateFund: [],
          };
          return marketingCustcodeData;
        })
    );

    const portfolioAllocation = buildPortfolioAllocation(
      marketings.flatMap((marketing) => marketing.thaiEquity),
      marketings.flatMap((marketing) => marketing.mutualFund),
      marketings.flatMap((marketing) => marketing.mutualFundOffshore),
      marketings.flatMap((marketing) => marketing.bond),
      marketings.flatMap((marketing) => marketing.bondOffshore),
      marketings.flatMap((marketing) => marketing.tfex),
      marketings.flatMap((marketing) => marketing.cash),
      marketings.flatMap((marketing) => marketing.structuredProduct),
      marketings.flatMap((marketing) => marketing.structuredProductOnshore),
      marketings.flatMap((marketing) => marketing.globalEquityOtc),
      marketings.flatMap((marketing) => marketing.globalEquity),
      marketings.flatMap((marketing) => marketing.balanceSummary)
    );

    const usedExchangeRate = Array.from(
      new Set(
        marketings.flatMap((marketing) => [
          marketing.cash.flatMap((cash) => cash.currency),
          marketing.structuredProduct.flatMap((s) => s.originalCurrency),
          marketing.globalEquity.flatMap((g) => g.originalCurrency),
          marketing.globalEquityOtc.flatMap((g) => g.originalCurrency),
          marketing.bondOffshore.flatMap((g) => g.currency),
          marketing.mutualFundOffshore.flatMap((g) => g.currency),
        ])
      )
    ).flatMap((result) => result);

    const consolidatedSummary = await buildPortfolioSummary(
      uniqueMarketingIdCustcode,
      portfolioAllocation,
      lastDateToQuery
    );
    console.info('consolidatedSummary', JSON.stringify(consolidatedSummary));
    marketings.forEach((c) => {
      c.cash?.forEach((cashItem) => {
        if (cashItem.accountName === StructuredProductCondition.accountName) {
          cashItem.accountName = OTCOffshoreCondition.accountName;
        }
      });
    });
    const result: CustomerStatementData = {
      thaiId: customerInfo.data.identificationNo,
      customerName: concatenateNames(customerInfo, marketingId),
      customerAddress: '',
      isHighNetWorth: marketingInfos.data.some((x) =>
        PrivateWealthTeamIds.some((id) => x.teamId == id)
      ),
      consolidatedSummary: consolidatedSummary,
      exchangeRate: exchangeRate.filter((exchangeRate) =>
        usedExchangeRate.some(
          (usedExchangeRate) => usedExchangeRate == exchangeRate.currency
        )
      ),
      marketings: marketings,
    };

    return result;
  } catch (error) {
    console.error('Error buildCustomerStatementData:', error);
  }
}

async function buildAggregatedThaiEquity(
  productTypes: GroupedProductType[],
  lastDateToQuery: string
) {
  const equityPromises = [
    buildThaiEquity(
      productTypes,
      lastDateToQuery,
      ThaiEquityCashBalanceCondition
    ),
    buildThaiEquity(productTypes, lastDateToQuery, ThaiEquityCashCondition),
    buildThaiEquity(
      productTypes,
      lastDateToQuery,
      ThaiEquityCreditBalanceCondition
    ),
  ];

  // Wait for all promises to resolve
  const results = await Promise.all(equityPromises);

  return results.flatMap((result) => result);
}

function buildCustcodeProductMappings(
  customerInfo: InternalCustomerByIdentificationNo,
  marketingId: string
): CustcodeProductMapping[] {
  try {
    const products: ProductBase[] = customerInfo.data.marketings
      .find((customerMarketing) => customerMarketing.marketingId == marketingId)
      .custCodes.flatMap((custCode) =>
        custCode.tradingAccounts.flatMap((tradingAccount) => {
          const accountType = AllAccountTypeConditions.find(
            (condition) =>
              (condition.accountCondition.length === 0 ||
                condition.accountCondition.some(
                  (filter) =>
                    tradingAccount.exchangeMarketId ==
                      filter.exchangeMarketId &&
                    tradingAccount.accountTypeCode == filter.accountTypeCode &&
                    tradingAccount.accountType == filter.accountType
                )) &&
              (condition.customerCondition.length === 0 ||
                condition.customerCondition.some(
                  (filter) =>
                    custCode.customerSubType === filter.customerSubType &&
                    filter.customerType.includes(custCode.customerType)
                ))
          );
          if (!accountType) {
            console.log('Could not find account type', tradingAccount);
            return [];
          }

          const result: ProductBase = {
            custcode: custCode.custCode,
            accountNo: tradingAccount.tradingAccountNo,
            accountName: accountType.accountName,
          };
          return [result];
        })
      );

    const custcodeMap: { [key: string]: Set<string> } = {};
    const productMap: { [key: string]: Set<ProductAccount> } = {};

    products.forEach((product) => {
      const { custcode, accountNo, accountName } = product;

      if (!custcodeMap[custcode]) {
        custcodeMap[custcode] = new Set();
        productMap[custcode] = new Set();
      }

      const productKey = `${accountNo}-${accountName}`;

      if (!custcodeMap[custcode].has(productKey)) {
        custcodeMap[custcode].add(productKey);
        let accountNameForDisplay = accountName;
        if (
          accountName === 'Cash Balance' ||
          accountName === 'Cash' ||
          accountName === 'Credit Balance'
        ) {
          accountNameForDisplay = `Thai Equity (${accountName})`;
        }
        productMap[custcode].add({
          accountNo: accountNo,
          accountName: accountNameForDisplay,
        });
      }
    });

    return Object.keys(custcodeMap).map((custcode) => ({
      custcode,
      products: Array.from(productMap[custcode]),
    }));
  } catch (error) {
    console.error('Error buildCustcodeProductMappings:', error);
    throw error;
  }
}

function buildBalanceSummary(
  cashes: PortfolioCash[],
  tfexSummary: PortfolioTfexSummaryDailySnapshot[]
): BalanceSummary[] {
  const grouped = cashes.reduce((acc, item) => {
    if (!acc[item.custcode]) {
      acc[item.custcode] = {
        custcode: item.custcode,
        thaiEquityCash: 0,
        thaiEquityCashBalance: 0,
        thaiEquityCreditBalance: 0,
        tfex: 0,
        tfexEquityValue: 0,
        tfexExcessEquity: 0,
        structureProduct: 0,
        globalEquity: 0,
      };
    }

    switch (item.accountName) {
      case ThaiEquityCashCondition.accountName:
        acc[item.custcode].thaiEquityCash += parseFloat(
          nullToZero(item.marketValue).toString()
        );
        break;
      case ThaiEquityCashBalanceCondition.accountName:
        acc[item.custcode].thaiEquityCashBalance += parseFloat(
          nullToZero(item.marketValue).toString()
        );
        break;
      case ThaiEquityCreditBalanceCondition.accountName:
        acc[item.custcode].thaiEquityCreditBalance += parseFloat(
          nullToZero(item.marketValue).toString()
        );
        break;
      case TfexCondition.accountName:
        acc[item.custcode].tfex += parseFloat(
          nullToZero(item.marketValue).toString()
        );
        break;
      case StructuredProductCondition.accountName:
        acc[item.custcode].structureProduct += parseFloat(
          nullToZero(item.cashBalance).toString()
        );
        break;
      case GlobalEquityCondition.accountName:
        acc[item.custcode].globalEquity += parseFloat(
          nullToZero(item.cashBalance).toString()
        );
        break;
      default:
        break;
    }

    return acc;
  }, {} as { [key: string]: BalanceSummary });

  tfexSummary.forEach((tfex) => {
    if (!grouped[tfex.custcode]) {
      grouped[tfex.custcode] = {
        custcode: tfex.custcode,
        thaiEquityCash: 0,
        thaiEquityCashBalance: 0,
        thaiEquityCreditBalance: 0,
        tfex: 0,
        tfexEquityValue: parseFloat(nullToZero(tfex.equity).toString()),
        tfexExcessEquity: parseFloat(nullToZero(tfex.excessEquity).toString()),
        structureProduct: 0,
        globalEquity: 0,
      };
    } else {
      const item = grouped[tfex.custcode];
      item.tfexEquityValue += parseFloat(nullToZero(tfex.equity).toString());
      item.tfexExcessEquity += parseFloat(
        nullToZero(tfex.excessEquity).toString()
      );
    }
  });

  return Object.values(grouped);
}

export async function buildExchangeRate(
  lastDate: string
): Promise<PortfolioExchangeRate[]> {
  try {
    console.log(`[DB](1) - PortfolioExchangeRateDailySnapshot`);
    // Find the last dateKey
    const lastDateKeyRecord = await PortfolioExchangeRateDailySnapshot.findOne({
      attributes: [[Sequelize.fn('MAX', Sequelize.col('date_key')), 'dateKey']],
      where: {
        dateKey: {
          [Op.lte]: lastDate,
        },
      },
    });

    const lastDateKey = lastDateKeyRecord ? lastDateKeyRecord.dateKey : null;
    if (!lastDateKey) {
      throw new Error('No date_key found in the database.');
    }

    console.log(`[DB](2) - PortfolioExchangeRateDailySnapshot`);
    // Retrieve all rows with the last dateKey
    const resultSets = await PortfolioExchangeRateDailySnapshot.findAll({
      where: {
        dateKey: lastDateKey,
      },
      raw: true,
    });

    const result: PortfolioExchangeRate[] = resultSets.map((record) => ({
      currency: record.currency,
      exchangeRate: record.exchangeRate,
      dateKey: record.dateKey,
    }));
    return result;
  } catch (error) {
    console.error('Error fetching records:', error);
    throw error;
  }
}

export async function renderPDFBuffer(
  customerStatementData: CustomerStatementData,
  isOnDemand: boolean
) {
  const processedData = await processData(customerStatementData);

  const html = await ejs.renderFile(
    path.join('resources', 'atlas', 'views', 'portfolio', 'layout.ejs'),
    {
      themeConfig,
      data: processedData,
      isOnDemand: isOnDemand,
    }
  );

  const customDimensions = {
    width: '842px',
    height: '595px',
  };

  const pdfBuffer = await generatePdfBuffer(html, customDimensions, waitUntil);

  return pdfBuffer;
}

async function waitUntil(
  page: Page
): Promise<JSHandle<false> | JSHandle<true>> {
  page.setDefaultTimeout(360000);
  return page.waitForFunction(() => {
    console.log(document, 'document');

    const portfolioCharts = document.querySelectorAll(
      '.portfolioCharts'
    ) as NodeListOf<HTMLCanvasElement>;
    const portfolioChartAlloction = document.querySelectorAll(
      '.portfolioChartAlloction'
    ) as NodeListOf<HTMLCanvasElement>;

    let isChartReady = true;
    portfolioCharts.forEach((chart) => {
      const chartReady =
        chart &&
        chart.getContext('2d') &&
        chart.getContext('2d').canvas.width > 0;
      isChartReady = isChartReady && chartReady == true;
    });
    portfolioChartAlloction.forEach((chart) => {
      const chartReady =
        chart &&
        chart.getContext('2d') &&
        chart.getContext('2d').canvas.width > 0;
      isChartReady = isChartReady && chartReady == true;
    });

    const image = document.querySelector(
      '.image-inner-theme'
    ) as HTMLImageElement;
    const imageReady = image && image.complete && image.naturalWidth > 0;
    return imageReady && isChartReady;
  });
}
