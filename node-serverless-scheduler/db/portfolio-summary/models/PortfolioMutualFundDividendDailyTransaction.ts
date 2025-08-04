import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';
import { SnapshotBase } from './SnapshotBase';

export class PortfolioMutualFundDividendDailyTransaction
  extends Model<
    InferCreationAttributes<PortfolioMutualFundDividendDailyTransaction>,
    InferAttributes<PortfolioMutualFundDividendDailyTransaction>
  >
  implements SnapshotBase
{
  declare id: string;
  declare paymentDate: Date;
  declare bookClosedDate: Date;
  declare custcode: string;
  declare tradingAccountNo: string;
  declare fundCode: string;
  declare amcCode: string;
  declare unit: number;
  declare dividendRate: number;
  declare dividendAmount: number;
  declare withholdingTax: number;
  declare dividendAmountNet: number;
  declare paymentTypeDescription: string;
  declare bankName: string;
  declare bankAccount: string;
  declare dateKey: Date;
  declare createdAt: Date;
}

export const initModel = (sequelize) => {
  PortfolioMutualFundDividendDailyTransaction.init(
    {
      id: {
        type: DataTypes.STRING,
        primaryKey: true,
      },
      paymentDate: DataTypes.DATEONLY,
      bookClosedDate: DataTypes.DATEONLY,
      custcode: DataTypes.STRING,
      tradingAccountNo: DataTypes.STRING,
      fundCode: DataTypes.STRING,
      amcCode: DataTypes.STRING,
      unit: DataTypes.NUMBER,
      dividendRate: DataTypes.NUMBER,
      dividendAmount: DataTypes.NUMBER,
      withholdingTax: DataTypes.NUMBER,
      dividendAmountNet: DataTypes.NUMBER,
      paymentTypeDescription: DataTypes.STRING,
      bankName: DataTypes.STRING,
      bankAccount: DataTypes.STRING,
      dateKey: DataTypes.DATEONLY,
      createdAt: DataTypes.DATE,
    },
    {
      sequelize,
      tableName: 'portfolio_mutual_fund_dividend_daily_transaction',
      underscored: true,
      updatedAt: false,
    }
  );
};
