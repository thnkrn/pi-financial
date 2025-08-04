import { BankServiceRecord } from '@cgs-bank/models/bank-service-record';
import { KKPErrorResponse } from '@cgs-bank/external-api-services/kkp-api/kkp-error';
import {
  KKPResponseHeader,
  KKPResponseStatus,
} from '@cgs-bank/external-api-services/kkp-api/common';

export interface KKPBaseRecord
  extends BankServiceRecord,
    KKPResponseHeader,
    KKPResponseStatus,
    KKPErrorResponse {}
