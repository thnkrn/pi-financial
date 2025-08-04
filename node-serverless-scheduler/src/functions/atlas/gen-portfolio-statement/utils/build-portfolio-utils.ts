import { PortfolioSummaryDailySnapshot } from 'db/portfolio-summary/models/PortfolioSummaryDailySnapshot';
import { Op, Sequelize } from 'sequelize';
import { MarketingCustcode } from '../process-batch/internal-model';
import {
  BalanceSummary,
  PortfolioAllocation,
  PortfolioBond,
  PortfolioBondOffshore,
  PortfolioCash,
  PortfolioGlobalEquity,
  PortfolioGlobalEquityOtc,
  PortfolioMutualFund,
  PortfolioStructuredProduct,
  PortfolioSummary,
  PortfolioTfex,
  PortfolioThaiEquity,
} from '../process-batch/response-model';

// Function to convert null to zero
export const nullToZero = (value: number | null): number => {
  return value !== null ? value : 0;
};
export function buildPortfolioAllocation(
  thaiEquity: PortfolioThaiEquity[],
  mutualFund: PortfolioMutualFund[],
  mutualFundOffshore: PortfolioMutualFund[],
  bond: PortfolioBond[],
  bondOffshore: PortfolioBondOffshore[],
  tfex: PortfolioTfex[],
  cash: PortfolioCash[],
  structuredProduct: PortfolioStructuredProduct[],
  structuredProductOnshore: PortfolioStructuredProduct[],
  globalEquityOtc: PortfolioGlobalEquityOtc[],
  globalEquity: PortfolioGlobalEquity[],
  tfexSummary: BalanceSummary[]
) {
  const thaiSum: number = thaiEquity.reduce(
    (sum, value) => sum + parseFloat(nullToZero(value.marketValue).toString()),
    0
  );
  const mutualFundSum: number = mutualFund.reduce(
    (sum, value) => sum + parseFloat(nullToZero(value.marketValue).toString()),
    0
  );
  const mutualFundOffshoreSum: number = mutualFundOffshore.reduce(
    (sum, value) => sum + parseFloat(nullToZero(value.marketValue).toString()),
    0
  );
  const bondSum: number = bond.reduce(
    (sum, value) => sum + parseFloat(nullToZero(value.marketValue).toString()),
    0
  );
  const bondOffshoreSum: number = bondOffshore.reduce(
    (sum, value) => sum + parseFloat(nullToZero(value.marketValue).toString()),
    0
  );
  const tfexSum: number = tfexSummary.reduce(
    (sum, value) =>
      sum + parseFloat(nullToZero(value.tfexEquityValue).toString()),
    0
  );
  const cashSum: number = cash.reduce(
    (sum, value) => sum + parseFloat(nullToZero(value.marketValue).toString()),
    0
  );
  const structuredProductSum: number = structuredProduct.reduce(
    (sum, value) => sum + parseFloat(nullToZero(value.marketValue).toString()),
    0
  );
  const structuredProductOnshoreSum: number = structuredProductOnshore.reduce(
    (sum, value) => sum + parseFloat(nullToZero(value.marketValue).toString()),
    0
  );
  const globalEquityOtcSum: number = globalEquityOtc.reduce(
    (sum, value) => sum + parseFloat(nullToZero(value.marketValue).toString()),
    0
  );
  const globalEquitySum: number = globalEquity.reduce(
    (sum, value) => sum + parseFloat(nullToZero(value.marketValue).toString()),
    0
  );
  const total =
    thaiSum +
    mutualFundSum +
    mutualFundOffshoreSum +
    bondSum +
    bondOffshoreSum +
    tfexSum +
    cashSum +
    structuredProductSum +
    structuredProductOnshoreSum +
    globalEquityOtcSum +
    globalEquitySum;
  const isEmpty = total == 0;

  const portfolioAllocations: PortfolioAllocation[] = [
    {
      productName: 'Thai Equity',
      percentAllocation: isEmpty
        ? 0
        : parseFloat(((thaiSum / total) * 100).toFixed(2)),
      valueAllocation: thaiSum,
    },
    {
      productName: 'Mutual Funds',
      percentAllocation: isEmpty
        ? 0
        : parseFloat(((mutualFundSum / total) * 100).toFixed(2)),
      valueAllocation: mutualFundSum,
    },
    {
      productName: 'Mutual Funds (Offshore)',
      percentAllocation: isEmpty
        ? 0
        : parseFloat(((mutualFundOffshoreSum / total) * 100).toFixed(2)),
      valueAllocation: mutualFundOffshoreSum,
    },
    {
      productName: 'Bonds & Debentures',
      percentAllocation: isEmpty
        ? 0
        : parseFloat((((bondSum + bondOffshoreSum) / total) * 100).toFixed(2)),
      valueAllocation: bondSum + bondOffshoreSum,
    },
    {
      productName: 'TFEX',
      percentAllocation: isEmpty
        ? 0
        : parseFloat(((tfexSum / total) * 100).toFixed(2)),
      valueAllocation: tfexSum,
    },
    {
      productName: 'Cash (Available Balance)',
      percentAllocation: isEmpty
        ? 0
        : parseFloat(((cashSum / total) * 100).toFixed(2)),
      valueAllocation: cashSum,
    },
    {
      productName: 'Structured Products',
      percentAllocation: isEmpty
        ? 0
        : parseFloat(
            (
              ((structuredProductSum + structuredProductOnshoreSum) / total) *
              100
            ).toFixed(2)
          ),
      valueAllocation: structuredProductSum + structuredProductOnshoreSum,
    },
    {
      productName: 'Global Equity',
      percentAllocation: isEmpty
        ? 0
        : parseFloat(
            (((globalEquitySum + globalEquityOtcSum) / total) * 100).toFixed(2)
          ),
      valueAllocation: globalEquitySum + globalEquityOtcSum,
    },
  ];
  return portfolioAllocations;
}

