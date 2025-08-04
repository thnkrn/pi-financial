export enum PaymentStatus {
  Completed = 'Completed',
  Processing = 'Processing',
  Default = 'Default',
}

export class PaymentHistoryResponse {
  transactionNo: number;
  paymentDate: string;
  clientCustcode: string;
  mt5Period: string;
  validUntil: string | null;
  amount: number;
  status: PaymentStatus;
}

export class PaymentHistoryListResponse {
  items: PaymentHistoryResponse[];
}
