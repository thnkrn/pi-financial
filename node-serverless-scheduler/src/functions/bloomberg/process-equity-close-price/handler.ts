import { downloadEquityClosePrice } from '@libs/bloomberg-api';
import { parseCsv } from '@libs/csv-utils';
import { getSequelize } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import { storeFileToS3 } from '@libs/s3-utils';
import { getConfigFromSsm } from '@libs/ssm-config';
import {
  defaultNull,
  stringToDate,
  stringToFloat,
  stringToNumber,
} from '@libs/string-utils';
import {
  BloombergEquityCloseprice,
  initModel,
} from 'db/reconcile/models/BloombergEquityClosePrice';
import Papa from 'papaparse';

interface EquityClosePrice {
  dlRequestId: string;
  dlRequestName: string;
  dlSnapshotStartTime: Date;
  dlSnapshotTz: string;
  identifier: string;
  rc: number;
  pxCloseDt: Date;
  idExchSymbol: string;
  name: string;
  pxLastEod: number;
  crncy: string;
  compositeExchCode: string;
  idIsin: string;
  lastUpdateDateEod: Date;
  icbSupersectorName: string;
  icbSectorName: string;
}

interface Event {
  body: Payload;
}

interface Payload {
  identifier: string;
  date: string;
}

interface EftData {
  [key: string]: string | number | Date | null;
}

async function getBloomBergConfig() {
  const [bloombergServiceHost] = await getConfigFromSsm('report', [
    'bloomberg-srv-host',
  ]);

  return {
    bloombergServiceHost,
  };
}

const DEFAULT_VALUES = {
  ICB_SUPERSECTOR_NAME: 'Financial Services',
  ICB_SECTOR_NAME: 'Open End and Miscellaneous Investment Vehicles',
};

/**
 * Runs the main logic for processing the event.
 *
 * @param {Event} event - the event object containing the data to be processed
 * @return {Promise<object>} - a promise that resolves to the response object
 */
const run = async (event: Event) => {
  console.info(event);
  const payload: Payload = event.body;
  console.info(payload);

  try {
    const bloombergConfig = await getBloomBergConfig();
    const response = await downloadEquityClosePrice(
      bloombergConfig.bloombergServiceHost,
      payload?.identifier
    );

    const data = await parseCsv(response.body.toString('utf-8'));

    const eftData: EftData[] = data.map((obj) =>
      Object.fromEntries(
        Object.entries(obj).map(([key, value]) => [
          key,
          (key === 'ICB_SUPERSECTOR_NAME' || key === 'ICB_SECTOR_NAME') &&
          (value === null || value === '' || !value)
            ? DEFAULT_VALUES[key]
            : value,
        ])
      )
    );

    await Promise.all([
      uploadFiletoS3(eftData, payload.date),
      insertToDB(eftData),
    ]);

    return {
      body: {
        status: 'SUCCEEDED',
      },
    };
  } catch (error) {
    console.error(
      'Error processing Bloomberg equity close price. Exception: ',
      JSON.stringify(error)
    );
    throw error;
  }
};

/**
 * Uploads EftData to S3.
 *
 * @param eftData - The EftData to upload.
 * @param date - The date for the file name from payload date at intialization.
 * @returns A Promise that resolves when the upload is complete.
 */
const uploadFiletoS3 = async (eftData: EftData[], date: string) => {
  const equityClosePriceCSV = Papa.unparse(eftData);
  const csvBuffer = Buffer.from(equityClosePriceCSV, 'utf-8');
  const fileName = `equity_closeprice_${date}.csv`;

  await Promise.all([
    storeFileToS3(
      `backoffice-reports-${process.env.ENVIRONMENT}`,
      csvBuffer,
      `bloomberg/${fileName}`
    ),
    storeFileToS3(
      `pi-exante-reports-${
        process.env.ENVIRONMENT === 'production' ? 'prod' : 'nonprod'
      }`,
      csvBuffer,
      fileName
    ),
  ]);
};

/**
 * Inserts EftData into the database.
 *
 * @param eftData - An array of EftData objects.
 * @returns A Promise that resolves when the insertion is complete.
 */
const insertToDB = async (eftData: EftData[]) => {
  const equityClosePrices: EquityClosePrice[] = eftData.map(transformPayload);
  const sequelize = await getSequelize({
    parameterName: 'reconcile',
    dbHost: 'reconcile-db-host',
    dbPassword: 'reconcile-db-password',
    dbUsername: 'reconcile-db-username',
    dbName: 'reconcile_db',
    pool: {
      max: 20,
      min: 0,
      idle: 10000,
      evict: 1000,
    },
  });

  try {
    initModel(sequelize);

    await sequelize.transaction(async (transaction) => {
      await BloombergEquityCloseprice.bulkCreate(equityClosePrices, {
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

/**
 * Transforms the payload event into an EquityClosePrice object.
 *
 * @param event - The payload event.
 * @returns The transformed EquityClosePrice object.
 */
const transformPayload = (event: Record<string, string>): EquityClosePrice => {
  return {
    dlRequestId: defaultNull(event['DL_REQUEST_ID']),
    dlRequestName: defaultNull(event['DL_REQUEST_NAME']),
    dlSnapshotStartTime: stringToDate(event['DL_SNAPSHOT_START_TIME']),
    dlSnapshotTz: defaultNull(event['DL_SNAPSHOT_TZ']),
    identifier: defaultNull(event['IDENTIFIER']),
    rc: stringToNumber(event['RC']),
    pxCloseDt: stringToDate(event['PX_CLOSE_DT']),
    idExchSymbol: defaultNull(event['ID_EXCH_SYMBOL']),
    name: defaultNull(event['NAME']),
    pxLastEod: stringToFloat(event['PX_LAST_EOD']),
    crncy: defaultNull(event['CRNCY']),
    compositeExchCode: defaultNull(event['COMPOSITE_EXCH_CODE']),
    idIsin: defaultNull(event['ID_ISIN']),
    lastUpdateDateEod: stringToDate(event['LAST_UPDATE_DATE_EOD']),
    icbSupersectorName: defaultNull(event['ICB_SUPERSECTOR_NAME']),
    icbSectorName: defaultNull(event['ICB_SECTOR_NAME']),
  };
};

export const main = middyfy(run);
