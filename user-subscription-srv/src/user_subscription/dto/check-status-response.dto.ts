export class CheckStatusResponse {
  hasThaiEquityAccount: boolean;
  mt5: MT5Status;
  activeCustomerCode: string;
  customerCodes: string[];
}

export class MT5Status {
  planStatus: MT5PlanStatus;
  planExpiredDate?: string;
  lastSubscription?: string;
}

export enum MT5PlanStatus {
  trial = 'trial',
  active = 'active',
  processing = 'processing',
  expired = 'expired',
}
