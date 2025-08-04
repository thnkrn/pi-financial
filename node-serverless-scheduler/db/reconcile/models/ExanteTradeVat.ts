import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';

export class ExanteTradeVat extends Model<
  InferCreationAttributes<ExanteTradeVat>,
  InferAttributes<ExanteTradeVat>
> {
  declare id: string;
  declare transactionId: string;
  declare accountId: string;
  declare symbolId: string;
  declare orderId: string;
  declare isin: string;
  declare operationType: string;
  declare when: Date;
  declare sum: number;
  declare commissionBeforeVat: number;
  declare otherFees: number;
  declare vatAmount: number;
  declare totalCommission: number;
  declare exanteCommissionWithOtherFees: number;
  declare partnerRebate: number;
  declare asset: string;
}

export const initModel = (sequelize) => {
  ExanteTradeVat.init(
    {
      id: {
        type: DataTypes.UUID,
        primaryKey: true,
      },
      transactionId: DataTypes.STRING,
      accountId: DataTypes.STRING,
      symbolId: DataTypes.STRING,
      orderId: DataTypes.STRING,
      isin: DataTypes.STRING,
      operationType: DataTypes.STRING,
      when: DataTypes.DATE,
      sum: DataTypes.DECIMAL,
      commissionBeforeVat: DataTypes.DECIMAL,
      otherFees: DataTypes.DECIMAL,
      vatAmount: DataTypes.DECIMAL,
      totalCommission: DataTypes.DECIMAL,
      exanteCommissionWithOtherFees: DataTypes.DECIMAL,
      partnerRebate: DataTypes.DECIMAL,
      asset: DataTypes.STRING,
    },
    {
      sequelize,
      tableName: 'exante_trade_vats',
      underscored: true,
      createdAt: false,
      updatedAt: false,
    }
  );
};
