import { DBConfigType, SecretManagerType, getSequelize } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import console from 'console';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import { PortfolioBase } from 'db/portfolio-summary/models/PortfolioBase';
import {
  PortfolioBondDailySnapshot,
  initModel as initBondModel,
} from 'db/portfolio-summary/models/PortfolioBondDailySnapshot';
import {
  PortfolioBondOffshoreDailySnapshot,
  initModel as initBondOffshoreModel,
} from 'db/portfolio-summary/models/PortfolioBondOffshoreDailySnapshot';
import {
  PortfolioCashDailySnapshot,
  initModel as initCashModel,
} from 'db/portfolio-summary/models/PortfolioCashDailySnapshot';
import {
  PortfolioExchangeRateDailySnapshot,
  initModel as initExchangeRateModel,
} from 'db/portfolio-summary/models/PortfolioExchangeRateDailySnapshot';
import {
  PortfolioGlobalEquityDailySnapshot,
  initModel as initGlobalEquityModel,
} from 'db/portfolio-summary/models/PortfolioGlobalEquityDailySnapshot';
import {
  PortfolioGlobalEquityDepositwithdraw,
  initModel as initGlobalEquityDepositwithdrawModel,
} from 'db/portfolio-summary/models/PortfolioGlobalEquityDepositwithdraw';
import {
  PortfolioGlobalEquityDividend,
  initModel as initGlobalEquityDividendModel,
} from 'db/portfolio-summary/models/PortfolioGlobalEquityDividend';
import {
  PortfolioGlobalEquityOtcDailySnapshot,
  initModel as initGlobalEquityOtcModel,
} from 'db/portfolio-summary/models/PortfolioGlobalEquityOtcDailySnapshot';
import {
  PortfolioGlobalEquityTrade,
  initModel as initGlobalEquityTradeModel,
} from 'db/portfolio-summary/models/PortfolioGlobalEquityTrade';
import {
  PortfolioMutualFundDailySnapshot,
  initModel as initMutualFundModel,
} from 'db/portfolio-summary/models/PortfolioMutualFundDailySnapshot';
import {
  PortfolioStructuredProductDailySnapshot,
  initModel as initStructuredProductModel,
} from 'db/portfolio-summary/models/PortfolioStructuredProductDailySnapshot';
import {
  PortfolioStructuredProductOnshoreDailySnapshot,
  initModel as initStructuredProductOffshoreModel,
} from 'db/portfolio-summary/models/PortfolioStructuredProductOnshoreDailySnapshot';
import {
  PortfolioSummaryDailySnapshot,
  initModel as initSummaryModel,
} from 'db/portfolio-summary/models/PortfolioSummaryDailySnapshot';
import {
  PortfolioTfexDailySnapshot,
  initModel as initTfexModel,
} from 'db/portfolio-summary/models/PortfolioTfexDailySnapshot';
import {
  PortfolioTfexSummaryDailySnapshot,
  initModel as initTfexSummaryModel,
} from 'db/portfolio-summary/models/PortfolioTfexSummaryDailySnapshot';
import {
  PortfolioThaiEquityDailySnapshot,
  initModel as initThaiEquityModel,
} from 'db/portfolio-summary/models/PortfolioThaiEquityDailySnapshot';
import { SnapshotBase } from 'db/portfolio-summary/models/SnapshotBase';
import {
  StructureNotesCashMovement,
  initModel as initStructureNotesCashMovementModel,
} from 'db/portfolio-summary/models/StructureNotesCashMovement';
import { Model, Op, Sequelize } from 'sequelize';

dayjs.extend(utc);
dayjs.extend(timezone);
interface Payload {
  date: string;
  timezone: string;
  dayOffset: string;
  isPurge: string;
  enabled: string;
}

