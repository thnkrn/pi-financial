export interface SCBDDPaymentRequest {
  refNumber: string;
  refDateTime: string;
  amount: number;
  currency: string;
  accountNumber: string;
  customerRef?: string;
}
