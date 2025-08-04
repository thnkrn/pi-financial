export interface BankServiceStatus {
  status: boolean;
  internalStatusCode: string;
  internalStatusDescription: string;
  externalStatusCode?: string;
  externalStatusDescription?: string;
}
