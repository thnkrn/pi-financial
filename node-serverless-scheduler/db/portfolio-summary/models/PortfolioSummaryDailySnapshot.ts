import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';
import { PortfolioBase } from './PortfolioBase';

export class PortfolioSummaryDailySnapshot
  extends Model<
    InferCreationAttributes<PortfolioSummaryDailySnapshot>,
    InferAttributes<PortfolioSummaryDailySnapshot>
  >
  implements PortfolioBase
{
  declare custcode: string;
  declare mktid: string;
  declare tradingAccountNo: string;
  declare exchangeMarketId: string;
  declare customerType: string;
  declare customerSubType: string;
  declare accountType: string;
  declare accountTypeCode: string;
  declare y_0: number;
  declare y_1: number;
  declare y_2: number;
  declare y_3: number;
  declare m_0: number;
  declare m_1: number;
  declare m_2: number;
  declare m_3: number;
  declare m_4: number;
  declare m_5: number;
  declare m_6: number;
  declare m_7: number;
  declare m_8: number;
  declare m_9: number;
  declare m_10: number;
  declare m_11: number;
  declare as_of_date: Date;
  declare exchange_rate_as_of: Date;
  declare dateKey: Date;
  declare createdAt: Date;
}

export const initModel = (sequelize) => {
  PortfolioSummaryDailySnapshot.init(
    {
      custcode: {
        type: DataTypes.STRING,
        primaryKey: true,
      },
      mktid: DataTypes.STRING,
      tradingAccountNo: DataTypes.STRING,
      exchangeMarketId: DataTypes.STRING,
      customerType: DataTypes.STRING,
      customerSubType: DataTypes.STRING,
      accountType: DataTypes.STRING,
      accountTypeCode: DataTypes.STRING,
      y_0: DataTypes.DECIMAL,
      y_1: DataTypes.DECIMAL,
      y_2: DataTypes.DECIMAL,
      y_3: DataTypes.DECIMAL,
      m_0: DataTypes.DECIMAL,
      m_1: DataTypes.DECIMAL,
      m_2: DataTypes.DECIMAL,
      m_3: DataTypes.DECIMAL,
      m_4: DataTypes.DECIMAL,
      m_5: DataTypes.DECIMAL,
      m_6: DataTypes.DECIMAL,
      m_7: DataTypes.DECIMAL,
      m_8: DataTypes.DECIMAL,
      m_9: DataTypes.DECIMAL,
      m_10: DataTypes.DECIMAL,
      m_11: DataTypes.DECIMAL,
      as_of_date: DataTypes.DATEONLY,
      exchange_rate_as_of: DataTypes.DATEONLY,
      dateKey: DataTypes.DATEONLY,
      createdAt: DataTypes.DATE,
    },
    {
      sequelize,
      tableName: 'portfolio_summary_daily_snapshot',
      underscored: true,
      updatedAt: false,
    }
  );
};
