import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';
import { PortfolioBase } from './PortfolioBase';

export class PortfolioStructuredProductOnshoreDailySnapshot
  extends Model<
    InferCreationAttributes<PortfolioStructuredProductOnshoreDailySnapshot>,
    InferAttributes<PortfolioStructuredProductOnshoreDailySnapshot>
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
  declare productType: string;
  declare issuer: string;
  declare note: string;
  declare underlying: string;
  declare tradeDate: Date;
  declare maturityDate: Date;
  declare tenor: number;
  declare capitalProtection: string;
  declare yield: number;
  declare currency: string;
  declare exchangeRate: number;
  declare notionalValue: number;
  declare marketValue: number;
  declare dateKey: Date;
  declare createdAt: Date;
}

export const initModel = (sequelize) => {
  PortfolioStructuredProductOnshoreDailySnapshot.init(
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
      productType: DataTypes.STRING,
      issuer: DataTypes.STRING,
      note: DataTypes.STRING,
      underlying: DataTypes.STRING,
      tradeDate: DataTypes.DATEONLY,
      maturityDate: DataTypes.DATEONLY,
      tenor: DataTypes.INTEGER,
      capitalProtection: DataTypes.STRING,
      yield: DataTypes.DECIMAL,
      currency: DataTypes.STRING,
      exchangeRate: DataTypes.DECIMAL,
      notionalValue: DataTypes.DECIMAL,
      marketValue: DataTypes.DECIMAL,
      dateKey: DataTypes.DATEONLY,
      createdAt: DataTypes.DATE,
    },
    {
      sequelize,
      tableName: 'portfolio_structured_product_onshore_daily_snapshot',
      underscored: true,
      updatedAt: false,
    }
  );
};