const run = async (event) => {
  console.info(event);
  const payload = event.body as Payload;
  console.info(payload);
  const enabled = payload.enabled.toLowerCase() === 'true';
  if (!enabled) {
    console.info('Clean up is disabled');
    return {
      body: {
        success: true,
      },
    };
  }

  const isPurge = payload.isPurge.toLowerCase() === 'true';
  const refDate = dayjs(payload.date)
    .subtract(Number(payload.dayOffset), 'day')
    .tz(payload.timezone)
    .format('YYYY-MM-DD');

  console.info('Clean up date', refDate);

  const sequelize = await getSequelize({
    parameterName: 'atlas',
    dbHost: 'PORTFOLIOSUMMARYDATABASE_HOST',
    dbPassword: 'PORTFOLIOSUMMARYDATABASE_PASSWORD',
    dbUsername: 'PORTFOLIOSUMMARYDATABASE_USERNAME',
    dbName: 'portfolio_summary_db',
    dbConfigType: DBConfigType.SecretManager,
    secretManagerType: SecretManagerType.Public,
  });

  initCashModel(sequelize);
  initBondModel(sequelize);
  initBondOffshoreModel(sequelize);
  initGlobalEquityOtcModel(sequelize);
  initGlobalEquityModel(sequelize);
  initGlobalEquityDepositwithdrawModel(sequelize);
  initMutualFundModel(sequelize);
  initStructuredProductModel(sequelize);
  initSummaryModel(sequelize);
  initTfexModel(sequelize);
  initThaiEquityModel(sequelize);
  initExchangeRateModel(sequelize);
  initTfexSummaryModel(sequelize);
  initStructureNotesCashMovementModel(sequelize);
  initStructuredProductOffshoreModel(sequelize);
  initGlobalEquityDepositwithdrawModel(sequelize);
  initGlobalEquityTradeModel(sequelize);
  initGlobalEquityDividendModel(sequelize);

  await Promise.all([
    cleanUpBuilder(PortfolioBondDailySnapshot, refDate, isPurge),
    cleanUpBuilder(PortfolioBondOffshoreDailySnapshot, refDate, isPurge),
    cleanUpBuilder(PortfolioGlobalEquityOtcDailySnapshot, refDate, isPurge),
    cleanUpBuilder(PortfolioMutualFundDailySnapshot, refDate, isPurge),
    cleanUpBuilder(PortfolioStructuredProductDailySnapshot, refDate, isPurge),
    cleanUpBuilder(PortfolioSummaryDailySnapshot, refDate, isPurge),
    cleanUpBuilder(PortfolioTfexDailySnapshot, refDate, isPurge),
    cleanUpBuilder(PortfolioThaiEquityDailySnapshot, refDate, isPurge),
    cleanUpBuilder(PortfolioTfexSummaryDailySnapshot, refDate, isPurge),
    cleanUpBuilder(StructureNotesCashMovement, refDate, isPurge),
    cleanUpBuilder(
      PortfolioStructuredProductOnshoreDailySnapshot,
      refDate,
      isPurge
    ),
    cleanUpExchangeRatePartition(sequelize, refDate, isPurge),
    cleanUpPartition(PortfolioCashDailySnapshot, refDate, isPurge),
    cleanUpPartition(PortfolioGlobalEquityDailySnapshot, refDate, isPurge),
    cleanUpPartition(PortfolioGlobalEquityDepositwithdraw, refDate, isPurge),
    cleanUpPartition(PortfolioGlobalEquityTrade, refDate, isPurge),
    cleanUpPartition(PortfolioGlobalEquityDividend, refDate, isPurge),
  ]);

  return {
    body: {
      success: true,
    },
  };
};

const cleanUpBuilder = async <T extends Model<PortfolioBase>>(
  model: { new (): T } & typeof Model<PortfolioBase>,
  refDate: string,
  isPurge: boolean
) => {
  try {
    await model.destroy({
      where: {
        dateKey: {
          [isPurge ? Op.lt : Op.gte]: Date.parse(refDate),
        },
      },
    });

    console.info('Clean up partition');
  } catch (error) {
    console.error('Error clean up partition:', error);
    throw error;
  }
};

const cleanUpPartition = async <T extends Model<SnapshotBase>>(
  model: { new (): T } & typeof Model<SnapshotBase>,
  refDate: string,
  isPurge: boolean
): Promise<void> => {
  try {
    if (!isPurge) {
      await model.destroy({
        where: { dateKey: { [Op.gte]: Date.parse(refDate) } },
      });
      console.info('Clean up partition');
    }
  } catch (error) {
    console.error('Error clean partition:', error);
    throw error;
  }
};

const cleanUpExchangeRatePartition = async (
  sequelize: Sequelize,
  refDate: string,
  isPurge: boolean
) => {
  try {
    initExchangeRateModel(sequelize);

    await PortfolioExchangeRateDailySnapshot.destroy({
      where: {
        dateKey: {
          [isPurge ? Op.lt : Op.gte]: Date.parse(refDate),
        },
      },
    });

    console.info('Clean up exchange rate partition');
  } catch (error) {
    console.error('Error clean up exchange rate partition:', error);
    throw error;
  }
};

export const main = middyfy(run);
