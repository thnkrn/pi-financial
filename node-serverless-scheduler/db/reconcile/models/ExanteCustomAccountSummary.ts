import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';

export class ExanteAccountSummaryCustom extends Model<
  InferCreationAttributes<ExanteAccountSummaryCustom>,
  InferAttributes<ExanteAccountSummaryCustom>
> {
  declare id: string;
  declare date: Date;
  declare account: string;
  declare instrument: string;
  declare iso: string;
  declare instrumentName: string;
  declare qty: number;
  declare avgPrice: number;
  declare price: number;
  declare ccy: string;
  declare pAndL: number;
  declare pAndLInEur: number;
  declare pAndLPercent: number;
  declare value: number;
  declare valueInEur: number;
  declare dailyPAndL: number;
  declare dailyPAndLInEur: number;
  declare dailyPAndLPercent: number;
  declare isin: string;
}

export const initModel = (sequelize) => {
  ExanteAccountSummaryCustom.init(
    {
      id: {
        type: DataTypes.UUID,
        primaryKey: true,
      },
      date: DataTypes.DATE,
      account: DataTypes.STRING,
      instrument: DataTypes.STRING,
      iso: DataTypes.STRING,
      instrumentName: DataTypes.STRING,
      qty: DataTypes.DECIMAL,
      avgPrice: DataTypes.DECIMAL,
      price: DataTypes.DECIMAL,
      ccy: DataTypes.STRING,
      pAndL: DataTypes.DECIMAL,
      pAndLInEur: DataTypes.DECIMAL,
      pAndLPercent: DataTypes.DECIMAL,
      value: DataTypes.DECIMAL,
      valueInEur: DataTypes.DECIMAL,
      dailyPAndL: DataTypes.DECIMAL,
      dailyPAndLInEur: DataTypes.DECIMAL,
      dailyPAndLPercent: DataTypes.DECIMAL,
      isin: DataTypes.STRING,
    },
    {
      sequelize,
      tableName: 'exante_custom_account_summaries',
      underscored: true,
      createdAt: false,
      updatedAt: false,
    }
  );
};
