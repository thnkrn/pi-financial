import { DATEONLY } from 'sequelize';
import {
  CustomerStatementData,
  PortfolioSummary,
} from '../process-batch/response-model';

export function mockCustomerStatementData(): CustomerStatementData {
  interface marketingCustcode {
    marketingId: string;
    custcode: string[];
  }

  const customer: marketingCustcode[] = [
    {
      marketingId: 'marketing1',
      custcode: ['custcode1', 'custcode2'],
    },
    {
      marketingId: 'marketing2',
      custcode: ['custcode9', 'custcode9', 'custcode9', 'custcode9'],
    },
  ];

  const toMockCustcode = ['custcode1', 'custcode2'];

  function portfolioSummary(customer: marketingCustcode[]): PortfolioSummary {
    return {
      custcode: customer.flatMap((c) => c.custcode),
      allocationAmount: 1000000,
      yearOnYear: -1000000,
      y_0: 2000000,
      y_1: 2000000,
      y_2: 2500000,
      y_3: 2000000,
      m_0: 1000000,
      m_1: 20000000000,
      m_2: 3000000000,
      m_3: 400000000,
      m_4: 100000000,
      m_5: 10000000,
      m_6: 1500000,
      m_7: 1000000,
      m_8: 500000,
      m_9: 100,
      m_10: 20000,
      m_11: 1500,
      summaryAsOf: '2024-01-01',
      portfolioAllocation: range(1, 10).map((i) => ({
        productName: `Product ${i}`,
        percentAllocation: 10,
        valueAllocation: 100000,
      })),
      latestMonthIndex: 11,
      latestYearIndex: 2,
    };
  }

  const date: Date = new DATEONLY() as unknown as Date;

  const result: CustomerStatementData = {
    thaiId: '1234567890123',
    customerName: 'Mock Customer',
    customerAddress: 'Mock Address',
    isHighNetWorth: true,
    consolidatedSummary: portfolioSummary(customer),
    exchangeRate: range(1, 3).map((i) => ({
      currency: 'USD',
      exchangeRate: 1,
      dateKey: new Date(),
    })),
    marketings: customer.map((c) => ({
      marketingId: c.marketingId,
      marketingName: `FIRSTNAME LASTNAME ${c.marketingId}`,
      marketingPhoneNo: '0812345678',
      portfoliosummary: portfolioSummary([c]),
      custcodeProductMapping: c.custcode.map((custcode) => ({
        custcode,
        products: range(1, parseInt(custcode.substring('custcode'.length))).map(
          (i) => ({
            accountName: `Account Name ${i}`,
            accountNo: `Account No ${i}`,
          })
        ),
      })),

      balanceSummary: c.custcode.map((custcode) => ({
        custcode: custcode,
        thaiEquityCash: 1000 * Math.random(),
        thaiEquityCashBalance: 1000 * Math.random(),
        thaiEquityCreditBalance: 1000 * Math.random(),
        tfex: 1000 * Math.random(),
        tfexEquityValue: 1000 * Math.random(),
        tfexExcessEquity: 1000 * Math.random(),
        structureProduct: 100 * Math.random(),
        globalEquity: 1000 * Math.random(),
      })),
      privateFundSummary: c.custcode.map((custcode) => ({
        custcode: custcode,
        accountName: 'Private Fund A',
        nav: 100 * Math.random(),
      })),

      thaiEquity: c.custcode.flatMap((custcode) =>
        !toMockCustcode.includes(custcode)
          ? []
          : range(1, 2).flatMap((accountName) => {
              return range(1, Math.ceil(getRandomValue(1, 20))).map((i) => {
                const totalCost = getRandomValue(1, 5000);
                const gainLoss = getRandomValue(
                  -0.1 * totalCost,
                  0.1 * totalCost
                );
                const isGain = Math.random() < 0.5;
                const marketValue = isGain
                  ? totalCost + gainLoss
                  : totalCost - gainLoss;
                return {
                  custcode: custcode,
                  accountNo: custcode + '-1',
                  sharecode: `sharecode ${i}`,
                  unit: 25000,
                  avgPrice: 21.341456,
                  marketPrice: 18.7,
                  totalCost: totalCost,
                  marketValue: marketValue,
                  gainLoss: marketValue - totalCost,
                  dateKey: new Date(),
                  accountName: `Account Name ${accountName}`,
                };
              });
            })
      ),
      mutualFund: c.custcode.flatMap((custcode) =>
        !toMockCustcode.includes(custcode)
          ? []
          : range(1, 2).flatMap((category) => {
              return range(1, Math.ceil(getRandomValue(20, 25))).map((i) => {
                const totalCost = getRandomValue(1, 5000);
                const gainLoss = getRandomValue(
                  -0.1 * totalCost,
                  0.1 * totalCost
                );
                const isGain = Math.random() < 0.5;
                const marketValue = isGain
                  ? totalCost + gainLoss
                  : totalCost - gainLoss;
                return {
                  custcode: custcode,
                  accountNo: custcode + '-1',
                  fundCategory: `CATEGORY ${category}`,
                  amccode: `EASTSPRING${i}`,
                  fundName: `PRINCIPAL SET50SSF-SSFX${i}`,
                  navDate: date,
                  unit: 7000.9871,
                  avgNavCost: 14.2837,
                  marketNav: 14.422,
                  totalCost: totalCost,
                  marketValue: marketValue,
                  gainLoss: marketValue - totalCost,
                  dateKey: new Date(),
                  accountName: 'Mutual Fund',
                };
              });
            })
      ),
      bond: c.custcode.flatMap((custcode) =>
        !toMockCustcode.includes(custcode)
          ? []
          : range(1, 2).flatMap((marketType) => {
              return range(1, 10).map((i) => {
                const totalCost = getRandomValue(1, 5000);
                const gainLoss = getRandomValue(
                  -0.1 * totalCost,
                  0.1 * totalCost
                );
                const isGain = Math.random() < 0.5;
                const marketValue = isGain
                  ? totalCost + gainLoss
                  : totalCost - gainLoss;
                return {
                  custcode: custcode,
                  accountNo: custcode + '-1',
                  marketType: `type ${marketType}`,
                  assetName: `asset ${i}`,
                  issuer: `Energy Absolute PCL`,
                  maturityDate: date,
                  initialDate: date,
                  couponRate: 1,
                  totalCost: totalCost,
                  marketValue: marketValue,
                  dateKey: new Date(),
                  accountName: 'Bond',
                };
              });
            })
      ),
      tfex: c.custcode.flatMap((custcode) => {
        return !toMockCustcode.includes(custcode)
          ? []
          : range(1, Math.ceil(getRandomValue(1, 20))).map((i) => {
              const totalCost = getRandomValue(1, 5000);
              const gainLoss = getRandomValue(
                -0.1 * totalCost,
                0.1 * totalCost
              );
              const isGain = Math.random() < 0.5;
              const marketValue = isGain
                ? totalCost + gainLoss
                : totalCost - gainLoss;
              return {
                custcode: custcode,
                accountNo: custcode + '-1',
                sharecode: `sharecode ${i}`,
                multiplier: 1000,
                unit: 5.0,
                avgPrice: 156,
                marketPrice: 137.01,
                totalCost: totalCost,
                marketValue: marketValue,
                gainLoss: marketValue - totalCost,
                dateKey: new Date(),
                accountName: 'TFEX',
              };
            });
      }),
      cash: c.custcode.flatMap((custcode) =>
        !toMockCustcode.includes(custcode)
          ? []
          : range(1, 3).flatMap((currency) => {
              return range(1, Math.ceil(getRandomValue(1, 10))).map((i) => {
                const totalCost = getRandomValue(1, 5000);
                const gainLoss = getRandomValue(
                  -0.1 * totalCost,
                  0.1 * totalCost
                );
                const isGain = Math.random() < 0.5;
                const marketValue = isGain
                  ? totalCost + gainLoss
                  : totalCost - gainLoss;
                return {
                  custcode: custcode,
                  accountNo: custcode + '-1',
                  currency: `US${currency}`,
                  currencyFullName: `Currency Full Name ${currency}`,
                  cashBalance: marketValue - totalCost,
                  marketValue: marketValue - totalCost,
                  dateKey: new Date(),
                  accountName: `Account Name ${i}`,
                };
              });
            })
      ),
      structuredProduct: c.custcode.flatMap((custcode) => {
        return !toMockCustcode.includes(custcode)
          ? []
          : range(1, Math.ceil(getRandomValue(1, 10))).map((i) => {
              const totalCost = getRandomValue(1, 5000);
              const gainLoss = getRandomValue(
                -0.1 * totalCost,
                0.1 * totalCost
              );
              const isGain = Math.random() < 0.5;
              const marketValue = isGain
                ? totalCost + gainLoss
                : totalCost - gainLoss;
              return {
                custcode: custcode,
                accountNo: custcode + '-1',
                productType: `productType ${i}`,
                issuer: `issuer ${i}`,
                note: `note ${i}`,
                underlying: `underlying ${i}`,
                tradeDate: date,
                maturityDate: date,
                tenor: 100,
                capitalProtection: `1`,
                yield: 0.2435,
                originalCurrency: 'THB',
                currency: 'USD',
                notionalValue: totalCost,
                marketValueOriginalCurrency: 102000,
                marketValue: marketValue,
                gainLoss: marketValue - totalCost,
                dateKey: new Date(),
                accountName: 'Structured Product',
              };
            });
      }),
      globalEquityOtc: c.custcode.flatMap((custcode) =>
        !toMockCustcode.includes(custcode)
          ? []
          : range(1, 2).flatMap((category) => {
              return range(1, Math.ceil(getRandomValue(1, 15))).map((i) => {
                const totalCost = getRandomValue(1, 5000);
                const gainLoss = getRandomValue(
                  -0.1 * totalCost,
                  0.1 * totalCost
                );
                const isGain = Math.random() < 0.5;
                const marketValue = isGain
                  ? totalCost + gainLoss
                  : totalCost - gainLoss;
                return {
                  custcode: custcode,
                  accountNo: custcode + '-1',
                  sharecode: `sharecode ${i}`,
                  equityCategory: `category ${category}`,
                  originalCurrency: 'THB',
                  currency: 'USD',
                  units: 6156,
                  avgCost: 420.80934,
                  marketPrice: 422.285864,
                  totalCost: totalCost,
                  marketValue: marketValue,
                  marketValueOriginalCurrency: marketValue * 0.1,
                  gainLoss: marketValue - totalCost,
                  dateKey: new Date(),
                  accountName: `Global Equity OTC`,
                };
              });
            })
      ),
      globalEquity: c.custcode.flatMap((custcode) =>
        !toMockCustcode.includes(custcode)
          ? []
          : range(1, 1).flatMap((category) => {
              return range(1, 13).map((i) => {
                const totalCost = getRandomValue(1, 5000);
                const gainLoss = getRandomValue(
                  -0.1 * totalCost,
                  0.1 * totalCost
                );
                const isGain = Math.random() < 0.5;
                const marketValue = isGain
                  ? totalCost + gainLoss
                  : totalCost - gainLoss;
                return {
                  custcode: custcode,
                  accountNo: custcode + '-1',
                  sharecode: `sharecode ${i}`,
                  equityCategory: `category ${category}`,
                  originalCurrency: 'THB',
                  currency: `USD`,
                  units: 6156,
                  avgCost: 420.80934,
                  marketPrice: 422.285864,
                  totalCost: totalCost,
                  marketValue: marketValue,
                  marketValueOriginalCurrency: marketValue * 0.1,
                  gainLoss: marketValue - totalCost,
                  dateKey: new Date(),
                  accountName: `Global Equity`,
                };
              });
            })
      ),
      alternativePrivateEquity: c.custcode.flatMap((custcode) => {
        return !toMockCustcode.includes(custcode)
          ? []
          : range(1, 10).map((i) => {
              const totalCost = getRandomValue(1, 5000);
              const gainLoss = getRandomValue(
                -0.1 * totalCost,
                0.1 * totalCost
              );
              const isGain = Math.random() < 0.5;
              const marketValue = isGain
                ? totalCost + gainLoss
                : totalCost - gainLoss;
              return {
                custcode: custcode,
                accountNo: custcode + '-1',
                sharecode: `sharecode ${i}`,
                unit: 100,
                avgPrice: 100,
                marketPrice: 100,
                totalCost: totalCost,
                marketValue: marketValue,
                gainLoss: marketValue - totalCost,
                dateKey: new Date(),
                accountName: 'Alternative Private Equity',
              };
            });
      }),
      alternativePrivateCredit: c.custcode.flatMap((custcode) => {
        return !toMockCustcode.includes(custcode)
          ? []
          : range(1, 10).map((i) => {
              const totalCost = getRandomValue(1, 5000);
              const gainLoss = getRandomValue(
                -0.1 * totalCost,
                0.1 * totalCost
              );
              const isGain = Math.random() < 0.5;
              const marketValue = isGain
                ? totalCost + gainLoss
                : totalCost - gainLoss;
              return {
                custcode: custcode,
                accountNo: custcode + '-1',
                assetName: `assetName ${i}`,
                initialDate: date,
                maturityDate: date,
                couponRate: 100,
                nextCouponDate: date,
                initialValue: totalCost,
                marketValue: marketValue,
                gainLoss: marketValue - totalCost,
                dateKey: new Date(),
                accountName: 'Alternative Private Credit',
              };
            });
      }),
      privateFund: c.custcode.flatMap((custcode) => {
        return !toMockCustcode.includes(custcode)
          ? []
          : range(1, 10).map((i) => {
              const totalCost = getRandomValue(1, 5000);
              const gainLoss = getRandomValue(
                -0.1 * totalCost,
                0.1 * totalCost
              );
              const isGain = Math.random() < 0.5;
              const marketValue = isGain
                ? totalCost + gainLoss
                : totalCost - gainLoss;
              return {
                custcode: custcode,
                accountNo: custcode + '-1',
                tickercode: `tickercode ${i}`,
                issuer: `issuer ${i}`,
                type: `type ${i}`,
                weight: '10',
                unit: 6156,
                avgPrice: 420.80934,
                marketPrice: 422.285864,
                totalCost: totalCost,
                marketValue: marketValue,
                gainLoss: gainLoss,
                dateKey: new Date(),
                accountName: 'Private Fund A',
              };
            });
      }),
    })),
  };

  return result;
}

function range(start: number, end: number): number[] {
  return Array.from({ length: end - start + 1 }, (_, i) => start + i);
}

function getRandomValue(min, max) {
  return Math.random() * (max - min) + min;
}