export async function buildPortfolioSummary(
  marketingCustcode: MarketingCustcode[],
  portfolioAllocation: PortfolioAllocation[],
  lastDate: string
): Promise<PortfolioSummary> {
  try {
    console.log(
      `[DB](4) - PortfolioSummaryDailySnapshot buildPortfolioSummary(1)`
    );
    const lastDateKeyRecord = await PortfolioSummaryDailySnapshot.findOne({
      attributes: [[Sequelize.fn('MAX', Sequelize.col('date_key')), 'dateKey']],
      where: {
        dateKey: {
          [Op.lte]: lastDate,
        },
      },
    });
    const lastDateKey = lastDateKeyRecord
      ? lastDateKeyRecord.get('dateKey')
      : null;

    let resultSets = [];
    if (!lastDateKey) {
      console.error(
        "Couldn't find lastDateKey for buildPortfolioSummary: " +
          JSON.stringify(marketingCustcode)
      );
    } else {
      console.log(
        `[DB](4) - PortfolioSummaryDailySnapshot buildPortfolioSummary(2)`
      );
      resultSets = await PortfolioSummaryDailySnapshot.findAll({
        where: {
          [Op.or]: marketingCustcode.map((account) => ({
            custcode: account.custcode,
            mktid: account.marketingId,
          })),
          dateKey: lastDateKey,
        },
        raw: true,
      });
    }

    const allocationAmount: number = portfolioAllocation.reduce(
      (sum, allocation) => sum + allocation.valueAllocation,
      0
    );
    const result: PortfolioSummary = resultSets.reduce(
      (acc, item) => {
        // Summing up fields
        const yearKeys = ['y_1', 'y_2', 'y_3'];
        yearKeys.forEach((yearKey, index) => {
          acc[yearKey] += parseFloat(nullToZero(item[yearKey]).toString());
          if (item[yearKey] !== null) {
            acc.latestYearIndex = Math.max(acc.latestYearIndex, index + 1);
          }
        });

        const monthKeys = [
          'm_1',
          'm_2',
          'm_3',
          'm_4',
          'm_5',
          'm_6',
          'm_7',
          'm_8',
          'm_9',
          'm_10',
          'm_11',
        ];
        monthKeys.forEach((monthKey, index) => {
          acc[monthKey] += parseFloat(nullToZero(item[monthKey]).toString());
          if (item[monthKey] !== null) {
            acc.latestMonthIndex = Math.max(acc.latestMonthIndex, index + 1);
          }
        });

        // Collecting unique custcode and marketingId
        if (!acc.custcode.includes(item.custcode)) {
          acc.custcode.push(item.custcode);
        }

        return acc;
      },
      {
        custcode: [
          ...new Set(marketingCustcode.map((account) => account.custcode)),
        ],
        allocationAmount: allocationAmount,
        yearOnYear: 0,
        y_0: allocationAmount,
        y_1: 0,
        y_2: 0,
        y_3: 0,
        m_0: allocationAmount,
        m_1: 0,
        m_2: 0,
        m_3: 0,
        m_4: 0,
        m_5: 0,
        m_6: 0,
        m_7: 0,
        m_8: 0,
        m_9: 0,
        m_10: 0,
        m_11: 0,
        summaryAsOf: lastDateKey,
        portfolioAllocation: portfolioAllocation,
        latestYearIndex: 0,
        latestMonthIndex: 1,
      }
    );
    result.yearOnYear = result.y_0 - result.y_1;
    return result;
  } catch (error) {
    console.error('Error in buildPortfolioSummary', error);
    throw error;
  }
}
