import { InternalCustomerByIdentificationNo } from '@libs/user-v2-api';
import { initModel as initModePortfolioCashDailySnapshot } from 'db/portfolio-summary/models/PortfolioCashDailySnapshot';
import { initModel as initModelPortfolioExchangeRateDailySnapshot } from 'db/portfolio-summary/models/PortfolioExchangeRateDailySnapshot';
import {
  initModel as initModelStructureNotesCashMovement,
  StructureNotesCashMovement,
} from 'db/portfolio-summary/models/StructureNotesCashMovement';
import ejs from 'ejs';
import path from 'path';
import { JSHandle, Page } from 'puppeteer';
import { Op, Sequelize } from 'sequelize';
import { StructuredProductCondition } from 'src/constants/account-type-condition';
import { Currencies } from 'src/constants/currencies';
import {
  DateKeyItem,
  GroupedMarketingProductType,
} from '../process-batch/internal-model';
import {
  PortfolioCash,
  SNCashMovement,
  SNCashMovementCustcodeGroup,
} from '../process-batch/response-model';
import { themeSNConfig } from '../theme/config';
import { buildExchangeRate } from './build-customer-statement-utils';
import { nullToZero } from './build-portfolio-utils';
import { buildCash } from './build-product-utils';
import { generatePdfBuffer } from './generate-pdf';

export async function buildSNCashMovementData(
  groupedMarketings: GroupedMarketingProductType[],
  lastDate: string,
  daysToQuery: number,
  customerInfo: InternalCustomerByIdentificationNo,
  sequelize: Sequelize
): Promise<SNCashMovementCustcodeGroup[]> {
  initModelStructureNotesCashMovement(sequelize);
  initModePortfolioCashDailySnapshot(sequelize);
  initModelPortfolioExchangeRateDailySnapshot(sequelize);

  try {
    const snCashMovement = await buildSNCashMovement(
      lastDate,
      daysToQuery,
      groupedMarketings.flatMap((marketing) =>
        marketing.productTypes.flatMap((productType) =>
          productType.tradingAccounts.map(
            (tradingAccount) => tradingAccount.custcode
          )
        )
      )
    );
    const groupedCustcodeAndCurrency = groupCashMovementsByCustcodeAndCurrency(
      snCashMovement.items,
      customerInfo
    );
    const exchangeRate = await buildExchangeRate(lastDate);

    if (snCashMovement.dateKey == null) return [];

    const cashes = await buildCash(
      groupedMarketings.flatMap((marketing) => marketing.productTypes),
      exchangeRate,
      groupedMarketings.flatMap((marketing) =>
        marketing.productTypes.flatMap((productType) => {
          return {
            products: productType.tradingAccounts.map((tradingAccount) => {
              return {
                custcode: tradingAccount.custcode,
                accountNo: tradingAccount.tradingAccountNo,
                accountName: StructuredProductCondition.accountName,
              };
            }),
            dateKey: snCashMovement.dateKey,
            accountName: StructuredProductCondition.accountName,
          };
        })
      )
    );
    const withBalance = calculateBalance(
      groupedCustcodeAndCurrency,
      cashes,
      snCashMovement.dateKey
    );

    return withBalance;
  } catch (error) {
    console.error('Error buildCustomerStatementData:', error);
    throw error;
  }
}

function calculateBalance(
  groupedCashMovement: SNCashMovementCustcodeGroup[],
  cashes: PortfolioCash[],
  dateKey: Date
): SNCashMovementCustcodeGroup[] {
  return groupedCashMovement.map((custcodeGroup) => {
    return {
      custcode: custcodeGroup.custcode,
      subAccount: custcodeGroup.subAccount,
      customerName: custcodeGroup.customerName,
      dateKey: dateKey,
      snCurrency: custcodeGroup.snCurrency.map((currencyGroup) => {
        // Find the total balance for the current custcode and currency
        const cash = cashes.find(
          (cash) =>
            cash.custcode === custcodeGroup.custcode &&
            cash.currency === currencyGroup.currency &&
            cash.accountName === StructuredProductCondition.accountName
        );
        const totalBalance = cash?.cashBalance;

        if (totalBalance === undefined) {
          // Handle case where totalBalance is not found
          throw new Error(
            `Total balance not found for custcode: ${custcodeGroup.custcode}, currency: ${currencyGroup.currency}`
          );
        }

        // Sort the cashMovements by settlementDate, then transactionDate, then note (description)
        const sortedCashMovements = currencyGroup.cashMovement.sort((a, b) => {
          // Sort by settlementDate (descending)
          const settlementDateDiff =
            new Date(b.settlementDate).getTime() -
            new Date(a.settlementDate).getTime();
          if (settlementDateDiff !== 0) return settlementDateDiff;

          // If settlementDate is the same, sort by transactionDate (descending)
          const transactionDateDiff =
            new Date(b.transactionDate).getTime() -
            new Date(a.transactionDate).getTime();
          if (transactionDateDiff !== 0) return transactionDateDiff;

          // If both dates are the same, sort by note (description) (ascending)
          return a.note.localeCompare(b.note);
        });

        // Initialize balance to totalBalance
        let previousBalance = totalBalance;

        // Recalculate the balance based on the amountDebit and amountCredit
        const updatedCashMovements = sortedCashMovements.map((movement) => {
          // Update balance by subtracting amountDebit and adding amountCredit
          const currentBalance = previousBalance;
          previousBalance =
            parseFloat(nullToZero(previousBalance).toString()) -
            parseFloat(nullToZero(movement.amountDebit).toString()) +
            parseFloat(nullToZero(movement.amountCredit).toString());

          // Return a new object with updated balance
          return {
            ...movement,
            balance: currentBalance,
          };
        });

        return {
          currency: currencyGroup.currency,
          currencyFullName: currencyGroup.currencyFullName,
          cashMovement: updatedCashMovements.reverse(),
        };
      }),
    };
  });
}

