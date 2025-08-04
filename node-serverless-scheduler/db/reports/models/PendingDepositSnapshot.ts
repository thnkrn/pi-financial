import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';

export class PendingDepositSnapshot extends Model<
  InferCreationAttributes<PendingDepositSnapshot>,
  InferAttributes<PendingDepositSnapshot>
> {
  declare id: string;
  declare paymentReceivedDatetime: Date;
  declare senderAccount: string;
  declare bankCode: string;
  declare senderChannel: string;
  declare transactionNumber: string;
  declare customerBankName: string;
}

export const initModel = (sequelize) => {
  PendingDepositSnapshot.init(
    {
      id: {
        type: DataTypes.UUID,
        primaryKey: true,
      },
      paymentReceivedDatetime: DataTypes.DATE,
      senderAccount: DataTypes.STRING,
      bankCode: DataTypes.STRING,
      senderChannel: DataTypes.STRING,
      transactionNumber: DataTypes.STRING,
      customerBankName: DataTypes.STRING,
    },
    {
      sequelize,
      modelName: 'pending_deposit_snapshot',
      underscored: true,
    }
  );
};
