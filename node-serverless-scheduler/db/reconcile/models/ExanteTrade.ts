import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';

export class ExanteTrade extends Model<
  InferCreationAttributes<ExanteTrade>,
  InferAttributes<ExanteTrade>
> {
  declare id: string;
  declare time: Date;
  declare accountId: string;
  declare side: string;
  declare symbolId: string;
  declare isin: string;
  declare type: string;
  declare price: number;
  declare currency: string;
  declare quantity: number;
  declare commission: number;
  declare commissionCurrency: string;
  declare pAndL: number;
  declare tradedVolume: number;
  declare orderId: string;
  declare orderPos: number;
  declare valueDate: Date;
  declare uti: string;
  declare tradeType: string;
}

export const initModel = (sequelize) => {
  ExanteTrade.init(
    {
      id: {
        type: DataTypes.UUID,
        primaryKey: true,
      },
      time: DataTypes.DATE,
      accountId: DataTypes.STRING,
      side: DataTypes.STRING,
      symbolId: DataTypes.STRING,
      isin: DataTypes.STRING,
      type: DataTypes.STRING,
      price: DataTypes.DECIMAL,
      currency: DataTypes.STRING,
      quantity: DataTypes.DECIMAL,
      commission: DataTypes.DECIMAL,
      commissionCurrency: DataTypes.STRING,
      pAndL: DataTypes.DECIMAL,
      tradedVolume: DataTypes.DECIMAL,
      orderId: DataTypes.STRING,
      orderPos: DataTypes.INTEGER,
      valueDate: DataTypes.DATEONLY,
      uti: DataTypes.STRING,
      tradeType: DataTypes.STRING,
    },
    {
      sequelize,
      tableName: 'exante_trades',
      underscored: true,
      createdAt: false,
      updatedAt: false,
    }
  );
};
