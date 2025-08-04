export interface KkpBillPaymentResponse {
  TransactionId: string;
  PaymentDate: string;
  PaymentType: string;
  CustomerName: string;
  PaymentAmount: number;
  ReferenceNo1: string;
  ReferenceNo2: string;
  AccountNo: string;
  AccountBank: string;
  BillerId: string;
  ChannelId: string;
}
