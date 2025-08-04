import { InternalEmployeesResponse } from '@libs/employee-api';
import { InternalCustomerByIdentificationNo } from '@libs/user-v2-api';
import dayjs from 'dayjs';
import {
  initModel as initModePortfolioCashDailySnapshot,
  PortfolioCashDailySnapshot,
} from 'db/portfolio-summary/models/PortfolioCashDailySnapshot';
import {
  initModel as initModelPortfolioGlobalEquityDailySnapshot,
  PortfolioGlobalEquityDailySnapshot,
} from 'db/portfolio-summary/models/PortfolioGlobalEquityDailySnapshot';
import {
  initModel as initModelPortfolioGlobalEquityDepositwithdraw,
  PortfolioGlobalEquityDepositwithdraw,
} from 'db/portfolio-summary/models/PortfolioGlobalEquityDepositwithdraw';
import {
  initModel as initModelPortfolioGlobalEquityDividend,
  PortfolioGlobalEquityDividend,
} from 'db/portfolio-summary/models/PortfolioGlobalEquityDividend';
import {
  initModel as initModelPortfolioGlobalEquityTrade,
  PortfolioGlobalEquityTrade,
} from 'db/portfolio-summary/models/PortfolioGlobalEquityTrade';
import ejs from 'ejs';
import path from 'path';
import { JSHandle, Page } from 'puppeteer';
import { Op, QueryTypes, Sequelize } from 'sequelize';
import { GlobalEquityCondition } from 'src/constants/account-type-condition';
import { getCustomerAddress } from '../process-batch/handler-helper';
import {
  GEActivityStatement,
  GEDepositwithdraw,
  GEDividend,
  GEOutStandingCashBalance,
  GEStocksAndETF,
  GETrade,
  GroupedMarketingProductType,
} from '../process-batch/internal-model';
import { GlobalEquityGroup } from '../process-batch/response-model';
import { themeGEConfig } from '../theme/config';
import { generatePdfBuffer } from './generate-pdf';

const sideMapping = {
  buy: 'Buy',
  sell: 'Sell',
};
const typeMapping = {
  deposit: 'deposit',
  withdraw: 'withdraw',
  ccyConversion: 'ccy conversion',
};
export async function buildGlobalEquityStatementData(
  marketingId: string,
  groupedMarketings: GroupedMarketingProductType[],
  customerInfo: InternalCustomerByIdentificationNo,
  marketingInfos: InternalEmployeesResponse,
  sequelize: Sequelize,
  dateFrom: Date,
  dateTo: Date,
  custcode: string,
  userId: string
): Promise<GlobalEquityGroup> {
  initModePortfolioCashDailySnapshot(sequelize);
  initModelPortfolioGlobalEquityDailySnapshot(sequelize);
  initModelPortfolioGlobalEquityDepositwithdraw(sequelize);
  initModelPortfolioGlobalEquityTrade(sequelize);
  initModelPortfolioGlobalEquityDividend(sequelize);

  try {
    const filteredMarketing = await filteredMarketingsByProduct(
      groupedMarketings,
      marketingId
    );
    const activityStatement = await buildActivityStatement(
      customerInfo,
      filteredMarketing,
      marketingInfos,
      dateFrom,
      dateTo,
      custcode,
      userId
    );
    const outstandingCashBalance = await buildOutstandingCashBalance(
      dateTo,
      activityStatement.accountNumber
    );
    const stocksAndETFs = await buildStocksAndETFs(
      dateTo,
      activityStatement.accountNumber
    );
    const depositwithdraws = await buildDepositwithdraw(
      dateFrom,
      dateTo,
      activityStatement.accountNumber
    );
    const trades = await buildTrade(
      dateFrom,
      dateTo,
      activityStatement.accountNumber
    );
    const dividends = await buildDividend(
      dateFrom,
      dateTo,
      activityStatement.accountNumber
    );
    return {
      activityStatement,
      outstandingCashBalance,
      stocksAndETFs,
      depositwithdraws,
      trades,
      dividends,
    };
  } catch (error) {
    console.error('Error buildCustomerStatementData:', error);
    throw error;
  }
}

async function filteredMarketingsByProduct(
  groupedMarketings: GroupedMarketingProductType[],
  marketingId: string
) {
  return groupedMarketings
    .filter((marketing) =>
      marketingId === '' ? true : marketing.marketingId === marketingId
    )
    .map((marketing) => {
      const filteredProductTypes = marketing.productTypes.filter(
        (productType) =>
          GlobalEquityCondition.accountCondition.length === 0 ||
          GlobalEquityCondition.accountCondition.some(
            (filter) =>
              productType.exchangeMarketId == filter.exchangeMarketId &&
              productType.accountTypeCode == filter.accountTypeCode &&
              productType.accountType == filter.accountType
          )
      );
      if (filteredProductTypes.length > 0) {
        return { ...marketing, productTypes: filteredProductTypes };
      }
      return null;
    })
    .filter((marketing) => marketing !== null);
}

