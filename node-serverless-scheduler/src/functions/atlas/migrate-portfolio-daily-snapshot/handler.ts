import { DBConfigType, SecretManagerType, getSequelize } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import console from 'console';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import {
  getImportQuery,
  incrementalImportDataFromFiles,
  ProductType,
} from '../gen-portfolio-statement/utils/etl-utils';
import dayjs from 'dayjs';
dayjs.extend(utc);
dayjs.extend(timezone);

interface Payload {
  bucket: string;
  product: string;
  dateFrom: string;
  dateTo: string;
}

const migrate = async (event) => {
  console.info(event);
  const payload = event.body as Payload;
  console.info(payload);
  const dateFrom = payload.dateFrom;
  const dateTo = payload.dateTo;
  const bucket = payload.bucket;
  const product = payload.product;

  const sequelize = await getSequelize({
    parameterName: 'atlas',
    dbHost: 'PORTFOLIOSUMMARYDATABASE_HOST',
    dbPassword: 'PORTFOLIOSUMMARYDATABASE_PASSWORD',
    dbUsername: 'PORTFOLIOSUMMARYDATABASE_USERNAME',
    dbName: 'portfolio_summary_db',
    dbConfigType: DBConfigType.SecretManager,
    secretManagerType: SecretManagerType.Public,
  });

  const dateRange = getDateRange(dateFrom, dateTo);

  const productType = (Object.values(ProductType) as string[]).includes(product)
    ? (product as ProductType)
    : undefined;
  if (!productType) {
    return {
      statusCode: 422,
      message: 'invalid product',
      body: JSON.stringify({
        success: false,
        message: 'invalid product',
        product,
        dateFrom,
        dateTo,
      }),
    };
  }

  for (const dateKey of dateRange) {
    await incrementalImportDataFromFiles(
      sequelize,
      bucket,
      dateKey,
      productType,
      getImportQuery(productType)
    );
  }
  return {
    statusCode: 200,
    body: JSON.stringify({
      success: true,
      product,
      dateFrom,
      dateTo,
    }),
  };
};

const getDateRange = (start: string, end: string) => {
  const dateArray = [];
  const currentDate = new Date(start);

  while (currentDate <= new Date(end)) {
    dateArray.push(new Date(currentDate).toISOString().slice(0, 10));
    currentDate.setDate(currentDate.getDate() + 1);
  }

  return dateArray;
};

export const atlasMigrate = middyfy(migrate);
