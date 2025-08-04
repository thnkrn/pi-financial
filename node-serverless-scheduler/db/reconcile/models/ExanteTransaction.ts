import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';

export class ExanteTransaction extends Model<
  InferCreationAttributes<ExanteTransaction>,
  InferAttributes<ExanteTransaction>
> {
  declare id: string;
  declare transactionId: string;
  declare accountId: string;
  declare symbolId: string;
  declare isin: string;
  declare operationType: string;
  declare when: Date;
  declare sum: number;
  declare asset: string;
  declare eurEquivalent: number;
  declare comment: string;
  declare uuid: string;
  declare parentUuid: string;
}

export const initModel = (sequelize) => {
  ExanteTransaction.init(
    {
      id: {
        type: DataTypes.UUID,
        primaryKey: true,
      },
      transactionId: DataTypes.STRING,
      accountId: DataTypes.STRING,
      symbolId: DataTypes.STRING,
      isin: DataTypes.STRING,
      operationType: DataTypes.STRING,
      when: DataTypes.DATE,
      sum: DataTypes.DECIMAL,
      asset: DataTypes.STRING,
      eurEquivalent: DataTypes.DECIMAL,
      comment: DataTypes.STRING,
      uuid: DataTypes.UUID,
      parentUuid: DataTypes.UUID,
    },
    {
      sequelize,
      tableName: 'exante_transactions',
      underscored: true,
      createdAt: false,
      updatedAt: false,
    }
  );
};
