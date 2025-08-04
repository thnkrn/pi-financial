import { getSequelize } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import {
  stringNotNone,
  stringToFloat,
  stringToNumber,
} from '@libs/string-utils';
import { ExanteTrade, initModel } from 'db/reconcile/models/ExanteTrade';
import { v4 as uuidv4 } from 'uuid';

interface Trades {
  time: Date;
  accountId: string;
  side: string;
  symbolId: string;
  isin: string;
  type: string;
  price: number;
  currency: string;
  quantity: number;
  commission: number;
  commissionCurrency: string;
  pAndL: number;
  tradedVolume: number;
  orderId: string;
  orderPos: number;
  valueDate: Date;
  uniqueTransactionIdentifier: string;
  tradeType: string;
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

const transformPayload = (event): Trades => {
  return {
    time: new Date(event['Time']),
    accountId: stringNotNone(event['Account ID']),
    side: stringNotNone(event['Side']),
    symbolId: stringNotNone(event['Symbol ID']),
    isin: stringNotNone(event['ISIN']),
    type: stringNotNone(event['Type']),
    price: stringToFloat(event['Price']),
    currency: stringNotNone(event['Currency']),
    quantity: stringToFloat(event['Quantity']),
    commission: stringToFloat(event['Commission']),
    commissionCurrency: stringNotNone(event['Commission Currency']),
    pAndL: stringToFloat(event['P&L']),
    tradedVolume: stringToFloat(event['Traded Volume']),
    orderId: stringNotNone(event['Order Id']),
    orderPos: stringToNumber(event['Order pos']),
    valueDate: new Date(event['Value Date']),
    uniqueTransactionIdentifier: stringNotNone(
      event['Unique Transaction Identifier (UTI)']
    ),
    tradeType: stringNotNone(event['Trade type']),
  };
};

const insertReportData = async (trades: Trades) => {
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
      time: trades.time,
      accountId: trades.accountId,
      side: trades.side,
      symbolId: trades.symbolId,
      isin: trades.isin,
      type: trades.type,
      price: trades.price,
      currency: trades.currency,
      quantity: trades.quantity,
      commission: trades.commission,
      commissionCurrency: trades.commissionCurrency,
      pAndL: trades.pAndL,
      tradedVolume: trades.tradedVolume,
      orderId: trades.orderId,
      orderPos: trades.orderPos,
      valueDate: trades.valueDate,
      uti: trades.uniqueTransactionIdentifier,
      tradeType: trades.tradeType,
    };
    initModel(sequelize);
    await ExanteTrade.create(data);
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