async function buildActivityStatement(
  customerInfo: InternalCustomerByIdentificationNo,
  groupedMarketings: GroupedMarketingProductType[],
  marketingInfos: InternalEmployeesResponse,
  dateFrom: Date,
  dateTo: Date,
  custcode: string,
  userId: string
): Promise<GEActivityStatement> {
  try {
    const tradingAccounts = groupedMarketings.flatMap((marketing) =>
      marketing.productTypes.flatMap((productType) =>
        productType.tradingAccounts.map((tradingAccount) => ({
          tradingAccountNo: tradingAccount.tradingAccountNo,
          custCode: tradingAccount.custcode,
        }))
      )
    );
    const { tradingAccountNo, custCode } = tradingAccounts.find(
      (acc) => acc.custCode === custcode
    );
    const marketing = customerInfo.data.marketings.find((marketing) =>
      marketing.custCodes.some((custInfo) => custInfo.custCode === custCode)
    );
    const customer = marketing.custCodes.find(
      (custInfo) => custInfo.custCode === custCode
    );
    const { firstnameTh, lastnameTh } = customer.basicInfo.name;
    const marketingName = marketingInfos.data.find(
      (info) => info.id === marketing.marketingId
    )?.nameTh;

    let fullAddress = '';
    try {
      const customerAddress = await getCustomerAddress(userId);

      if (customerAddress) {
        fullAddress = [
          'homeNo',
          'building',
          'village',
          'floor',
          'soi',
          'road',
          'subDistrict',
          'district',
          'province',
          'country',
          'zipCode',
        ]
          .map((key) => customerAddress[key as keyof typeof customerAddress])
          .filter(Boolean)
          .join(' ');
      }
    } catch (error) {
      console.error('Error Get Customer Address:', error);
    }

    return {
      accountNumber: tradingAccountNo,
      accountName: `${firstnameTh} ${lastnameTh}`,
      taxIdNumber: customerInfo.data.identificationNo,
      address: fullAddress,
      marketingOfficer: marketingName,
      period: `${dayjs(dateFrom).format('D MMMM YYYY')} - ${dayjs(
        dateTo
      ).format('D MMMM YYYY')}`,
    };
  } catch (error) {
    console.error('Error fetching records:', error);
    throw error;
  }
}

async function buildStocksAndETFs(
  dateTo: Date,
  tradingAccountNo: string
): Promise<GEStocksAndETF[]> {
  console.log(`[DB](2) - PortfolioGlobalEquityDailySnapshot`);
  try {
    const maxDateRow = await PortfolioGlobalEquityDailySnapshot.findOne({
      attributes: [[Sequelize.fn('MAX', Sequelize.col('date_key')), 'dateKey']],
      where: {
        dateKey: { [Op.lte]: dateTo },
      },
      raw: true,
    });
    const maxDateKey = maxDateRow?.dateKey;
    if (!maxDateKey) return [];
    const accountMaxRow = await PortfolioGlobalEquityDailySnapshot.findOne({
      attributes: [[Sequelize.fn('MAX', Sequelize.col('date_key')), 'dateKey']],
      where: {
        tradingAccountNo,
        dateKey: { [Op.lte]: dateTo },
      },
      raw: true,
    });
    const accountMaxDateKey = accountMaxRow?.dateKey;
    if (!accountMaxDateKey) return [];
    const results = (await PortfolioGlobalEquityDailySnapshot.sequelize.query(
      `
      SELECT 
        sharecode,
        currency,
        units,
        avg_cost AS avgCost,
        market_price AS marketPrice,
        market_value AS marketValue,
        date_key
      FROM portfolio_global_equity_daily_snapshot
      WHERE trading_account_no = :tradingAccountNo
        AND date_key = :accountMaxDateKey
      ORDER BY sharecode;
      `,
      {
        replacements: { tradingAccountNo, accountMaxDateKey },
        type: QueryTypes.SELECT,
      }
    )) as PortfolioGlobalEquityDailySnapshot[];

    const maxDate = new Date(maxDateKey);
    const itemMaxDate = new Date(accountMaxDateKey);
    if (itemMaxDate < maxDate) return [];
    return results.map(
      (item) =>
        ({
          symbol: item.sharecode,
          currency: item.currency,
          quantity: item.units,
          costPrice: item.avgCost ?? 0,
          closePrice: item.marketPrice,
          marketValue: item.marketValue,
        } as GEStocksAndETF)
    );
  } catch (error) {
    console.error('Error buildStocksAndETFs:', error);
    throw error;
  }
}

async function buildDepositwithdraw(
  dateFrom: Date,
  dateTo: Date,
  tradingAccountNo: string
): Promise<GEDepositwithdraw[]> {
  try {
    console.log(`[DB](3) - PortfolioGlobalEquityDepositwithdraw`);
    const results = await PortfolioGlobalEquityDepositwithdraw.findAll({
      where: {
        dateKey: {
          [Op.between]: [dateFrom, dateTo],
        },
        tradingAccountNo: tradingAccountNo,
      },
      order: [['dateKey', 'ASC']],
      raw: true,
    });

    return results.map((result) => {
      const isWithdraw =
        result.type.toLocaleLowerCase() === typeMapping.withdraw;

      return {
        transactionDate: result.dateKey,
        transaction: result.type,
        currency: result.currency,
        amount: isWithdraw ? -result.amountUsd : result.amountUsd,
        fxRate: result.fxRate,
        amountThb: isWithdraw ? -result.amountThb : result.amountThb,
      } as GEDepositwithdraw;
    });
  } catch (error) {
    console.error('Error buildDepositwithdraw:', error);
    throw error;
  }
}

