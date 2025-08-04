import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';

export class PortfolioExchangeRateDailySnapshot extends Model<
  InferCreationAttributes<PortfolioExchangeRateDailySnapshot>,
  InferAttributes<PortfolioExchangeRateDailySnapshot>
> {
  declare currency: string;
  declare exchangeRate: number;
  declare dateKey: Date;
  declare createdAt: Date;
}

export const initModel = (sequelize) => {
  PortfolioExchangeRateDailySnapshot.init(
    {
      currency: DataTypes.STRING,
      exchangeRate: DataTypes.DECIMAL,
      dateKey: {
        type: DataTypes.DATEONLY,
        primaryKey: true,
      },
      createdAt: DataTypes.DATE,
    },
    {
      sequelize,
      tableName: 'portfolio_exchange_rate_daily_snapshot',
      underscored: true,
      updatedAt: false,
    }
  );
};
