import { ProductType } from '@functions/atlas/gen-portfolio-statement/utils/etl-utils';
import { ModelStatic, Sequelize } from 'sequelize';
import { PortfolioBase } from './PortfolioBase';
import {
  PortfolioBondDailySnapshot,
  initModel as initPortfolioBondDailySnapshot,
} from './PortfolioBondDailySnapshot';
import {
  PortfolioBondOffshoreDailySnapshot,
  initModel as initPortfolioBondOffshoreDailySnapshot,
} from './PortfolioBondOffshoreDailySnapshot';
import {
  PortfolioCashDailySnapshot,
  initModel as initPortfolioCashDailySnapshot,
} from './PortfolioCashDailySnapshot';
import {
  PortfolioExchangeRateDailySnapshot,
  initModel as initPortfolioExchangeRateDailySnapshot,
} from './PortfolioExchangeRateDailySnapshot';
import {
  PortfolioGlobalEquityDailySnapshot,
  initModel as initPortfolioGlobalEquityDailySnapshot,
} from './PortfolioGlobalEquityDailySnapshot';
import {
  PortfolioGlobalEquityDepositwithdraw,
  initModel as initPortfolioGlobalEquityDepositwithdraw,
} from './PortfolioGlobalEquityDepositwithdraw';
import {
  PortfolioGlobalEquityDividend,
  initModel as initPortfolioGlobalEquityDividend,
} from './PortfolioGlobalEquityDividend';
import {
  PortfolioGlobalEquityOtcDailySnapshot,
  initModel as initPortfolioGlobalEquityOtcDailySnapshot,
} from './PortfolioGlobalEquityOtcDailySnapshot';
import {
  PortfolioGlobalEquityTrade,
  initModel as initPortfolioGlobalEquityTrade,
} from './PortfolioGlobalEquityTrade';
import {
  PortfolioMutualFundDailySnapshot,
  initModel as initPortfolioMutualFundDailySnapshot,
} from './PortfolioMutualFundDailySnapshot';
import {
  PortfolioStructuredProductDailySnapshot,
  initModel as initPortfolioStructuredProductDailySnapshot,
} from './PortfolioStructuredProductDailySnapshot';
import {
  PortfolioStructuredProductOnshoreDailySnapshot,
  initModel as initPortfolioStructuredProductOnshoreDailySnapshot,
} from './PortfolioStructuredProductOnshoreDailySnapshot';
import {
  PortfolioSummaryDailySnapshot,
  initModel as initPortfolioSummaryDailySnapshot,
} from './PortfolioSummaryDailySnapshot';
import {
  PortfolioTfexDailySnapshot,
  initModel as initPortfolioTfexDailySnapshot,
} from './PortfolioTfexDailySnapshot';
import {
  PortfolioTfexSummaryDailySnapshot,
  initModel as initPortfolioTfexSummaryDailySnapshot,
} from './PortfolioTfexSummaryDailySnapshot';
import {
  PortfolioThaiEquityDailySnapshot,
  initModel as initPortfolioThaiEquityDailySnapshot,
} from './PortfolioThaiEquityDailySnapshot';
import { SnapshotBase } from './SnapshotBase';
import {
  StructureNotesCashMovement,
  initModel as initStructureNotesCashMovement,
} from './StructureNotesCashMovement';
import {
  PortfolioMutualFundDividendDailyTransaction,
  initModel as initPortfolioMutualFundDividendDailyTransaction,
} from './PortfolioMutualFundDividendDailyTransaction';

export default {
  PortfolioBondDailySnapshot,
  PortfolioBondOffshoreDailySnapshot,
  PortfolioCashDailySnapshot,
  PortfolioExchangeRateDailySnapshot,
  PortfolioGlobalEquityDailySnapshot,
  PortfolioGlobalEquityDepositwithdraw,
  PortfolioGlobalEquityDividend,
  PortfolioGlobalEquityOtcDailySnapshot,
  PortfolioGlobalEquityTrade,
  PortfolioMutualFundDailySnapshot,
  PortfolioStructuredProductDailySnapshot,
  PortfolioStructuredProductOnshoreDailySnapshot,
  PortfolioSummaryDailySnapshot,
  PortfolioTfexDailySnapshot,
  PortfolioTfexSummaryDailySnapshot,
  PortfolioThaiEquityDailySnapshot,
};

