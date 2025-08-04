import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';

export class ReportHistory extends Model<
  InferCreationAttributes<ReportHistory>,
  InferAttributes<ReportHistory>
> {
  declare id: string;
  declare reportName: string;
  declare userName: string;
  declare filePath: string;
  declare status: string;
  declare dateFrom: Date;
  declare dateTo: Date;
  declare createdAt: Date;
  declare updatedAt: Date;
}

export const initModel = (sequelize) => {
  ReportHistory.init(
    {
      id: {
        type: DataTypes.UUID,
        primaryKey: true,
      },
      reportName: DataTypes.STRING,
      userName: DataTypes.STRING,
      filePath: DataTypes.STRING(2048),
      status: DataTypes.STRING,
      dateFrom: DataTypes.DATEONLY,
      dateTo: DataTypes.DATEONLY,
      createdAt: DataTypes.DATE,
      updatedAt: DataTypes.DATE,
    },
    {
      sequelize,
      tableName: 'report_history',
      underscored: true,
    }
  );
};
