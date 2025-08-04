import { KKPBaseRecord } from '@cgs-bank/models/kkp/kkp-base-record';
import { DataTypes, Model, Optional } from 'sequelize';
import { KKPConfirmLookupResponse } from '@cgs-bank/external-api-services/kkp-api/response/kkp-confirm-lookup-response';
import { KKPLookupConfirmRequest } from '@cgs-bank/external-api-services/kkp-api/form/kkp-lookup-confirm';
import { KKPLookupRequest } from '@cgs-bank/external-api-services/kkp-api/form/kkp-lookup-request';
import mySqlDbConnection from '@cgs-bank/database-services/mysql-db-connection';
import { DDPaymentRequest } from '@cgs-bank/models/common/dd-payment';
import { InternalReference } from '@cgs-bank/models/common/internal-reference';
import { InternalRefProduct } from '@cgs-bank/models/common/internal-ref-product';
import { decrypt, encrypt } from '@cgs-bank/utils/aes-coder';

export interface KKPConfirmPaymentResultAttributed
  extends Optional<DDPaymentRequest, 'internalRef'>,
    InternalReference,
    KKPBaseRecord,
    KKPLookupRequest,
    KKPLookupConfirmRequest,
    KKPConfirmLookupResponse {}

export type KKPConfirmPaymentResultInput = Optional<
  KKPConfirmPaymentResultAttributed,
  'id'
>;

export class MySqlKKPConfirmPaymentResult
  extends Model<KKPConfirmPaymentResultAttributed, KKPConfirmPaymentResultInput>
  implements KKPConfirmPaymentResultAttributed
{
  readonly id: number;

  // DDPaymentRequest
  accountNo: string;
  amount: number;
  destinationBankCode: string;
  transactionNo: string;
  transactionRefCode: string;

  // InternalReference
  customerCode: string;
  product: InternalRefProduct;

  // KKP Response HEADER
  ChannelCode: string;
  ServiceName: string;
  SystemCode: string;
  TransactionDateTime: string;
  TransactionID: string;

  // KKP Response Status
  ResponseCode: string;
  ResponseMessage: string;

  // KKP Lookup Request
  effectiveDate: string;
  transferAmount: number;

  // KKP LookupConfirmRequest
  receivingAccountNo: string;
  receivingBankCode: string;
  txnReferenceNo: string;

  // KKP ConfirmLookupResponse
  FeeAmount: number;

  // KKP ERROR
  code: string;
  description: string;
  message: string;

  readonly createdAt: Date;
  readonly deletedAt: Date;
  readonly updatedAt: Date;
}

MySqlKKPConfirmPaymentResult.init(
  {
    id: {
      type: DataTypes.INTEGER.UNSIGNED,
      autoIncrement: true,
      primaryKey: true,
    },

    // DDPaymentRequest
    accountNo: { type: DataTypes.STRING },
    amount: { type: DataTypes.NUMBER },
    destinationBankCode: { type: DataTypes.STRING },
    transactionNo: { type: DataTypes.STRING },
    transactionRefCode: { type: DataTypes.STRING },

    // InternalReference
    customerCode: { type: DataTypes.STRING },
    product: { type: DataTypes.STRING },

    // KKP Response HEADER
    ChannelCode: {
      field: 'channel_code',
      type: DataTypes.STRING,
    },
    ServiceName: {
      field: 'service_name',
      type: DataTypes.STRING,
    },
    SystemCode: {
      field: 'system_code',
      type: DataTypes.STRING,
    },

    TransactionDateTime: {
      field: 'transaction_date_time',
      type: DataTypes.STRING,
    },
    TransactionID: {
      field: 'transaction_id',
      type: DataTypes.STRING,
    },

    ResponseCode: {
      field: 'response_code',
      type: DataTypes.STRING,
    },
    ResponseMessage: {
      field: 'response_message',
      type: DataTypes.STRING,
    },

    effectiveDate: { type: DataTypes.STRING },
    transferAmount: { type: DataTypes.NUMBER },

    receivingAccountNo: { type: DataTypes.STRING },
    receivingBankCode: { type: DataTypes.STRING },
    txnReferenceNo: { type: DataTypes.STRING },

    FeeAmount: {
      field: 'fee_amount',
      type: DataTypes.NUMBER,
    },

    code: { type: DataTypes.STRING },
    description: { type: DataTypes.STRING },
    message: { type: DataTypes.STRING },
  },
  {
    tableName: 'kkp_payment_result',
    timestamps: true,
    underscored: true,
    sequelize: mySqlDbConnection,
    hooks: {
      beforeCreate(attributes, _) {
        attributes.dataValues.accountNo = encrypt(
          attributes.dataValues.accountNo
        );
        attributes.dataValues.receivingAccountNo = encrypt(
          attributes.dataValues.receivingAccountNo
        );
      },
      beforeUpdate: (attributes, _) => {
        attributes.dataValues.accountNo = encrypt(
          attributes.dataValues.accountNo
        );
        attributes.dataValues.receivingAccountNo = encrypt(
          attributes.dataValues.receivingAccountNo
        );
      },
      afterFind: (
        result:
          | MySqlKKPConfirmPaymentResult
          | MySqlKKPConfirmPaymentResult[]
          | null
      ) => {
        if (Array.isArray(result)) {
          result.forEach((obj) => {
            if (obj.dataValues.accountNo)
              obj.dataValues.accountNo = decrypt(obj.dataValues.accountNo);
            if (obj.dataValues.receivingAccountNo)
              obj.dataValues.receivingAccountNo = decrypt(
                obj.dataValues.receivingAccountNo
              );
          });
        } else {
          if (result.dataValues.accountNo)
            result.dataValues.accountNo = decrypt(result.dataValues.accountNo);
          if (result.dataValues.receivingAccountNo)
            result.dataValues.receivingAccountNo = decrypt(
              result.dataValues.receivingAccountNo
            );
        }
      },
    },
  }
);