export const ProductModelMap: Record<
  ProductType,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  ModelStatic<PortfolioBase | SnapshotBase | any>
> = {
  [ProductType.Summary]: PortfolioSummaryDailySnapshot,
  [ProductType.Cash]: PortfolioCashDailySnapshot,
  [ProductType.ExchangeRate]: PortfolioExchangeRateDailySnapshot,
  [ProductType.MutualFunds]: PortfolioMutualFundDailySnapshot,
  [ProductType.MutualFundsDividend]:
    PortfolioMutualFundDividendDailyTransaction,
  [ProductType.Debentures]: PortfolioBondDailySnapshot,
  [ProductType.OffshoreDebentures]: PortfolioBondOffshoreDailySnapshot,

  [ProductType.ThaiEquity]: PortfolioThaiEquityDailySnapshot,
  [ProductType.Tfex]: PortfolioTfexDailySnapshot,
  [ProductType.TfexSummary]: PortfolioTfexSummaryDailySnapshot,

  [ProductType.GlobalEquity]: PortfolioGlobalEquityDailySnapshot,
  [ProductType.GlobalEquityOtc]: PortfolioGlobalEquityOtcDailySnapshot,
  [ProductType.GlobalEquityDepositwithdraw]:
    PortfolioGlobalEquityDepositwithdraw,
  [ProductType.GlobalEquityTrade]: PortfolioGlobalEquityTrade,
  [ProductType.GlobalEquityDividend]: PortfolioGlobalEquityDividend,

  [ProductType.StructuredProducts]: PortfolioStructuredProductDailySnapshot,
  [ProductType.StructureNoteCashMovement]: StructureNotesCashMovement,
  [ProductType.StructuredProductsOnshore]:
    PortfolioStructuredProductOnshoreDailySnapshot,
};

export const initModel = (sequelize: Sequelize, product: ProductType) => {
  switch (product) {
    case ProductType.Summary:
      initPortfolioSummaryDailySnapshot(sequelize);
      break;
    case ProductType.Cash:
      initPortfolioCashDailySnapshot(sequelize);
      break;
    case ProductType.ExchangeRate:
      initPortfolioExchangeRateDailySnapshot(sequelize);
      break;
    case ProductType.MutualFunds:
      initPortfolioMutualFundDailySnapshot(sequelize);
      break;
    case ProductType.MutualFundsDividend:
      initPortfolioMutualFundDividendDailyTransaction(sequelize);
      break;
    case ProductType.Debentures:
      initPortfolioBondDailySnapshot(sequelize);
      break;
    case ProductType.OffshoreDebentures:
      initPortfolioBondOffshoreDailySnapshot(sequelize);
      break;
    case ProductType.ThaiEquity:
      initPortfolioThaiEquityDailySnapshot(sequelize);
      break;
    case ProductType.Tfex:
      initPortfolioTfexDailySnapshot(sequelize);
      break;
    case ProductType.TfexSummary:
      initPortfolioTfexSummaryDailySnapshot(sequelize);
      break;
    case ProductType.GlobalEquity:
      initPortfolioGlobalEquityDailySnapshot(sequelize);
      break;
    case ProductType.GlobalEquityOtc:
      initPortfolioGlobalEquityOtcDailySnapshot(sequelize);
      break;
    case ProductType.GlobalEquityDepositwithdraw:
      initPortfolioGlobalEquityDepositwithdraw(sequelize);
      break;
    case ProductType.GlobalEquityTrade:
      initPortfolioGlobalEquityTrade(sequelize);
      break;
    case ProductType.GlobalEquityDividend:
      initPortfolioGlobalEquityDividend(sequelize);
      break;
    case ProductType.StructuredProducts:
      initPortfolioStructuredProductDailySnapshot(sequelize);
      break;
    case ProductType.StructureNoteCashMovement:
      initStructureNotesCashMovement(sequelize);
      break;
    case ProductType.StructuredProductsOnshore:
      initPortfolioStructuredProductOnshoreDailySnapshot(sequelize);
      break;
  }
};
