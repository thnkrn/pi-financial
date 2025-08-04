import { getSequelize } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import { getObject } from '@libs/s3-utils';
import csvParser from 'csv-parser';
import {
  BmaInstrument,
  initModel,
} from '../../db/bond/models/BondTBmaMigration';
import { Readable } from 'stream';
import { Handler, S3Event } from 'aws-lambda';
import { Sequelize } from 'sequelize';

const syncInstruments = async (bucketName: string, fileKey: string) => {
  const data = await getObject(bucketName, fileKey);
  if (!data.Body) {
    throw new Error(`File content is null: ${fileKey}`);
  }

  if (data.Body instanceof Readable) {
    const sequelize = await getSequelize({
      parameterName: 'bond',
      dbHost: 'bond-db-host',
      dbPassword: 'bond-db-password',
      dbUsername: 'bond-db-username',
      dbName: 'bond_db',
    });
    initModel(sequelize);

    try {
      await BmaInstrument.truncate();

      // Sync bond instruments
      await processCsvStream(data.Body, sequelize);
      await sequelize.close();
    } catch (e) {
      await sequelize.close();
    }
  } else {
    throw new Error(`File is not Readable: ${fileKey}`);
  }
};

const processCsvStream = (
  stream: Readable,
  sequelize: Sequelize
): Promise<void> => {
  return new Promise((resolve, reject) => {
    const batchSize = 100;
    let batch: BmaInstrument[] = [];
    let inserting = false;

    stream
      .pipe(csvParser()) // Parses CSV row-by-row
      .on('data', (row) => {
        batch.push(<BmaInstrument>TransformData(row));

        if (batch.length === batchSize) {
          stream.pause(); // Pause the stream while saving to DB

          console.info('Inserting into database.');
          inserting = true;
          insertToPiDB(batch, sequelize)
            .then(() => {
              inserting = false;
              batch = [];
              stream.resume(); // Resume the stream after DB operation
            })
            .catch((err) => reject(err));
        }
      })
      .on('end', async () => {
        console.info('CSV processing complete.');
        if (batch.length > 0 && !inserting) {
          console.info('Final Inserting into database.');
          await insertToPiDB(batch, sequelize);
          batch = [];
        } else {
          resolve();
        }
      })
      .on('error', (err) => {
        console.error('Error processing CSV:', err);
        reject(err);
      });
  });
};

const TransformData = (data: any) => {
  const keys = Object.keys(data);

  return {
    symbol: data[keys[0]],
    issueRating: data['Issue rating / Rating Agency'] || null,
    companyRating: data['Company rating / Rating Agency'] || null,
    couponType: data['Coupon Type'] || null,
    couponRate: safeParseFloat(data['Coupon Rate (%)']),
    issuedDate: formatDate(data['Issued Date']),
    maturityDate: formatDate(data['Maturity Date']),
    ttm: safeParseFloat(data['TTM (Yrs.)']),
    outstanding: safeParseFloat(data['Outstanding Value (THB mln)']),
  };
};

const formatDate = (dateStr: string | null | undefined): string | null => {
  if (!dateStr || dateStr.trim() === '') return null;

  const [day, month, year] = dateStr.split('/').map((num) => parseInt(num));

  // Ensure two-digit day and month, and convert year to four digits
  const formattedDay = day.toString().padStart(2, '0');
  const formattedMonth = month.toString().padStart(2, '0');
  const formattedYear =
    year < 100 ? `20${year.toString().padStart(2, '0')}` : year.toString();

  return `${formattedYear}-${formattedMonth}-${formattedDay}`;
};

const safeParseFloat = (value: string | null | undefined): number | null => {
  if (!value || value.trim() === '') return null;
  const parsed = parseFloat(value);
  return isNaN(parsed) ? null : parsed;
};

const insertToPiDB = async (
  instruments: BmaInstrument[],
  sequelize: Sequelize
) => {
  try {
    await sequelize.transaction(async (transaction) => {
      await BmaInstrument.bulkCreate(instruments, {
        transaction,
      });
    });
  } catch (e) {
    console.error('Failed to insert to DB', JSON.stringify(e));
    throw e;
  }
};

export const handleSyncThaiBmaInstruments: Handler = async (event: S3Event) => {
  try {
    if ('Records' in event) {
      for (const record of event.Records) {
        if (record.eventSource !== 'aws:s3') continue;

        const bucketName = record.s3.bucket.name;
        const fileKey = record.s3.object.key;

        await syncInstruments(bucketName, fileKey);
      }
    } else {
      throw new Error('Unsupported event type');
    }
  } catch (error) {
    console.error(
      'Error processing SBL migration database. Exception: ',
      JSON.stringify(error)
    );
    throw error;
  }
};

export const main = middyfy(handleSyncThaiBmaInstruments);
