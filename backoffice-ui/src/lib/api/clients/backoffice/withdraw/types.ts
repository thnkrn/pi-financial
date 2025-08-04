import {
  PiBackofficeServiceAPIModelsTransactionHistoryV2Response
} from "@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsTransactionHistoryV2Response";

export type IWithdrawFilters = {
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
  effectiveDateFrom?: string | null
  effectiveDateTo?: string | null
  paymentReceivedDateFrom?: string | null
  paymentReceivedDateTo?: string | null
  createdAtFrom?: string | null
  createdAtTo?: string | null
  productType?: string
}

export interface IGetWithdrawTransactionsRequest {
  filters: IWithdrawFilters
  page: number
  pageSize: number
  orderBy: string
  orderDir: string
}

export interface IGetWithdrawTransactionsResponse {
  transactions: PiBackofficeServiceAPIModelsTransactionHistoryV2Response[]
  page: number
  pageSize: number
  total: number
  orderBy: string
  orderDir: string
}
