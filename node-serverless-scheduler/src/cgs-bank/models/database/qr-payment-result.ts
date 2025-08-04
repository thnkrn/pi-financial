import mySqlDbConnection from '@cgs-bank/database-services/mysql-db-connection';
import {
  KKPQrPaymentResultResponseItem,
  TransactionStatus,
} from '@cgs-bank/external-api-services/kkp-api/response/kkp-qr-payment-inquiry-response';
import { BankServiceRecord } from '@cgs-bank/models/bank-service-record';
import { DataTypes, Model, Optional } from 'sequelize';
import { decrypt, encrypt } from '@cgs-bank/utils/aes-coder';

interface KKPQrPaymentResultAttribute
  extends BankServiceRecord,
    KKPQrPaymentResultResponseItem {}

export type KKPQrPaymentResultInput = Optional<
  KKPQrPaymentResultAttribute,
  'id'
>;

export class MySqlKKPQrPaymentResult
  extends Model<KKPQrPaymentResultAttribute, KKPQrPaymentResultInput>
  implements KKPQrPaymentResultAttribute
{
  readonly id: number;

  // KKPQrPaymentResultResponseItem
  BillerReferenceNo: string;

  CustomerName: string;
  PaymentAmount: string;
  PaymentDate: string;
  PaymentType: string;

  AccountBank: string;
  AccountNo: string;
  BillerId: string;

  TransactionStatus: TransactionStatus;

  readonly createdAt: Date;
  readonly deletedAt: Date;
  readonly updatedAt: Date;
}

MySqlKKPQrPaymentResult.init(
  {
    id: {
      type: DataTypes.INTEGER.UNSIGNED,
      autoIncrement: true,
      primaryKey: true,
    },
    BillerReferenceNo: {
      field: 'biller_reference_no',
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
    TransactionStatus: {
      field: 'transaction_status',
      type: DataTypes.STRING,
    },
  },
  {
    tableName: 'kkp_qr_payment_result',
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
        result: MySqlKKPQrPaymentResult | MySqlKKPQrPaymentResult[] | null
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
