import { SecretManagerType } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';

import { InternalEmployeesResponse } from '@libs/employee-api';
import { getAccessibility, getSecretValue } from '@libs/secrets-manager-client';
import { InternalCustomerByIdentificationNo } from '@libs/user-v2-api';
import { createHash } from 'crypto';
import { existsSync, mkdirSync, writeFileSync } from 'fs';
import { Sequelize } from 'sequelize';
import { StructuredProductCondition } from 'src/constants/account-type-condition';
import {
  buildCustomerStatementData,
  renderPDFBuffer,
} from '../utils/build-customer-statement-utils';
import {
  buildGlobalEquityStatementData,
  renderGEPDFBuffer,
} from '../utils/build-global-equity-utils';
import {
  buildSNCashMovementData,
  renderPDFBuffer as renderSNPDFBuffer,
} from '../utils/build-sn-cash-movement-utils';
import { htmlEmailContent } from '../utils/process-batch-utils';
import { writeToFile } from './handler-helper';
import { GroupedMarketingProductType } from './internal-model';
import { statementHandler } from './statement-handler';

async function getHandler(event) {
  const { custcode, marketingId, hash, dateFrom, dateTo } =
    event.queryStringParameters;
  const secret = await getSecretValue(
    'atlas',
    getAccessibility(SecretManagerType.Public)
  );

  function sha256Hash(data: string): string {
    return createHash('sha256').update(data).digest('hex');
  }
  const dataToHash = `${custcode}-${marketingId}-${secret['PORTFOLIOSUMMARY_HASHKEY']}`;
  const hashedData = sha256Hash(dataToHash);

  if (hashedData !== hash) {
    console.error('Invalid hash');
    throw new Error('Invalid hash');
  }

  event.body = {
    custcode: custcode,
    marketingId: marketingId,
    sendDate: new Date().toISOString().split('T')[0],
    dateFrom: dateFrom && new Date(dateFrom).toISOString().slice(0, 10),
    dateTo: dateTo && new Date(dateTo).toISOString().slice(0, 10),
  };
  return event;
}

async function portfolioGetHandler(event, context) {
  try {
    const modifiedEvent = await getHandler(event);
    return await portfolioPostHandler(modifiedEvent, context);
  } catch (error) {
    return {
      statusCode: 400,
      body: JSON.stringify({ exception: error }),
    };
  }
}

async function portfolioPostHandler(event, context) {
  return await statementHandler(
    event,
    context,
    buildPortfolioPdfBuffer,
    htmlEmailContent,
    'PORTFOLIO'
  );
}

async function snCashMovementGetHandler(event, context) {
  try {
    const modifiedEvent = await getHandler(event);
    return await snCashMovementPostHandler(modifiedEvent, context);
  } catch (error) {
    return {
      statusCode: 400,
      body: JSON.stringify({ exception: error }),
    };
  }
}

async function snCashMovementPostHandler(event, context) {
  return await statementHandler(
    event,
    context,
    buildSNCashMovementPdfBuffer,
    htmlEmailContent,
    'SN_CASH'
  );
}

async function genGlobalEquityStatementGetHandler(event, context) {
  try {
    const modifiedEvent = await getHandler(event);
    return await genGlobalEquityStatementPostHandler(modifiedEvent, context);
  } catch (error) {
    return {
      statusCode: 400,
      body: JSON.stringify({ exception: error }),
    };
  }
}

async function genGlobalEquityStatementPostHandler(event, context) {
  return await statementHandler(
    event,
    context,
    buildGlobalEquityStatementPdfBuffer,
    htmlEmailContent,
    'GE'
  );
}

async function buildGlobalEquityStatementPdfBuffer(
  customerInfo: InternalCustomerByIdentificationNo,
  marketingInfos: InternalEmployeesResponse,
  groupedMarketings: GroupedMarketingProductType[],
  marketingId: string,
  lastDateToQuery: string,
  sequelize: Sequelize,
  isHttp: boolean,
  isMarketingEmail: boolean,
  dateFrom: Date,
  dateTo: Date,
  custcode: string,
  userId: string
): Promise<Buffer> {
  const data = await buildGlobalEquityStatementData(
    marketingId,
    groupedMarketings,
    customerInfo,
    marketingInfos,
    sequelize,
    dateFrom,
    dateTo,
    custcode,
    userId
  );

  return await renderGEPDFBuffer(data);
}

