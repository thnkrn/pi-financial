import { DataTypes, Model } from 'sequelize';
import dbConnection from '@cgs-bank/database-services/db-connection';

interface UserPaymentGatewayAttribute {
  id: number;
  username: number;
  password: string;

  createdAt?: Date;
  updatedAt?: Date;
}

export type UserPaymentGatewayOutput = Required<UserPaymentGatewayAttribute>;

export class UserPaymentGateway
  extends Model<UserPaymentGatewayAttribute, UserPaymentGatewayOutput>
  implements UserPaymentGatewayAttribute
{
  public readonly id;
  public username: number;
  public password: string;

  public readonly createdAt: Date;
  public readonly updatedAt: Date;
}

UserPaymentGateway.init(
  {
    id: {
      type: DataTypes.INTEGER.UNSIGNED,
      autoIncrement: true,
      primaryKey: true,
    },
    username: { type: DataTypes.STRING, allowNull: false, unique: true },
    password: { type: DataTypes.STRING, allowNull: false },
  },
  {
    timestamps: true,
    underscored: true,
    freezeTableName: true,
    tableName: 'user_payment_gateway',
    sequelize: dbConnection,
    paranoid: true,
  }
);
