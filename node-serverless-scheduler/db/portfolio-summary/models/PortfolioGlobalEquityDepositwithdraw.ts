import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';
import { SnapshotBase } from './SnapshotBase';

export class PortfolioGlobalEquityDepositwithdraw
  extends Model<
    InferCreationAttributes<PortfolioGlobalEquityDepositwithdraw>,
    InferAttributes<PortfolioGlobalEquityDepositwithdraw>
  >
  implements SnapshotBase
{
  declare id: string;
  declare type: string;
  declare custcode: string;
  declare tradingAccountNo: string;
  declare currency: string;
  declare fxRate: number;
  declare amountUsd: number;
  declare amountThb: number;
  declare dateKey: Date;
  declare createdAt: Date;
}

export const initModel = (sequelize) => {
  PortfolioGlobalEquityDepositwithdraw.init(
    {
      id: {
        type: DataTypes.STRING,
        primaryKey: true,
      },
      type: DataTypes.STRING,
      custcode: DataTypes.STRING,
      tradingAccountNo: DataTypes.STRING,
      currency: DataTypes.STRING,
      fxRate: DataTypes.NUMBER,
      amountUsd: DataTypes.NUMBER,
      amountThb: DataTypes.NUMBER,
      dateKey: DataTypes.DATEONLY,
      createdAt: DataTypes.DATE,
    },
    {
      sequelize,
      tableName: 'portfolio_global_equity_depositwithdraw_daily_snapshot',
      underscored: true,
      updatedAt: false,
    }
  );
};
