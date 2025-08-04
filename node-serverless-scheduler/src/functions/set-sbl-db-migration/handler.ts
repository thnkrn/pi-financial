import { getSequelize } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import { getObject } from '@libs/s3-utils';
import { getConfigFromSsm } from '@libs/ssm-config';
import csvParser from 'csv-parser';
import {
  PiSblOrder,
  SblOrder,
  SiriusData,
  initModel,
} from 'db/sirius-migration/models/SblOrdersMigration';
import { PassThrough } from 'stream';
import { getInternalTradingAccount } from './api';

/**
 * Runs the main logic for processing the event.
 *
 * @return {Promise<object>} - a promise that resolves to the response object
 */
const run = async () => {
  try {
    const csvData = await downloadDataFromS3();
    await insertToPiDB(csvData);
    return {
      body: {
        status: 'SUCCEEDED',
      },
    };
  } catch (error) {
    console.error(
      'Error processing SBL migration database. Exception: ',
      JSON.stringify(error)
    );
    throw error;
  }
};

const streamToBuffer = async (
  stream: NodeJS.ReadableStream
): Promise<Buffer> => {
  return new Promise((resolve, reject) => {
    const chunks: Buffer[] = [];
    stream.on('data', (chunk) => chunks.push(chunk));
    stream.on('end', () => resolve(Buffer.concat(chunks)));
    stream.on('error', (err) => reject(err));
  });
};

const downloadDataFromS3 = async (): Promise<SiriusData[]> => {
  return new Promise(async (resolve, reject) => {
    const [fileKey, bucketName] = await getConfigFromSsm('set', [
      'sbl-migration-db-file-name',
      'sbl-migration-db-file-bucket-name',
    ]);
    try {
      const data = await getObject(bucketName, fileKey);
      if (!data.Body) {
        return reject(new Error(`CSV file not found. fileKey: ${fileKey}`));
      }

      const results: SiriusData[] = [];
      const stream = new PassThrough();
      const buffer = await streamToBuffer(data.Body as NodeJS.ReadableStream);
      stream.end(buffer);

      stream
        .pipe(csvParser())
        .on('data', (row) => {
          // Map row to SiriusData object
          const siriusData: SiriusData = {
            sbl_order_id: row.sbl_order_id,
            account_id: row.account_id,
            account_code: row.account_code,
            symbol: row.symbol,
            amount: row.amount,
            type: row.type,
            submit_time: row.submit_time,
            status: row.status,
            reject_reason: row.reject_reason,
            update_by: row.update_by,
            update_time: row.update_time,
            extra_info: null,
          };
          results.push(siriusData);
        })
        .on('end', () => {
          resolve(results);
        })
        .on('error', (error) => {
          reject(error);
        });
    } catch (error) {
      reject(error);
    }
  });
};
/**
 * Transforms the payload event into an EquityClosePrice object.
 *
 * @param event - The payload event.
 * @returns The transformed EquityClosePrice object.
 */
const transformPayload = async (
  data: SiriusData,
  host: string
): Promise<SblOrder> => {
  const custCode = data.account_code.substring(0, data.account_code.length - 1);

  return new Promise(async (resolve, reject) => {
    const onBoardAPIResult = await getInternalTradingAccount(host, {
      customercode: custCode,
    });
    const cbAccountInfo = onBoardAPIResult.data.find((item) => {
      return item.accountTypeCode == 'CB';
    });
    if (cbAccountInfo) {
      // Map Sirius status to Pi Status
      const statusMap: Record<string, string> = {
        Approve: 'Approved',
        Reject: 'Rejected',
        Pending: 'Pending',
      };
      if (!(data.status in statusMap)) {
        return reject(new Error('Invalid status'));
      }
      resolve({
        createdAt: data.submit_time,
        customerCode: custCode,
        orderId: parseInt(data.sbl_order_id),
        rejectedReason: data.reject_reason,
        reviewerId: null,
        status: statusMap[data.status],
        symbol: data.symbol,
        tradingAccountId: cbAccountInfo.id,
        tradingAccountNo: cbAccountInfo.tradingAccountNo,
        type: data.type,
        updatedAt: data.update_time,
        volume: data.amount,
      });
    } else {
      resolve(null);
    }
  });
};

const insertToPiDB = async (siriusDataArray: SiriusData[]) => {
  const hosts = await getConfigFromSsm('set', ['onboard-srv-host']);
  const transfromDataList: SblOrder[] = [];
  for (const item of siriusDataArray) {
    const transformedItem = await transformPayload(item, hosts[0]);
    if (transformedItem != null) transfromDataList.push(transformedItem);
  }

  const sequelize = await getSequelize({
    parameterName: 'set',
    dbHost: 'sbl-migration-db-host',
    dbPassword: 'sbl-migration-db-password',
    dbUsername: 'sbl-migration-db-username',
    dbName: 'set_db',
  });

  try {
    initModel(sequelize);

    await sequelize.transaction(async (transaction) => {
      await PiSblOrder.bulkCreate(transfromDataList, {
        transaction,
      });
    });
  } catch (e) {
    console.error('Failed to insert to DB\n', JSON.stringify(e));
    throw e;
  } finally {
    await sequelize.close();
  }
};

export const main = middyfy(run);
