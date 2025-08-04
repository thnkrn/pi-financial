import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';

export class UnitHolder extends Model<
  InferCreationAttributes<UnitHolder>,
  InferAttributes<UnitHolder>
> {
  declare id: string;
  declare amcCode: string;
  declare createdAt: Date;
  declare updatedAt: Date;
  declare customerCode: string;
  declare tradingAccountNo: string;
  declare unitHolderId: string;
  declare unitHolderType: string;
  declare status: string;
}

export const initModel = (sequelize) => {
  UnitHolder.init(
    {
      id: {
        type: DataTypes.UUID,
        primaryKey: true,
      },
      amcCode: DataTypes.STRING,
      createdAt: DataTypes.DATE,
      updatedAt: DataTypes.DATE,
      customerCode: DataTypes.STRING,
      tradingAccountNo: DataTypes.STRING,
      unitHolderId: DataTypes.STRING,
      unitHolderType: DataTypes.STRING,
      status: DataTypes.STRING,
    },
    {
      sequelize,
      tableName: 'unit_holders',
      underscored: true,
      createdAt: 'createdAt',
      updatedAt: 'updatedAt',
    }
  );
};
