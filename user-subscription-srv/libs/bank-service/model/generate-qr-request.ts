export enum GenerateQrPropose {
  DEPOSIT = 'DEPOSIT',
  PAYMENT = 'PAYMENT',
}

export interface GenerateQrRequest {
  transactionNo: string;
  expiredTimeInMinute: number;
  amount: number;
  transactionRefCode: string;
  internalRef: {
    customerCode: string;
    product: string;
  };
  propose: GenerateQrPropose;
}
