import { PiBackofficeServiceAPIModelsTransactionV2DetailResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsTransactionV2DetailResponse'
import { PiBackofficeServiceAPIModelsTicketDetailResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsTicketDetailResponse'
import { PiBackofficeServiceDomainAggregateModelsTicketAggregateTicketState } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceDomainAggregateModelsTicketAggregateTicketState'
import { PiBackofficeServiceAPIModelsTransferCashDetailResponse  } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsTransferCashDetailResponse';
import Decimal from 'decimal.js'
import {PiBackofficeServiceAPIModelsTransferCashResponse} from "@pi-financial/backoffice-srv";

export interface IGetTicketDetailsResponse {
  tickets: PiBackofficeServiceAPIModelsTicketDetailResponse[]
}

export interface IGetTransactionDetailsResponse {
  transaction: PiBackofficeServiceAPIModelsTransactionV2DetailResponse
}

export interface IGetTransferCashDetailsResponse {
  transaction: PiBackofficeServiceAPIModelsTransferCashDetailResponse
}

export interface ICustomerProfile {
  customerCode?: string | null
  customerAccountName?: string | null
}

export interface IBaseStatusAction {
  status?: string | null
  errorCode?: string | null
  callToAction?: string | null
  failedReason?: string | null
  requestedAmount?: Decimal | null
}

export interface IStatusAction extends IBaseStatusAction {
  refundSuccessfulAt: Date | null
  paymentReceivedAmount?: Decimal | null
}

export interface ITransactionDetail {
  transactionNo?: string | null
  senderBankName?: string | null
  senderBankAccountNumber?: string | null
  senderBankAccountName?: string | null
  requestedAmount: Decimal | null
  requestedCurrency?: string | null
  transactionType?: string | null
  effectiveDate: Date | null
  createdAt: Date | null
}

export interface ITransferCashCustomerProfile {
  customerName?: string | null
}

export interface ITransferCashAccountInfo {
  transferFromAccountCode?: string | null
  transferFromExchangeMarket?: string | null
  transferToAccountCode?: string | null
  transferToExchangeMarket?: string | null
}

export interface ITransferCashTransactionDetail {
  transactionNo?: string | null
  transactionType?: string | null
  requestedAmount: Decimal | null
  effectiveDate: Date | null
  createdAt: Date | null
}

export interface IProductDetailChannel {
  channelName?: string | null
  customerAccountNumber?: string | null
  accountType?: string | null
}

export interface IQrDepositDetail {
  state: string | null
  paymentReceivedDateTime: Date | null
  paymentReceivedAmount: Decimal | null
  fee: Decimal | null
  failedReason: string | null
  createdAt: Date | null
}

export interface IOddDepositDetail {
  state: string | null
  paymentReceivedDateTime: Date | null
  paymentReceivedAmount: Decimal | null
  fee: Decimal | null
  failedReason: string | null
  createdAt: Date | null
}

export interface IAtsDepositDetail {
  state: string | null
  paymentReceivedDateTime: Date | null
  paymentReceivedAmount: Decimal | null
  fee: Decimal | null
  failedReason: string | null
  createdAt: Date | null
}

export interface IBillPaymentDepositDetail {
  state: string | null
  paymentReceivedDateTime: Date | null
  paymentReceivedAmount: Decimal | null
  fee: Decimal | null
  reference1: string | null
  reference2: string | null
  billPaymentTransactionRef: string | null
  failedReason: string | null
}

export interface IOddWithdrawDetail {
  state: string | null
  paymentDisbursedDateTime: Date | null
  paymentDisbursedAmount: Decimal | null
  fee: Decimal | null
  failedReason: string | null
  createdAt: Date | null
}

export interface IAtsWithdrawDetail {
  state: string | null
  paymentDisbursedDateTime: Date | null
  paymentDisbursedAmount: Decimal | null
  fee: Decimal | null
  failedReason: string | null
  createdAt: Date | null
}

export interface IUpBackDetail {
  state: string | null
  failedReason: string | null
  createdAt: Date | null
}

export interface IGlobalTransferDetail {
  state: string | null
  globalAccount: string | null
  requestedFxCurrency: string | null
  requestedFxRate: Decimal | null
  fxConfirmedExchangeRate: Decimal | null
  fxConfirmedDateTime: Date | null
  transferAmount: Decimal | null
  transferFee: Decimal | null
  transferCurrency: string | null
  transferCompleteTime: Date | null
  failedReason: string | null
  createdAt: Date | null
}

export interface IRecoveryDetail {
  state: string | null
  globalAccount: string | null
  transferAmount: Decimal | null
  transferCurrency: string | null
  transferFromAccount: string | null
  transferToAccount: string | null
  transferCompleteTime: Date | null
  failedReason: string | null
  createdAt: Date | null
}

export interface IRefundDetail {
  id: string | null
  amount: Decimal | null
  transferToAccountNo: string | null
  transferToAccountName: string | null
  fee: Decimal | null
  createdAt: Date | null
}

export interface IAction {
  label?: string | null
  value?: string | null
}

export interface ITicket {
  timestamp?: Date | null
  ticketId?: string | null
  name?: string | null
  email?: string | null
  status?: string | null
  remark?: string | null
  actionBy?: string | null
  ticketDescription?: string | null
  ticketStatus?: string | null
}

export interface IGetTransactionResponse {
  customerProfile: ICustomerProfile
  statusAction: IStatusAction
  transactionDetail: ITransactionDetail
  productDetail: IProductDetailChannel
  qrDeposit: IQrDepositDetail | null
  oddDeposit: IOddDepositDetail | null
  atsDeposit: IAtsDepositDetail | null
  billPaymentDeposit: IBillPaymentDepositDetail | null
  oddWithdraw: IOddWithdrawDetail | null
  atsWithdraw: IAtsWithdrawDetail | null
  upback: IUpBackDetail | null
  globalTransfer: IGlobalTransferDetail | null
  recovery: IRecoveryDetail | null
  refundInfo: IRefundDetail | null
  actions?: IAction[]
  pendingTicketNo?: string | null
  makerType: ITicket[]
  checkerType: ITicket[]
}

export interface IGetTransferCashTransactionResponse {
  statusAction: IBaseStatusAction
  customerProfile: ITransferCashCustomerProfile
  accountInfo: ITransferCashAccountInfo
  transactionDetail: ITransferCashTransactionDetail
  actions?: IAction[]
  pendingTicketNo?: string | null
  makerType: ITicket[]
  checkerType: ITicket[]
}

export type ITransferCashFilters = {
  status?: string
  state?: string
  transactionNo?: string
  transferFromAccountCode?: string
  transferToAccountCode?: string
  transferFromExchangeMarket?: string
  transferToExchangeMarket?: string
  otpConfirmedDateFrom?: string | null
  otpConfirmedDateTo?: string | null
  createdAtFrom?: string | null
  createdAtTo?: string | null
}

export interface IGetTransferCashTransactionsRequest {
  filters: ITransferCashFilters
  page: number
  pageSize: number
  orderBy: string
  orderDir: string
}

export interface IGetTransferCashTransactionsResponse {
  transactions: PiBackofficeServiceAPIModelsTransferCashResponse[]
  page: number
  pageSize: number
  total: number
  orderBy: string
  orderDir: string
}

export interface ICreateTicketRequest {
  transactionNo: string
  transactionType: string
  method: string
  remark: string
  payload?: string | null
}

export interface ICreateTicketResponse {
  ticket: PiBackofficeServiceDomainAggregateModelsTicketAggregateTicketState
}

export interface IVerifyTicketRequest {
  method: string
  remark: string
  payload?: string | null
}

export interface IVerifyTicketResponse {
  ticket: PiBackofficeServiceDomainAggregateModelsTicketAggregateTicketState
  errorMsg?: string
  isSuccess?: boolean
}

export interface IPayloadResponse {
  action?: string
  payload?: string
}

export interface UpdateBillPaymentReferencePayload {
  oldReference?: string
  newReference?: string
}
