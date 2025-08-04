import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';
import { SnapshotBase } from './SnapshotBase';

export class PortfolioGlobalEquityDividend
  extends Model<
    InferCreationAttributes<PortfolioGlobalEquityDividend>,
    InferAttributes<PortfolioGlobalEquityDividend>
  >
  implements SnapshotBase
{
  declare id: string;
  declare custcode: string;
  declare tradingAccountNo: string;
  declare sharecode: string;
  declare currency: string;
  declare units: number;
  declare dividen_per_share: number;
  declare amount: number;
  declare tax_amount: number;
  declare net_amount: number;
  declare fx_rate: number;
  declare net_amount_usd: number;
  declare dateKey: Date;
  declare createdAt: Date;
}

export const initModel = (sequelize) => {
  PortfolioGlobalEquityDividend.init(
    {
      id: {
        type: DataTypes.STRING,
        primaryKey: true,
      },
      custcode: DataTypes.STRING,
      tradingAccountNo: DataTypes.STRING,
      sharecode: DataTypes.STRING,
      currency: DataTypes.STRING,
      units: DataTypes.NUMBER,
      dividen_per_share: DataTypes.NUMBER,
      amount: DataTypes.NUMBER,
      tax_amount: DataTypes.NUMBER,
      net_amount: DataTypes.NUMBER,
      fx_rate: DataTypes.NUMBER,
      net_amount_usd: DataTypes.NUMBER,
      dateKey: DataTypes.DATEONLY,
      createdAt: DataTypes.DATE,
    },
    {
      sequelize,
      tableName: 'portfolio_global_equity_dividend_daily_snapshot',
      underscored: true,
      updatedAt: false,
    }
  );
};
