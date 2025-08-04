import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';
import { PortfolioBase } from './PortfolioBase';

export class PortfolioMutualFundDailySnapshot
  extends Model<
    InferCreationAttributes<PortfolioMutualFundDailySnapshot>,
    InferAttributes<PortfolioMutualFundDailySnapshot>
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
  declare fundCategory: string;
  declare amccode: string;
  declare fundName: string;
  declare navDate: Date;
  declare unit: number;
  declare avgNavCost: number;
  declare marketNav: number;
  declare totalCost: number;
  declare marketValue: number;
  declare gainLoss: number;
  declare dateKey: Date;
  declare createdAt: Date;
  declare currency: string;
}

export const initModel = (sequelize) => {
  PortfolioMutualFundDailySnapshot.init(
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
      fundCategory: DataTypes.STRING,
      amccode: DataTypes.STRING,
      fundName: DataTypes.STRING,
      navDate: DataTypes.DATEONLY,
      unit: DataTypes.DECIMAL,
      avgNavCost: DataTypes.DECIMAL,
      marketNav: DataTypes.DECIMAL,
      totalCost: DataTypes.DECIMAL,
      marketValue: DataTypes.DECIMAL,
      gainLoss: DataTypes.DECIMAL,
      dateKey: DataTypes.DATEONLY,
      createdAt: DataTypes.DATE,
      currency: DataTypes.STRING,
    },
    {
      sequelize,
      tableName: 'portfolio_mutual_fund_daily_snapshot',
      underscored: true,
      updatedAt: false,
    }
  );
};
