import { getSequelize } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import { defaultNull, stringToFloat } from '@libs/string-utils';
import { ExanteTradeVat, initModel } from 'db/reconcile/models/ExanteTradeVat';
import { v4 as uuidv4 } from 'uuid';

interface TradeVat {
  transactionId: string;
  accountId: string;
  symbolId: string;
  orderId: string;
  isin: string;
  operationType: string;
  when: Date;
  sum: number;
  commissionBeforeVat: number;
  otherFees: number;
  vatAmount: number;
  totalCommission: number;
  exanteCommissionWithOtherFees: number;
  partnerRebate: number;
  asset: string;
}

const run = async (event) => {
  console.info('---event---');
  console.info(event);
  console.info('---end event---');
  const payload = transformPayload(event);
  console.info(payload);
  await insertReportData(payload);

  return {
    body: {
      Status: 'success',
    },
  };
};

const transformPayload = (event) => {
  return {
    transactionId: defaultNull(event['Transaction ID']),
    symbolId: defaultNull(event['Symbol ID']),
    accountId: defaultNull(event['Account ID']),
    orderId: defaultNull(event['Order ID']),
    isin: defaultNull(event['ISIN']),
    operationType: defaultNull(event['Operation type']),
    when: new Date(event['When']),
    sum: stringToFloat(event['Sum']),
    commissionBeforeVat: stringToFloat(event['Commission before VAT']),
    otherFees: stringToFloat(event['Other fees']),
    vatAmount: stringToFloat(event['VAT 7 %']),
    totalCommission: stringToFloat(event['Total commission']),
    exanteCommissionWithOtherFees: stringToFloat(
      event['Exante commission with other fees']
    ),
    partnerRebate: stringToFloat(event['Partner rebate']),
    asset: defaultNull(event['Asset']),
  };
};

const insertReportData = async (tradeVat: TradeVat) => {
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
      transactionId: tradeVat.transactionId,
      accountId: tradeVat.accountId,
      symbolId: tradeVat.symbolId,
      orderId: tradeVat.orderId,
      isin: tradeVat.isin,
      operationType: tradeVat.operationType,
      when: tradeVat.when,
      sum: tradeVat.sum,
      commissionBeforeVat: tradeVat.commissionBeforeVat,
      otherFees: tradeVat.otherFees,
      vatAmount: tradeVat.vatAmount,
      totalCommission: tradeVat.totalCommission,
      exanteCommissionWithOtherFees: tradeVat.exanteCommissionWithOtherFees,
      partnerRebate: tradeVat.partnerRebate,
      asset: tradeVat.asset,
    };
    initModel(sequelize);
    await ExanteTradeVat.create(data);
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
