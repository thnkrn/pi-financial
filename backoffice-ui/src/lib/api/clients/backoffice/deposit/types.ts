import {
  PiBackofficeServiceAPIModelsTransactionHistoryV2Response
} from "@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsTransactionHistoryV2Response";

export type IDepositFilters = {
  channel?: string
  transactionType?: string
  accountType?: string
  responseCodeId?: string
  bankCode?: string
  accountNumber?: string
  customerCode?: string
  accountCode?: string
  transactionNumber?: string
  status?: string
  effectiveDateFrom?: string
  effectiveDateTo?: string
  paymentReceivedDateFrom?: string | null
  paymentReceivedDateTo?: string | null
  createdAtFrom?: string | null
  createdAtTo?: string | null
  productType?: string
}

export interface IGetDepositTransactionsRequest {
  filters: IDepositFilters
  page: number
  pageSize: number
  orderBy: string
  orderDir: string
}

export interface IGetDepositTransactionsResponse {
  transactions: PiBackofficeServiceAPIModelsTransactionHistoryV2Response[]
  page: number
  pageSize: number
  total: number
  orderBy: string
  orderDir: string
}
