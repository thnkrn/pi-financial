export interface KKPQRRequest {
  creditTransferMobileNumber?: string;
  creditTransferTaxID?: string;
  creditTransferEWalletID?: string;
  creditTransferBankAccount?: string;
  billPaymentBillerId: string;
  billPaymentTaxId: string;
  billPaymentSuffix: string;
  billPaymentReference1: string;
  billPaymentReference2?: string;
  billPaymentReference3?: string;
  transactionAmount?: string;
}
