import { PaymentGenerated, PurchaseRequest } from '@prisma/client';

export class CreatePurchaseRequestInput {
  planId: number;
}

export class CreatePurchaseRequestOutput {
  purchaseRequest: PurchaseRequest;
  paymentGenerated: PaymentGenerated;
}