function groupCashMovementsByCustcodeAndCurrency(
  cashMovements: SNCashMovement[],
  customerInfo: InternalCustomerByIdentificationNo
): SNCashMovementCustcodeGroup[] {
  // This object will store the grouped results
  const groupedResult: { [key: string]: SNCashMovementCustcodeGroup } = {};

  // Iterate through each cash movement and organize them into the desired structure
  cashMovements.forEach((movement) => {
    const { custcode, subAccount, currency } = movement;
    const key = `${custcode}-${subAccount}`;
    const customer = customerInfo.data.marketings
      .flatMap((marketing) => marketing.custCodes)
      .find((custCode) => custCode.custCode == custcode);
    const { firstnameEn, lastnameEn } = customer.basicInfo.name;

    // If there's no entry for this custcode-subAccount, create one
    if (!groupedResult[key]) {
      groupedResult[key] = {
        custcode,
        subAccount,
        customerName: `${firstnameEn} ${lastnameEn}`,
        dateKey: null,
        snCurrency: [],
      };
    }

    // Find if there's already a currency group for the current currency
    let currencyGroup = groupedResult[key].snCurrency.find(
      (group) => group.currency === currency
    );

    // If no currency group exists, create one
    if (!currencyGroup) {
      currencyGroup = {
        currency,
        currencyFullName: Currencies.find(
          (c) => c.ISO3.toLowerCase() == currency.toLowerCase()
        ).Name,
        cashMovement: [],
      };
      groupedResult[key].snCurrency.push(currencyGroup);
    }

    // Add the current cash movement to the corresponding currency group
    currencyGroup.cashMovement.push(movement);
  });

  // Return the grouped result as an array
  return Object.values(groupedResult);
}

async function buildSNCashMovement(
  lastDate: string,
  daysToQuery: number,
  custCodes: string[]
): Promise<DateKeyItem<SNCashMovement>> {
  try {
    console.log(`[DB](0) - StructureNotesCashMovement`);
    // Find the last dateKey
    const lastDateKeyRecord = await StructureNotesCashMovement.findOne({
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

    const queryTransactionDate = new Date(lastDate);
    queryTransactionDate.setDate(queryTransactionDate.getDate() - daysToQuery);
    // Retrieve all rows with the last dateKey
    const resultSets = await StructureNotesCashMovement.findAll({
      where: {
        dateKey: lastDateKey,
        custcode: { [Op.in]: custCodes },
        transactionDate: {
          [Op.gte]: queryTransactionDate.toISOString().split('T')[0],
        },
      },
      raw: true,
    });

    const result: SNCashMovement[] = resultSets.map((record) => ({
      custcode: record.custcode,
      accountNo: record.tradingAccountNo,
      subAccount: record.subAccount,
      transactionDate: record.transactionDate,
      settlementDate: record.settlementDate,
      transactionType: record.transactionType,
      currency: record.currency,
      amountDebit:
        record.amount >= 0
          ? parseFloat(nullToZero(record.amount).toString())
          : null,
      amountCredit:
        record.amount < 0
          ? parseFloat(nullToZero(record.amount).toString()) * -1
          : null,
      balance: 0, //filling in later
      note: record.note,
      dateKey: record.dateKey,
    }));

    return {
      items: result,
      dateKey: lastDateKey,
    };
  } catch (error) {
    console.error('Error fetching records:', error);
    throw error;
  }
}

export async function renderPDFBuffer(data: SNCashMovementCustcodeGroup[]) {
  const html = await ejs.renderFile(
    path.join('resources', 'atlas', 'views', 'sn-cash-movement', 'layout.ejs'),
    { themeSNConfig, data: data }
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

const delay = (ms) => new Promise((res) => setTimeout(res, ms));
