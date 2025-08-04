
import {
  DataTypes,
  InferAttributes,
  InferCreationAttributes,
  Model,
} from 'sequelize';

export interface SiriusData {
  sbl_order_id: string;
  account_id: string;
  account_code: string;
  symbol: string;
  amount: number;
  type: string;
  submit_time: string;
  status: string;
  reject_reason?: string;
  update_by: number;
  update_time: string;
  extra_info?: SiriusExtraInfo;
}

export interface SiriusExtraInfo {
  bankCode?: string;
  isBorrowed?: boolean;
  instrumentId?: number;
  placeOrderIp?: string;
  cancelOrderIp?: string;
  changeOrderIp?: string;
  bankBranchCode?: string;
  bankAccountCode?: string;
}
export interface SblOrder {
  createdAt: string;
  customerCode: string;
  orderId: number;
  rejectedReason?: string;
  reviewerId?: string | null;
  status: string;
  symbol: string;
  tradingAccountId: string;
  tradingAccountNo: string;
  type: string;
  updatedAt?: string | null;
  volume: number;
}

export class PiSblOrder extends Model<
  InferCreationAttributes<PiSblOrder>,
  InferAttributes<PiSblOrder>
> {
  declare id: string;
  declare createdAt: string;
  declare customerCode: string;
  declare orderId: number;
  declare rejectedReason?: string;
  declare reviewerId?: string | null;
  declare status: string;
  declare symbol: string;
  declare tradingAccountId: string;
  declare tradingAccountNo: string;
  declare type: string;
  declare updatedAt?: string | null;
  declare volume: number;
}

export const initModel = (sequelize) => {
  PiSblOrder.init(
    {
      id: {
        type: DataTypes.UUID,
        defaultValue: DataTypes.UUIDV4,
        primaryKey: true,
      },
      createdAt: DataTypes.STRING,
      customerCode: DataTypes.STRING,
      orderId: DataTypes.INTEGER,
      rejectedReason: DataTypes.STRING,
      reviewerId: DataTypes.STRING,
      status: DataTypes.STRING,
      symbol: DataTypes.STRING,
      tradingAccountId: DataTypes.STRING,
      tradingAccountNo: DataTypes.STRING,
      type: DataTypes.STRING,
      updatedAt: DataTypes.STRING,
      volume: DataTypes.INTEGER,
    },
    {
      sequelize,
      tableName: 'sbl_orders',
      underscored: true,
      createdAt: false,
      updatedAt: false,
    }
  );
};