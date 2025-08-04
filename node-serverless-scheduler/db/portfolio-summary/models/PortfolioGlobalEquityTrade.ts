import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';
import { SnapshotBase } from './SnapshotBase';

export class PortfolioGlobalEquityTrade
  extends Model<
    InferCreationAttributes<PortfolioGlobalEquityTrade>,
    InferAttributes<PortfolioGlobalEquityTrade>
  >
  implements SnapshotBase
{
  declare id: string;
  declare custcode: string;
  declare tradingAccountNo: string;
  declare exchangeMarketId: string;
  declare sharecode: string;
  declare side: string;
  declare currency: string;
  declare units: number;
  declare avg_price: number;
  declare gross_amount: number;
  declare commission_before_vat_usd: number;
  declare vat_amount: number;
  declare other_fees: number;
  declare wh_tax: number;
  declare net_amount: number;
  declare exchange_rate: number;
  declare net_amount_thb: number;
  declare dateKey: Date;
  declare createdAt: Date;
}

export const initModel = (sequelize) => {
  PortfolioGlobalEquityTrade.init(
    {
      id: {
        type: DataTypes.STRING,
        primaryKey: true,
      },
      custcode: DataTypes.STRING,
      tradingAccountNo: DataTypes.STRING,
      exchangeMarketId: DataTypes.STRING,
      sharecode: DataTypes.STRING,
      side: DataTypes.STRING,
      currency: DataTypes.STRING,
      units: DataTypes.NUMBER,
      avg_price: DataTypes.NUMBER,
      gross_amount: DataTypes.NUMBER,
      commission_before_vat_usd: DataTypes.NUMBER,
      vat_amount: DataTypes.NUMBER,
      other_fees: DataTypes.NUMBER,
      wh_tax: DataTypes.NUMBER,
      net_amount: DataTypes.NUMBER,
      exchange_rate: DataTypes.NUMBER,
      net_amount_thb: DataTypes.NUMBER,
      dateKey: DataTypes.DATEONLY,
      createdAt: DataTypes.DATE,
    },
    {
      sequelize,
      tableName: 'portfolio_global_equity_trade_daily_snapshot',
      underscored: true,
      updatedAt: false,
    }
  );
};
