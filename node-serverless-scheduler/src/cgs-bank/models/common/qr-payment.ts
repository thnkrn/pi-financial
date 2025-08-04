import { InternalReference } from '@cgs-bank/models/common/internal-reference';

export interface QRPaymentRequestExtended extends QRPaymentRequest {
  expiredTimeInMinute: number;
}

export interface QRPaymentRequest {
  transactionNo: string;
  amount: number;
  transactionRefCode: string;
  internalRef: InternalReference;
}

export const QRPaymentRequestScheme = {
  type: 'object',
  properties: {
    transactionNo: { type: 'string' },
    propose: {
      type: 'string',
      enum: ['DEPOSIT', 'PAYMENT'],
    },
    expiredTimeInMinute: {
      type: 'number',
      minimum: 1,
      multipleOf: 1,
    },
    amount: {
      type: 'number',
      minimum: 0.01,
      maximum: 2000000,
      multipleOf: 0.01,
    },
    transactionRefCode: { type: 'string' },
    internalRef: {
      type: 'object',
      properties: {
        customerCode: { type: 'string' },
        product: { type: 'string' },
      },
    },
  },
  required: ['transactionNo', 'amount', 'transactionRefCode', 'internalRef'],
};

export enum QRPropose {
  DEPOSIT = 'DEPOSIT',
  PAYMENT = 'PAYMENT',
}

export interface QRPaymentResponse {
  QRValue: string;
}

export interface QRPaymentInquiry {
  transactionNo: string;
}

export interface QRPaymentInquiryResponse {
  transactionNo: string;
  paymentAmount: string;
  paymentDateTime: string;
  bankCode: string;
  accountNo: string;
  customerName: string;
}

export const QRQRPaymentInquiryScheme = {
  type: 'object',
  properties: {
    transactionNo: { type: 'string' },
  },
  required: ['transactionNo'],
};

export interface QRPaymentCallbackResponse {
  transactionNo: string;
  amount: number;
  transactionRefCode: string;
  paymentTimestamp: string;
  payerAccountNumber: string;
  payerBankCode: string;
  payerName: string;
}

export interface WalletQrPaymentCallbackRequest {
  isSuccess: boolean;
  amount: number;
  customerCode: string;
  product: string;
  transactionNo: string;
  transactionRefCode: string;
  paymentDateTime: string;
  payerName: string;
  payerBankCode: string;
  payerAccountNo: string;
}
