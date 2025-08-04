import { InternalReference } from '@cgs-bank/models/common/internal-reference';

export interface DDPaymentRequest {
  transactionNo: string;
  transactionRefCode: string;
  accountNo: string;
  destinationBankCode: string;
  amount: number;
  internalRef: InternalReference;
}

export const DDPaymentRequestScheme = {
  type: 'object',
  properties: {
    transactionNo: { type: 'string' },
    amount: {
      type: 'number',
      minimum: 0.01,
      maximum: 2000000,
      multipleOf: 0.01,
    },
    accountNo: { type: 'string' },
    destinationBankCode: { type: 'string' },
    transactionRefCode: { type: 'string' },
    internalRef: {
      type: 'object',
      properties: {
        customerCode: { type: 'string' },
        product: { type: 'string' },
      },
    },
  },
  required: [
    'transactionNo',
    'amount',
    'transactionRefCode',
    'internalRef',
    'accountNo',
    'destinationBankCode',
  ],
};

export interface DDPaymentResponse {
  transactionNo: string;
  transactionRefCode: string;
  amount: number;
  externalRefTime: string;
  externalRefCode: string;
}

export interface DDPaymentInquiry {
  transactionNo: string;
  amount: number;
  externalRefTime: string;
  externalRefCode: string;
}

export const DDPaymentInquiryRequestScheme = {
  type: 'object',
  properties: {
    transactionNo: { type: 'string' },
    amount: {
      type: 'number',
      minimum: 0.01,
      maximum: 2000000,
      multipleOf: 0.01,
    },
    externalRefTime: { type: 'string' },
    externalRefCode: { type: 'string' },
  },
  required: ['transactionNo', 'amount', 'externalRefTime', 'externalRefCode'],
};
