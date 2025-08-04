import { getSequelize } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import { defaultNull, stringToFloat } from '@libs/string-utils';
import {
  ExanteAccountSummaryCustom,
  initModel,
} from 'db/reconcile/models/ExanteCustomAccountSummary';
import { v4 as uuidv4 } from 'uuid';

interface AccountSummary {
  date: Date;
  account: string;
  instrument: string;
  iso: string;
  instrumentName: string;
  qty: number;
  avgPrice: number;
  price: number;
  ccy: string;
  pAndL: number;
  pAndLInEur: number;
  pAndLPercent: number;
  value: number;
  valueInEur: number;
  dailyPAndL: number;
  dailyPAndLInEur: number;
  dailyPAndLPercent: number;
  isin: string;
}

const run = async (event) => {
  const payload = transformPayload(event);
  console.info(payload);
  await insertReportData(payload);

  return {
    body: {
      Status: 'success',
    },
  };
};

const transformPayload = (event): AccountSummary => {
  return {
    date: new Date(event['Date']),
    account: defaultNull(event['Account']),
    instrument: defaultNull(event['Instrument']),
    iso: defaultNull(event['ISO']),
    instrumentName: defaultNull(event['Instrument name']),
    qty: stringToFloat(event['QTY']),
    avgPrice: stringToFloat(event['Avg Price']),
    price: stringToFloat(event['Price']),
    ccy: defaultNull(event['CCY']),
    pAndL: stringToFloat(event['P&L']),
    pAndLInEur: stringToFloat(event['P&L in EUR']),
    pAndLPercent: stringToFloat(event['P&L, %']),
    value: stringToFloat(event['Value']),
    valueInEur: stringToFloat(event['Value in EUR']),
    dailyPAndL: stringToFloat(event['Daily P&L']),
    dailyPAndLInEur: stringToFloat(event['Daily P&L in EUR']),
    dailyPAndLPercent: stringToFloat(event['Daily P&L, %']),
    isin: defaultNull(event['ISIN']),
  };
};

const insertReportData = async (accountSummary: AccountSummary) => {
  const sequelize = await getSequelize({
    parameterName: 'reconcile',
    dbHost: 'reconcile-db-host',
    dbPassword: 'reconcile-db-password',
    dbUsername: 'reconcile-db-username',
    dbName: 'reconcile_db',
  });
  try {
    const data = {
      id: uuidv4(),
      date: accountSummary.date,
      account: accountSummary.account,
      instrument: accountSummary.instrument,
      iso: accountSummary.iso,
      instrumentName: accountSummary.instrumentName,
      qty: accountSummary.qty,
      avgPrice: accountSummary.avgPrice,
      price: accountSummary.price,
      ccy: accountSummary.ccy,
      pAndL: accountSummary.pAndL,
      pAndLInEur: accountSummary.pAndLInEur,
      pAndLPercent: accountSummary.pAndLPercent,
      value: accountSummary.value,
      valueInEur: accountSummary.valueInEur,
      dailyPAndL: accountSummary.dailyPAndL,
      dailyPAndLInEur: accountSummary.dailyPAndLInEur,
      dailyPAndLPercent: accountSummary.dailyPAndLPercent,
      isin: accountSummary.isin,
    };
    initModel(sequelize);
    await ExanteAccountSummaryCustom.create(data);
  } catch (e) {
    console.error('Failed to insert report data\n', +JSON.stringify(e));
    throw e;
  } finally {
    if (sequelize) {
      await sequelize.close();
    }
  }
};

export const main = middyfy(run);
