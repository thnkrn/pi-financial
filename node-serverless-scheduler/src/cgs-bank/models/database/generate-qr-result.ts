import { DataTypes, Model, Optional } from 'sequelize';
import mySqlDbConnection from '@cgs-bank/database-services/mysql-db-connection';
import { KKPBaseRecord } from '@cgs-bank/models/kkp/kkp-base-record';
import { KKPQRRequest } from '@cgs-bank/external-api-services/kkp-api/form/qr-request';
import { KKPQRResponse } from '@cgs-bank/external-api-services/kkp-api/response/kkp-qr-response';
import { InternalReference } from '@cgs-bank/models/common/internal-reference';
import { QRPaymentRequest } from '@cgs-bank/models/common/qr-payment';
import { InternalRefProduct } from '@cgs-bank/models/common/internal-ref-product';

export interface KKPGenerateQrResultAttribute
  extends Optional<QRPaymentRequest, 'internalRef'>,
    InternalReference,
    KKPBaseRecord,
    KKPQRRequest,
    KKPQRResponse {}

export type KKPGenerateQrResultInput = Optional<
  KKPGenerateQrResultAttribute,
  'id'
>;

export class MySqlKKPGenerateQrResult
  extends Model<KKPGenerateQrResultAttribute, KKPGenerateQrResultInput>
  implements KKPGenerateQrResultAttribute
{
  readonly id: number;

  // HEADER
  ChannelCode: string;
  ServiceName: string;
  SystemCode: string;
  TransactionDateTime: string;
  TransactionID: string;

  // QRPaymentRequest
  amount: number;
  transactionNo: string;
  transactionRefCode: string;

  // InternalReference
  customerCode: string;
  product: InternalRefProduct;

  // KKPQRRequest
  billPaymentBillerId: string;
  billPaymentReference1: string;
  billPaymentReference2: string;
  billPaymentReference3: string;
  billPaymentSuffix: string;
  billPaymentTaxId: string;
  creditTransferBankAccount: string;
  creditTransferEWalletID: string;
  creditTransferMobileNumber: string;
  creditTransferTaxID: string;
  transactionAmount: string;

  // KKPQRResponse
  Format: string;
  QRValue: string;

  // Response Status
  ResponseCode: string;
  ResponseMessage: string;

  // Error
  code: string;
  description: string;
  message: string;

  readonly createdAt: Date;
  readonly deletedAt: Date;
  readonly updatedAt: Date;
}

MySqlKKPGenerateQrResult.init(
  {
    id: {
      type: DataTypes.INTEGER.UNSIGNED,
      autoIncrement: true,
      primaryKey: true,
    },
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
    amount: { type: DataTypes.NUMBER },
    transactionNo: { type: DataTypes.STRING },
    transactionRefCode: { type: DataTypes.STRING },
    customerCode: { type: DataTypes.STRING },
    product: { type: DataTypes.STRING },
    billPaymentBillerId: { type: DataTypes.STRING },
    billPaymentReference1: { type: DataTypes.STRING },
    billPaymentReference2: { type: DataTypes.STRING },
    billPaymentReference3: { type: DataTypes.STRING },
    billPaymentSuffix: { type: DataTypes.STRING },
    billPaymentTaxId: { type: DataTypes.STRING },
    creditTransferBankAccount: { type: DataTypes.STRING },
    creditTransferEWalletID: {
      field: 'credit_transfer_e_wallet_id',
      type: DataTypes.STRING,
    },
    creditTransferMobileNumber: { type: DataTypes.STRING },
    creditTransferTaxID: {
      field: 'credit_transfer_tax_id',
      type: DataTypes.STRING,
    },
    transactionAmount: { type: DataTypes.STRING },

    Format: { type: DataTypes.STRING },
    QRValue: {
      field: 'qr_value',
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

    // Error
    code: { type: DataTypes.STRING },
    description: { type: DataTypes.STRING },
    message: { type: DataTypes.STRING },
  },
  {
    tableName: 'kkp_qr_generated_result',
    timestamps: true,
    underscored: true,
    sequelize: mySqlDbConnection,
  }
);
