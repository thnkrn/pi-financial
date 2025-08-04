import { BankServiceRecord } from '@cgs-bank/models/bank-service-record';
import {
  SCBBaseResponseAttribute,
  SCBResultStatus,
} from '@cgs-bank/external-api-services/scb-api/common';

export interface ScbBaseRecord
  extends BankServiceRecord,
    SCBBaseResponseAttribute,
    SCBResultStatus {}
