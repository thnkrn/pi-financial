import { getSequelize } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import { stringNotNone, stringToFloat } from '@libs/string-utils';
import {
  ExanteTransaction,
  initModel,
} from 'db/reconcile/models/ExanteTransaction';
import { v4 as uuidv4 } from 'uuid';

interface Transaction {
  transactionId: string;
  accountId: string;
  symbolId: string;
  isin: string;
  operationType: string;
  when: Date;
  sum: number;
  asset: string;
  eurEquivalent: number;
  comment: string;
  uuid: string;
  parentUuid: string;
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

const transformPayload = (event) => {
  return {
    transactionId: stringNotNone(event['Transaction ID']),
    symbolId: stringNotNone(event['Symbol ID']),
    accountId: stringNotNone(event['Account ID']),
    isin: stringNotNone(event['ISIN']),
    operationType: stringNotNone(event['Operation type']),
    when: new Date(event['When']),
    sum: stringToFloat(event['Sum']),
    asset: stringNotNone(event['Asset']),
    eurEquivalent: stringToFloat(event['EUR equivalent']),
    comment: stringNotNone(event['Comment']),
    uuid: stringNotNone(event['UUID']),
    parentUuid: stringNotNone(event['Parent UUID']),
  };
};

const insertReportData = async (transaction: Transaction) => {
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
      transactionId: transaction.transactionId,
      accountId: transaction.accountId,
      symbolId: transaction.symbolId,
      isin: transaction.isin,
      operationType: transaction.operationType,
      when: transaction.when,
      sum: transaction.sum,
      asset: transaction.asset,
      eurEquivalent: transaction.eurEquivalent,
      comment: transaction.comment,
      uuid: transaction.uuid,
      parentUuid: transaction.parentUuid,
    };
    initModel(sequelize);
    await ExanteTransaction.create(data);
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
