import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';
import { PortfolioBase } from './PortfolioBase';

export class PortfolioGlobalEquityOtcDailySnapshot
  extends Model<
    InferCreationAttributes<PortfolioGlobalEquityOtcDailySnapshot>,
    InferAttributes<PortfolioGlobalEquityOtcDailySnapshot>
  >
  implements PortfolioBase
{
  declare custcode: string;
  declare tradingAccountNo: string;
  declare exchangeMarketId: string;
  declare customerType: string;
  declare customerSubType: string;
  declare accountType: string;
  declare accountTypeCode: string;
  declare sharecode: string;
  declare currency: string;
  declare stockExchangeMarkets: string;
  declare units: number;
  declare avgCost: number;
  declare marketPrice: number;
  declare totalCost: number;
  declare marketValue: number;
  declare gainLoss: number;
  declare dateKey: Date;
  declare createdAt: Date;
}

export const initModel = (sequelize) => {
  PortfolioGlobalEquityOtcDailySnapshot.init(
    {
      custcode: {
        type: DataTypes.STRING,
        primaryKey: true,
      },
      tradingAccountNo: DataTypes.STRING,
      exchangeMarketId: DataTypes.STRING,
      customerType: DataTypes.STRING,
      customerSubType: DataTypes.STRING,
      accountType: DataTypes.STRING,
      accountTypeCode: DataTypes.STRING,
      sharecode: DataTypes.STRING,
      currency: DataTypes.STRING,
      stockExchangeMarkets: DataTypes.STRING,
      units: DataTypes.DECIMAL,
      avgCost: DataTypes.DECIMAL,
      marketPrice: DataTypes.DECIMAL,
      totalCost: DataTypes.DECIMAL,
      marketValue: DataTypes.DECIMAL,
      gainLoss: DataTypes.DECIMAL,
      dateKey: DataTypes.DATEONLY,
      createdAt: DataTypes.DATE,
    },
    {
      sequelize,
      tableName: 'portfolio_global_equity_otc_daily_snapshot',
      underscored: true,
      updatedAt: false,
    }
  );
};