async function buildTrade(
  dateFrom: Date,
  dateTo: Date,
  tradingAccountNo: string
): Promise<GETrade[]> {
  try {
    console.log(`[DB](4) - PortfolioGlobalEquityTrade`);
    const results = await PortfolioGlobalEquityTrade.findAll({
      where: {
        dateKey: {
          [Op.between]: [dateFrom, dateTo],
        },
        tradingAccountNo: tradingAccountNo,
      },
      order: [
        ['dateKey', 'ASC'],
        ['sharecode', 'ASC'],
      ],
      raw: true,
    });

    return results.map((result) => {
      return {
        transactionDate: result.dateKey,
        symbol: result.sharecode,
        side: (result.side = sideMapping[result.side] || result.side),
        currency: result.currency,
        quantity: result.units,
        price: result.avg_price,
        grossAmount: result.gross_amount,
        commission: result.commission_before_vat_usd,
        vat: result.vat_amount,
        otherFee: result.other_fees,
        withholdingTax: result.wh_tax,
        netAmount: result.net_amount,
      } as GETrade;
    });
  } catch (error) {
    console.error('Error buildTrade:', error);
    throw error;
  }
}

async function buildOutstandingCashBalance(
  dateTo: Date,
  tradingAccountNo: string
): Promise<GEOutStandingCashBalance[]> {
  try {
    console.log(`[DB](1) - PortfolioCashDailySnapshot`);
    const maxDateRow = await PortfolioCashDailySnapshot.findOne({
      attributes: [[Sequelize.fn('MAX', Sequelize.col('date_key')), 'dateKey']],
      where: {
        dateKey: { [Op.lte]: dateTo },
      },
      raw: true,
    });
    const maxDateKey = maxDateRow?.dateKey;
    if (!maxDateKey) return [];
    const accountMaxRow = await PortfolioCashDailySnapshot.findOne({
      attributes: [[Sequelize.fn('MAX', Sequelize.col('date_key')), 'dateKey']],
      where: {
        tradingAccountNo,
        dateKey: { [Op.lte]: dateTo },
      },
      raw: true,
    });
    const accountMaxDateKey = accountMaxRow?.dateKey;
    if (!accountMaxDateKey) return [];
    const result = await PortfolioCashDailySnapshot.findAll({
      where: {
        tradingAccountNo,
        dateKey: accountMaxRow.dateKey,
      },
      raw: true,
    });
    const maxDate = new Date(maxDateKey);
    const itemMaxDate = new Date(accountMaxDateKey);
    return result.map((item) => {
      if (itemMaxDate < maxDate) {
        return {
          currency: item.currency,
          balance: 0,
        } as GEOutStandingCashBalance;
      }

      return {
        currency: item.currency,
        balance: item.cashBalance,
      } as GEOutStandingCashBalance;
    });
  } catch (error) {
    console.error('Error getCashDailySnapshot:', error);
    throw error;
  }
}

async function buildDividend(
  dateFrom: Date,
  dateTo: Date,
  tradingAccountNo: string
): Promise<GEDividend[]> {
  try {
    console.log(`[DB](5) - PortfolioGlobalEquityDividend`);
    const results = await PortfolioGlobalEquityDividend.findAll({
      where: {
        dateKey: {
          [Op.between]: [dateFrom, dateTo],
        },
        tradingAccountNo: tradingAccountNo,
      },
      order: [
        ['dateKey', 'ASC'],
        ['sharecode', 'ASC'],
      ],
      raw: true,
    });
    return results.map((result) => {
      return {
        transactionDate: result.dateKey,
        symbol: result.sharecode,
        currency: result.currency,
        quantity: result.units,
        dividendPerShare: result.dividen_per_share,
        amount: result.amount,
        taxAmount: result.tax_amount,
        netAmount: result.net_amount_usd,
        fxRate: result.fx_rate,
      } as GEDividend;
    });
  } catch (error) {
    console.error('Error buildDividend:', error);
    throw error;
  }
}

export async function renderGEPDFBuffer(data: GlobalEquityGroup) {
  const html = await ejs.renderFile(
    path.join('resources', 'atlas', 'views', 'global-equity', 'layout.ejs'),
    { themeGEConfig, data: data }
  );

  const customDimensions = {
    width: '1123px',
    height: '794px',
  };
  const pdfBuffer = await generatePdfBuffer(html, customDimensions, waitUntil);

  return pdfBuffer;
}

async function waitUntil(
  page: Page
): Promise<JSHandle<false> | JSHandle<true>> {
  return page.waitForFunction(async () => {
    return true;
  });
}
