import { KKPBaseRecord } from '@cgs-bank/models/kkp/kkp-base-record';
import { KKPLookupConfirmRequest } from '@cgs-bank/external-api-services/kkp-api/form/kkp-lookup-confirm';
import { KKPOddPaymentInquiryResponse } from '@cgs-bank/external-api-services/kkp-api/response/kkp-odd-payment-inquiry-response';
import { DataTypes, Model, Optional } from 'sequelize';
import mySqlDbConnection from '@cgs-bank/database-services/mysql-db-connection';
import { DDPaymentInquiry } from '@cgs-bank/models/common/dd-payment';

export interface KKPDDPaymentInquiryResultAttributed
  extends DDPaymentInquiry,
    KKPBaseRecord,
    KKPLookupConfirmRequest,
    KKPOddPaymentInquiryResponse {}

export type KKPDDPaymentInquiryResultInput = Optional<
  KKPDDPaymentInquiryResultAttributed,
  'id'
>;

export class MySqlKKPDDPaymentInquiryResult
  extends Model<
    KKPDDPaymentInquiryResultAttributed,
    KKPDDPaymentInquiryResultInput
  >
  implements KKPDDPaymentInquiryResultAttributed
{
  readonly id: number;

  // DDPaymentInquiry
  amount: number;
  externalRefCode: string;
  externalRefTime: string;
  transactionNo: string;

  // KKP Response Header
  ChannelCode: string;
  ServiceName: string;
  SystemCode: string;
  TransactionDateTime: string;
  TransactionID: string;

  // KKP Response Status
  ResponseCode: string;
  ResponseMessage: string;

  // KKP Error Status
  code: string;
  description: string;
  message: string;

  // KKP LookupConfirmRequest
  txnReferenceNo: string;

  // KKP Payment Inquiry Result
  FeeAmount: number;
  StatusCode: string;
  StatusMessage: string;

  readonly createdAt: Date;
  readonly deletedAt: Date;
  readonly updatedAt: Date;
}

MySqlKKPDDPaymentInquiryResult.init(
  {
    id: {
      type: DataTypes.INTEGER.UNSIGNED,
      autoIncrement: true,
      primaryKey: true,
    },

    // DDPaymentInquiry
    amount: { type: DataTypes.NUMBER },
    externalRefCode: { type: DataTypes.STRING },
    externalRefTime: { type: DataTypes.STRING },
    transactionNo: { type: DataTypes.STRING },

    // KKP Response Header
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

    // KKP Response Status
    ResponseCode: {
      field: 'response_code',
      type: DataTypes.STRING,
    },
    ResponseMessage: {
      field: 'response_message',
      type: DataTypes.STRING,
    },

    // KKP Error Status
    code: { type: DataTypes.STRING },
    description: { type: DataTypes.STRING },
    message: { type: DataTypes.STRING },

    // KKP LookupConfirmRequest
    txnReferenceNo: { type: DataTypes.STRING },

    // KKP Payment Inquiry Result
    FeeAmount: {
      field: 'fee_amount',
      type: DataTypes.NUMBER,
    },
    StatusCode: {
      field: 'status_code',
      type: DataTypes.STRING,
    },
    StatusMessage: {
      field: 'status_message',
      type: DataTypes.STRING,
    },
  },
  {
    tableName: 'kkp_payment_inquiry',
    timestamps: true,
    underscored: true,
    sequelize: mySqlDbConnection,
  }
);
