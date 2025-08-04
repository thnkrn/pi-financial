import { getMssqlConnection, SecretManagerType } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import { storeFileToS3 } from '@libs/s3-utils';
import { getAccessibility, getSecretValue } from '@libs/secrets-manager-client';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import { ConnectionPool } from 'mssql';
import { traderFileConfig } from '../../models/TraderFile';
import { FixedLengthGenerator } from '../../utils/FixedLengthGenerator';

dayjs.extend(utc);
dayjs.extend(timezone);

interface Payload {
  date: string;
  timezone: string;
  filePrefix: string;
  fileDir: string;
  backofficeBucket: string;
  cmeBucket: string;
}

const run = async (event) => {
  console.info('Event:', event);
  let pool: ConnectionPool;

  try {
    const payload = event.body as Payload;
    console.info('Payload:', payload);

    const cmeSecret = await getSecretValue(
      'cme-marketdata',
      getAccessibility(SecretManagerType.Secret)
    );

    const CME_DB_NAME = cmeSecret['CME_DB_NAME'];
    if (!CME_DB_NAME) throw new Error('CME_DB_NAME missing from secret');
    console.info('Database name:', CME_DB_NAME);

    pool = await getMssqlConnection({
      parameterName: 'cme-marketdata',
      dbHost: 'CME_DB_HOST',
      dbPassword: 'CME_DB_PASSWORD',
      dbUsername: 'CME_DB_USERNAME',
      dbName: CME_DB_NAME,
      dbPort: 1433,
    });

    const result = await pool.request().execute('get_cmetraderuser');

    const results = result.recordset;

    console.info('Query successful, records found:', results.length);

    if (results.length === 0) {
      console.warn('No records found in database');
      throw new Error('No records found to process');
    }

    const generator = new FixedLengthGenerator(traderFileConfig);

    const fileContent = generator.generate(results);

    const formattedDate = dayjs(payload.date)
      .tz(payload.timezone)
      .format('YYYYMMDD');
    const fileName = `${payload.filePrefix}_${formattedDate}.txt`;
    const fileKey = `${formattedDate}/${fileName}`;
    const boFileKey = `${payload.fileDir}/${fileKey}`;

    await storeFileToS3(
      payload.backofficeBucket,
      Buffer.from(fileContent, 'utf8'),
      boFileKey
    );

    await storeFileToS3(
      payload.cmeBucket,
      Buffer.from(fileContent, 'utf8'),
      fileKey
    );

    console.info('File saved to S3:', fileKey);

    return {
      body: {
        fileKey: boFileKey,
        recordCount: results.length,
      },
    };
  } catch (error) {
    console.error('Error details:', error);
    throw new Error(`Failed to process: ${error.message}`);
  } finally {
    if (pool) {
      await pool.close();
      console.info('Database connection closed');
    }
  }
};

export const main = middyfy(run);
