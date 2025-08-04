import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';

export class PortfolioJobHistory extends Model<
  InferCreationAttributes<PortfolioJobHistory>,
  InferAttributes<PortfolioJobHistory>
> {
  declare id: string;
  declare identificationHash: string;
  declare custcode: string;
  declare marketingId: string;
  declare metadata: string;
  declare sendDate: Date;
  declare status: string;
  declare createdAt: Date;
}

export const initModel = (sequelize) => {
  PortfolioJobHistory.init(
    {
      id: {
        type: DataTypes.UUID,
        primaryKey: true,
      },
      identificationHash: DataTypes.STRING,
      custcode: DataTypes.STRING,
      marketingId: DataTypes.STRING,
      metadata: DataTypes.JSON,
      sendDate: DataTypes.DATE,
      status: DataTypes.STRING,
      createdAt: DataTypes.DATE,
    },
    {
      sequelize,
      tableName: 'portfolio_job_history',
      underscored: true,
      updatedAt: false,
    }
  );
};
