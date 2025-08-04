import { SCBDDPaymentRequest } from '@cgs-bank/external-api-services/scb-api/request/scb-dd-payment-request';

export interface SCBDDPaymentResponse extends SCBDDPaymentRequest {
  txnNumber: string;
  txnDateTime: string;
  statusCode: string;
  statusDesc: string;
  responseCode: string;
}
