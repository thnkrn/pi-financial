import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';

export class BmaInstrument extends Model<
  InferCreationAttributes<BmaInstrument>,
  InferAttributes<BmaInstrument>
> {
  declare id: string;
  declare symbol: string;
  declare issueRating: string;
  declare companyRating: string;
  declare couponType: string;
  declare couponRate: number;
  declare issuedDate: string;
  declare maturityDate: string;
  declare ttm: number;
  declare outstanding: number;
  declare createdAt: string;
}

export const initModel = (sequelize) => {
  BmaInstrument.init(
    {
      id: {
        type: DataTypes.UUID,
        defaultValue: DataTypes.UUIDV4,
        primaryKey: true,
      },
      symbol: {
        type: DataTypes.STRING,
        allowNull: false,
      },
      issueRating: {
        type: DataTypes.STRING,
      },
      companyRating: {
        type: DataTypes.STRING,
      },
      couponType: {
        type: DataTypes.STRING,
      },
      couponRate: {
        type: DataTypes.DECIMAL,
      },
      issuedDate: {
        type: DataTypes.DATEONLY,
      },
      maturityDate: {
        type: DataTypes.DATEONLY,
      },
      ttm: {
        type: DataTypes.DECIMAL,
      },
      outstanding: {
        type: DataTypes.DECIMAL,
      },
      createdAt: DataTypes.DATE,
    },
    {
      sequelize,
      tableName: 'thai_bma_instruments',
      underscored: true,
      createdAt: true,
      updatedAt: false,
    }
  );
};
