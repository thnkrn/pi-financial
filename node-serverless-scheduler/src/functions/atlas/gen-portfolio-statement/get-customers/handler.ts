import { DBConfigType, SecretManagerType, getSequelize } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import { UserInfo } from '@libs/user-v2-api';
import {
  PortfolioJobHistory,
  initModel,
} from 'db/portfolio-summary/models/PortfolioJobHistory';

export interface CustomerData {
  ids: number[];
  userId: string;
  userInfos: UserInfo;
  marketingId: string;
  sendDate: Date;
  dateFrom: Date;
  dateTo: Date;
  custcode: string;
}

const run = async (event) => {
  const sequelize = await getSequelize({
    parameterName: 'atlas',
    dbHost: 'PORTFOLIOSUMMARYDATABASE_HOST',
    dbPassword: 'PORTFOLIOSUMMARYDATABASE_PASSWORD',
    dbUsername: 'PORTFOLIOSUMMARYDATABASE_USERNAME',
    dbName: 'portfolio_summary_db',
    dbConfigType:
      process.env.DEBUG_LOCAL == 'true'
        ? DBConfigType.Local
        : DBConfigType.SecretManager,
    secretManagerType: SecretManagerType.Public,
  });
  try {
    initModel(sequelize);

    const data = await PortfolioJobHistory.findAll({
      where: {
        status: 'pending',
        'metadata.mailType': event.body.mailType,
      },
      raw: true,
    });

    const transformedData: CustomerData[] = data.reduce((acc, obj) => {
      const existingItem = acc.find(
        (item) =>
          obj.identificationHash === item.identificationHash &&
          obj.custcode === item.custcode &&
          obj.marketingId === item.marketingId &&
          obj.sendDate === item.sendDate
      );
      if (existingItem) {
        existingItem.ids.push(obj.id);
      } else {
        acc.push({
          ids: [obj.id],
          identificationHash: obj.identificationHash,
          marketingId: obj.marketingId,
          sendDate: obj.sendDate,
        });
      }
      return acc;
    }, []);

    return {
      statusCode: 200,
      body: batchArray(transformedData, Number(event.body.batchSize)),
    };
  } catch (error) {
    console.error('Error get customer data:', error);
    throw error;
  } finally {
    if (sequelize) {
      await sequelize.close();
    }
  }
};

function batchArray<T>(array: T[], batchSize: number): T[][] {
  const batches: T[][] = [];
  for (let i = 0; i < array.length; i += batchSize) {
    batches.push(array.slice(i, i + batchSize));
  }
  return batches;
}

export const main = middyfy(run);
