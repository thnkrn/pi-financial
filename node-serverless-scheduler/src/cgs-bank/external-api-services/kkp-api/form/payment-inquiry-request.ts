export interface KKPQrPaymentInquiryRequest {
  billerId: string;
  billReference1: string;
  billReference2?: string;
  billReference3?: string;
  billReference4?: string;
  txnDateFrom?: string;
  txnDateTo?: string;
}
