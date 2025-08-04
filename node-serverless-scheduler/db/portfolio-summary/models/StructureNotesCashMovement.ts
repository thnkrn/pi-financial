import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';
import { PortfolioBase } from './PortfolioBase';

export class StructureNotesCashMovement
  extends Model<
    InferCreationAttributes<StructureNotesCashMovement>,
    InferAttributes<StructureNotesCashMovement>
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

  declare subAccount: string;
  declare transactionDate: Date;
  declare settlementDate: Date;
  declare transactionType: string;
  declare currency: string;
  declare amount: number;
  declare note: string;
  declare description: string;

  declare dateKey: Date;
  declare createdAt: Date;
}

export const initModel = (sequelize) => {
  StructureNotesCashMovement.init(
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

      subAccount: DataTypes.STRING,
      transactionDate: DataTypes.DATEONLY,
      settlementDate: DataTypes.DATEONLY,
      transactionType: DataTypes.STRING,
      currency: DataTypes.STRING,
      amount: DataTypes.DECIMAL,
      note: DataTypes.STRING,
      description: DataTypes.STRING,

      dateKey: DataTypes.DATEONLY,
      createdAt: DataTypes.DATE,
    },
    {
      sequelize,
      tableName: 'structure_note_cash_movement',
      underscored: true,
      updatedAt: false,
    }
  );
};
