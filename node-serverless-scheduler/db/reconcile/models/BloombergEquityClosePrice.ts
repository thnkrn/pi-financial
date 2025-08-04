import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';

export class BloombergEquityCloseprice extends Model<
  InferCreationAttributes<BloombergEquityCloseprice>,
  InferAttributes<BloombergEquityCloseprice>
> {
  declare id: string;
  declare dlRequestId: string;
  declare dlRequestName: string;
  declare dlSnapshotStartTime: Date;
  declare dlSnapshotTz: string;
  declare identifier: string;
  declare rc: number;
  declare pxCloseDt: Date;
  declare idExchSymbol: string;
  declare name: string;
  declare pxLastEod: number;
  declare crncy: string;
  declare compositeExchCode: string;
  declare idIsin: string;
  declare lastUpdateDateEod: Date;
  declare icbSupersectorName: string;
  declare icbSectorName: string;
}

export const initModel = (sequelize) => {
  BloombergEquityCloseprice.init(
    {
      id: {
        type: DataTypes.UUID,
        defaultValue: DataTypes.UUIDV4,
        primaryKey: true,
      },
      dlRequestId: DataTypes.STRING,
      dlRequestName: DataTypes.STRING,
      dlSnapshotStartTime: DataTypes.DATE,
      dlSnapshotTz: DataTypes.STRING,
      identifier: DataTypes.STRING,
      rc: DataTypes.INTEGER,
      pxCloseDt: DataTypes.DATEONLY,
      idExchSymbol: DataTypes.STRING,
      name: DataTypes.STRING,
      pxLastEod: DataTypes.DECIMAL(10, 4),
      crncy: DataTypes.STRING,
      compositeExchCode: DataTypes.STRING,
      idIsin: DataTypes.STRING,
      lastUpdateDateEod: DataTypes.DATEONLY,
      icbSupersectorName: DataTypes.STRING,
      icbSectorName: DataTypes.STRING,
    },
    {
      sequelize,
      tableName: 'bloomberg_equity_closeprice',
      underscored: true,
      createdAt: false,
      updatedAt: false,
    }
  );
};
