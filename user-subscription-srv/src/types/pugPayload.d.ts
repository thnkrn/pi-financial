type ManualUpdateCustCodePayload = {
  bodyMessage: string;
  referenceCode: string;
  currentCustCode: string;
  newCustCode: string;
  amount?: string;
  status?: string;
  updatedAt?: string;
  effectiveDate?: string;
  accounts?: string[];
  userSubscription?: string;
  purchaseRequest?: string;
};
