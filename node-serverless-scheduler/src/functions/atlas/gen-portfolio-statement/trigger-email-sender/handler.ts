import { ValidatedEventAPIGatewayProxyEvent } from '@libs/api-gateway';
import { DBConfigType, SecretManagerType, getSequelize } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';
import { getCustomerGroupByIdentificationNo } from '@libs/user-v2-api';
import {
  PortfolioJobHistory,
  initModel,
} from 'db/portfolio-summary/models/PortfolioJobHistory';
import { MailType } from 'src/constants/report';
import { v4 as uuidv4 } from 'uuid';
import schema from './schema';

export interface CustomerData {
  ids: number[];
  custcode: string;
  marketingId: string;
  sendDate: Date;
}

const run: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
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

    const pageSize = 100;
    let currentPage = 0;
    let total = 0;
    let totalInserted = 0;
    const validMarketingIds = event.body?.marketingIds || [];
    const customerSubTypes = event.body?.customerSubTypes || [];
    const cardTypes = event.body?.cardTypes || [];
    const mailType = event.body?.mailType || MailType.SNCashMovement;

    const marketingEmail = validMarketingIds.length > 0;

    do {
      currentPage += 1;
      const response = await getCustomerIdentification(
        currentPage,
        pageSize,
        marketingEmail,
        validMarketingIds,
        customerSubTypes,
        cardTypes
      );
      total = response.total;

      console.log(marketingEmail);
      let dataToInsert = [];
      if (marketingEmail) {
        dataToInsert = response.data.flatMap((data) =>
          data.marketings.map((marketing) => ({
            id: uuidv4(),
            identificationHash: data.identificationHash,
            custcode: '',
            marketingId: marketing.marketingId,
            metadata: JSON.stringify({
              mailType: mailType,
            }),
            sendDate: new Date(), // Current date and time
            createdAt: new Date(), // Current date and time
            status: 'pending',
          }))
        );
      } else {
        dataToInsert = response.data.map((data) => ({
          id: uuidv4(),
          identificationHash: data.identificationHash,
          custcode: '',
          marketingId: '',
          metadata: JSON.stringify({
            mailType: mailType,
          }),
          sendDate: new Date(), // Current date and time
          createdAt: new Date(), // Current date and time
          status: 'pending',
        }));
      }

      console.log(JSON.stringify(dataToInsert));
      const result = await PortfolioJobHistory.bulkCreate(dataToInsert);
      totalInserted += result.length;
    } while (currentPage * pageSize < total);

    return {
      statusCode: 200,
      body: JSON.stringify({ insertCount: totalInserted }),
    };
  } catch (error) {
    console.error('Error insert customer data:', error);

    throw error;
  } finally {
    if (sequelize) {
      await sequelize.close();
    }
  }
};

async function getCustomerIdentification(
  page: number,
  pageSize: number,
  includeTradingAccount: boolean,
  marketingIds: string[],
  customerSubTypes: string[],
  cardTypes: string[]
) {
  const onboardServiceHost =
    process.env.DEBUG_LOCAL === 'true'
      ? 'http://onboard-api.nonprod.pi.internal/'
      : (await getConfig()).onboardServiceHost;
  console.log(
    `[API][Onboard] getCustomerGroupByIdentificationNo: ${marketingIds}`
  );
  const customerInfos = await getCustomerGroupByIdentificationNo(
    page,
    pageSize,
    includeTradingAccount,
    onboardServiceHost,
    marketingIds,
    customerSubTypes,
    cardTypes
  );
  console.log(`[API] getCustomerGroupByIdentificationNo success`);
  console.log(JSON.stringify(customerInfos));
  return customerInfos;
}

async function getConfig() {
  const [onboardServiceHost] = await getConfigFromSsm('atlas', [
    'onboard-srv-host',
  ]);

  return {
    onboardServiceHost,
  };
}

export const main = middyfy(run);
