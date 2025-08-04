export interface BankServiceStatus {
  status: boolean;
  internalStatusCode: string;
  internalStatusDescription: string;
  externalStatusCode?: string;
  externalStatusDescription?: string;
}

export interface BankServiceResponseWrapper<T> {
  status: BankServiceStatus;
  data?: T;
}
