export type KKPQrPaymentInquiryResponse = {
  ResultList: Array<KKPQrPaymentInquiryResponseItem>;
};

export enum TransactionStatus {
  PAID = 'PD',
  REVERSED = 'RV',
}

export interface KKPQrPaymentResultResponseItem {
  PaymentDate: string;
  PaymentType: string;
  CustomerName: string;
  PaymentAmount: string;
  BillerReferenceNo: string;
  AccountNo: string;
  AccountBank: string;
  BillerId: string;
  TransactionStatus?: TransactionStatus;
}

export interface KKPQrPaymentInquiryResponseItem {
  PaymentDate: string;
  PaymentType: string;
  CustomerName: string;
  PaymentAmount: number;
  BillerReferenceNo: string;
  AccountNo: string;
  BankCode: string;
  TransactionStatus?: TransactionStatus;
}
