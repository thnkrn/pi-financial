import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';
import { PortfolioBase } from './PortfolioBase';

export class PortfolioTfexSummaryDailySnapshot
  extends Model<
    InferCreationAttributes<PortfolioTfexSummaryDailySnapshot>,
    InferAttributes<PortfolioTfexSummaryDailySnapshot>
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
  declare equity: number;
  declare excessEquity: number;
  declare dateKey: Date;
  declare createdAt: Date;
}

export const initModel = (sequelize) => {
  PortfolioTfexSummaryDailySnapshot.init(
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
      equity: DataTypes.DECIMAL,
      excessEquity: DataTypes.DECIMAL,
      dateKey: DataTypes.DATEONLY,
      createdAt: DataTypes.DATE,
    },
    {
      sequelize,
      tableName: 'portfolio_tfex_summary_daily_snapshot',
      underscored: true,
      updatedAt: false,
    }
  );
};
