import { DBConfigType, getSequelize, SecretManagerType } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import console from 'console';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import {
  getImportQuery,
  importDataFromFiles,
  incrementalImportDataFromFiles,
  ProductType,
} from '../gen-portfolio-statement/utils/etl-utils';

dayjs.extend(utc);
dayjs.extend(timezone);
interface Payload {
  bucket: string;
  date: string;
  importType: 'incremental' | 'full';
}

const run = async (event) => {
  console.info(event);
  const payload = event.body as Payload;
  console.info(payload);
  const { bucket, date: dateKey, importType = 'full' } = payload;
  console.info('Date:', dateKey);
  console.info('ImportType:', importType);

  const sequelize = await getSequelize({
    parameterName: 'atlas',
    dbHost: 'PORTFOLIOSUMMARYDATABASE_HOST',
    dbPassword: 'PORTFOLIOSUMMARYDATABASE_PASSWORD',
    dbUsername: 'PORTFOLIOSUMMARYDATABASE_USERNAME',
    dbName: 'portfolio_summary_db',
    dbConfigType: DBConfigType.SecretManager,
    secretManagerType: SecretManagerType.Public,
  });

  const executeImportData =
    importType === 'incremental'
      ? incrementalImportDataFromFiles
      : importDataFromFiles;

  const products = Object.values(ProductType);
  console.info('Products:', products);
  await Promise.all(
    products.map((product) => {
      return executeImportData(
        sequelize,
        bucket,
        dateKey,
        product,
        getImportQuery(product)
      );
    })
  );

  return {
    body: {
      success: true,
    },
  };
};

export const main = middyfy(run);
