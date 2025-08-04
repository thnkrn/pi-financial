export interface SCBDDPaymentInquiryResponse {
  regNumber: string;
  refDateTime: string;
  amount: number;
  currency: string;
  customerRef: string;
  txnNumber: string;
  txnDateTime: string;
  statusCode: string;
  statusDesc: string;
  responseCode: string;
}