async function buildPortfolioPdfBuffer(
  customerInfo: InternalCustomerByIdentificationNo,
  marketingInfos: InternalEmployeesResponse,
  groupedMarketings: GroupedMarketingProductType[],
  marketingId: string,
  lastDateToQuery: string,
  sequelize: Sequelize,
  isHttp: boolean,
  isMarketingEmail: boolean
): Promise<Buffer> {
  const customerStatementData = await buildCustomerStatementData(
    customerInfo,
    marketingInfos,
    groupedMarketings,
    marketingId,
    lastDateToQuery,
    sequelize
  );
  // const customerStatementData = mockCustomerStatementData();

  let pdfBuffer: Buffer = null;
  if (
    customerStatementData.consolidatedSummary.hasYearOrMonthValue ||
    customerStatementData.marketings.some((marketing) =>
      marketing.bond.some((bond) => bond.marketType == 'Self Custodized')
    ) ||
    isHttp
  ) {
    pdfBuffer = await renderPDFBuffer(customerStatementData, isMarketingEmail);
  }

  if (process.env.DEBUG_LOCAL === 'true') {
    if (!existsSync('./tmp')) {
      mkdirSync('./tmp', { recursive: true }); // Create the directory recursively
    }

    writeToFile(customerStatementData, `customerStatementData`);
    if (pdfBuffer != null)
      writeFileSync('./tmp/customerStatementData.pdf', pdfBuffer);
    else console.error('No customerStatementData.pdf to write');
  }
  return pdfBuffer;
}

async function buildSNCashMovementPdfBuffer(
  customerInfo: InternalCustomerByIdentificationNo,
  marketingInfos: InternalEmployeesResponse,
  groupedMarketings: GroupedMarketingProductType[],
  marketingId: string,
  lastDateToQuery: string,
  sequelize: Sequelize,
  isHttp: boolean,
  isMarketingEmail: boolean
): Promise<Buffer> {
  const filteredMarketings = groupedMarketings
    .filter((marketing) =>
      marketingId === '' ? true : marketing.marketingId === marketingId
    )
    .map((marketing) => {
      // Filter custCodes inside each marketing based on the condition
      const filteredProductTypes = marketing.productTypes.filter(
        (productType) =>
          (StructuredProductCondition.accountCondition.length === 0 ||
            StructuredProductCondition.accountCondition.some(
              (filter) =>
                productType.exchangeMarketId == filter.exchangeMarketId &&
                productType.accountTypeCode == filter.accountTypeCode &&
                productType.accountType == filter.accountType
            )) &&
          (StructuredProductCondition.customerCondition.length === 0 ||
            StructuredProductCondition.customerCondition.some(
              (condition) =>
                productType.customerSubType === condition.customerSubType &&
                condition.customerType.includes(productType.customerType)
            ))
      );

      // If there are any valid custCodes, return the marketing object with the filtered custCodes
      if (filteredProductTypes.length > 0) {
        return { ...marketing, productTypes: filteredProductTypes };
      }

      // If no custCodes match, return null to be filtered out later
      return null;
    })
    // Filter out any marketing objects that had no matching custCodes
    .filter((marketing) => marketing !== null);

  const data = await buildSNCashMovementData(
    filteredMarketings,
    lastDateToQuery,
    365,
    customerInfo,
    sequelize
  );

  return await renderSNPDFBuffer(data);
}

export const portfolioPost = middyfy(portfolioPostHandler);

export const portfolioGet = middyfy(portfolioGetHandler);

export const snCashMovementPost = middyfy(snCashMovementPostHandler);

export const snCashMovementGet = middyfy(snCashMovementGetHandler);

export const genGlobalEquityStatementGet = middyfy(
  genGlobalEquityStatementGetHandler
);

export const genGlobalEquityStatementPost = middyfy(
  genGlobalEquityStatementPostHandler
);
