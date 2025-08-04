export interface CreateTaxInvoiceRequest {
  purchaseRequestId: number;
  taxInvoiceNo: string;
  amount: number;
  amountExVat: number;
  vat: number;
  customerName: string;
  customerId: string;
}
