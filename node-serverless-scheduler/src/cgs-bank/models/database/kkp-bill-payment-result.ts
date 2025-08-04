import mySqlDbConnection from '@cgs-bank/database-services/mysql-db-connection';
import { BankServiceRecord } from '@cgs-bank/models/bank-service-record';
import { DataTypes, Model, Optional } from 'sequelize';
import { decrypt, encrypt } from '@cgs-bank/utils/aes-coder';
import { KkpBillPaymentResponse } from '@cgs-bank/external-api-services/kkp-api/response/kkp-bill-payment-response';

interface KKPBillPaymentResultAttribute
  extends BankServiceRecord,
    KkpBillPaymentResponse {}

export type KKPBillPaymentResultInput = Optional<
  KKPBillPaymentResultAttribute,
  'id'
>;

export class MySqlKKPBillPaymentResult
  extends Model<KKPBillPaymentResultAttribute, KKPBillPaymentResultInput>
  implements KKPBillPaymentResultAttribute
{
  readonly id: number;

  TransactionId: string;
  ChannelId: string;
  CustomerName: string;
  PaymentAmount: number;
  ReferenceNo1: string;
  ReferenceNo2: string;
  PaymentDate: string;
  PaymentType: string;

  AccountBank: string;
  AccountNo: string;
  BillerId: string;

  readonly createdAt: Date;
  readonly deletedAt: Date;
  readonly updatedAt: Date;
}

MySqlKKPBillPaymentResult.init(
  {
    id: {
      type: DataTypes.UUID,
      defaultValue: DataTypes.UUIDV4,
      primaryKey: true,
    },
    TransactionId: {
      field: 'transaction_id',
      type: DataTypes.STRING,
    },
    ChannelId: {
      field: 'channel_id',
      type: DataTypes.STRING,
    },
    AccountBank: {
      field: 'account_bank',
      type: DataTypes.STRING,
    },
    AccountNo: {
      field: 'account_no',
      type: DataTypes.STRING,
    },
    BillerId: {
      field: 'biller_id',
      type: DataTypes.STRING,
    },
    CustomerName: {
      field: 'customer_name',
      type: DataTypes.STRING,
    },
    PaymentAmount: {
      field: 'payment_amount',
      type: DataTypes.DECIMAL(65, 8),
    },
    ReferenceNo1: {
      field: 'reference_no_1',
      type: DataTypes.STRING,
    },
    ReferenceNo2: {
      field: 'reference_no_2',
      type: DataTypes.STRING,
    },
    PaymentDate: {
      field: 'payment_date',
      type: DataTypes.STRING,
    },
    PaymentType: {
      field: 'payment_type',
      type: DataTypes.STRING,
    },
  },
  {
    tableName: 'kkp_bill_payment_result',
    timestamps: true,
    underscored: true,
    sequelize: mySqlDbConnection,
    hooks: {
      beforeCreate(attributes, _) {
        attributes.dataValues.CustomerName = encrypt(
          attributes.dataValues.CustomerName
        );
      },
      beforeUpdate: (attributes, _) => {
        attributes.dataValues.CustomerName = encrypt(
          attributes.dataValues.CustomerName
        );
      },
      afterFind: (
        result: MySqlKKPBillPaymentResult | MySqlKKPBillPaymentResult[] | null
      ) => {
        if (Array.isArray(result)) {
          result.forEach((obj) => {
            if (obj.dataValues.CustomerName)
              obj.dataValues.CustomerName = decrypt(
                obj.dataValues.CustomerName
              );
          });
        } else if (result.dataValues.CustomerName) {
          result.dataValues.CustomerName = decrypt(
            result.dataValues.CustomerName
          );
        }
      },
    },
  }
);
