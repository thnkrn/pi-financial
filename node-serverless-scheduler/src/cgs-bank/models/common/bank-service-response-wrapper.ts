import { BankServiceStatus } from '@cgs-bank/models/common/bank-service-status';

export interface BankServiceResponseWrapper<T> {
  status: BankServiceStatus;
  data?: T;
}
