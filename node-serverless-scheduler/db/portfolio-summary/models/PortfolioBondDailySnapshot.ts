import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';
import { PortfolioBase } from './PortfolioBase';

export class PortfolioBondDailySnapshot
  extends Model<
    InferCreationAttributes<PortfolioBondDailySnapshot>,
    InferAttributes<PortfolioBondDailySnapshot>
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
  declare marketType: string;
  declare assetName: string;
  declare issuer: string;
  declare maturityDate: Date;
  declare initialDate: Date;
  declare couponRate: number;
  declare totalCost: number;
  declare marketValue: number;
  declare dateKey: Date;
  declare createdAt: Date;
}

export const initModel = (sequelize) => {
  PortfolioBondDailySnapshot.init(
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
      marketType: DataTypes.STRING,
      assetName: DataTypes.STRING,
      issuer: DataTypes.STRING,
      maturityDate: DataTypes.DATEONLY,
      initialDate: DataTypes.DATEONLY,
      couponRate: DataTypes.DECIMAL,
      totalCost: DataTypes.DECIMAL,
      marketValue: DataTypes.DECIMAL,
      dateKey: DataTypes.DATEONLY,
      createdAt: DataTypes.DATE,
    },
    {
      sequelize,
      tableName: 'portfolio_bond_daily_snapshot',
      underscored: true,
      updatedAt: false,
    }
  );
};
