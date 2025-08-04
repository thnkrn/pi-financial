import { backofficeAxiosInstance } from '@/lib/api'
import { PiBackofficeServiceAPIModelsResponseCodeActionsResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsResponseCodeActionsResponse'
import Decimal from 'decimal.js'
import {
  ICreateTicketRequest,
  ICreateTicketResponse,
  IGetTicketDetailsResponse,
  IGetTransactionDetailsResponse,
  IGetTransactionResponse,
  IGetTransferCashDetailsResponse,
  IGetTransferCashTransactionResponse,
  IGetTransferCashTransactionsRequest,
  IGetTransferCashTransactionsResponse,
  IPayloadResponse,
  ITicket,
  IVerifyTicketRequest,
  IVerifyTicketResponse,
} from './types'

export const getTicketDetails = async (transactionNo: string): Promise<IGetTicketDetailsResponse> => {
  const instance = await backofficeAxiosInstance()
  const res = await instance.get(`tickets/transactions/${transactionNo}`)

  return {
    tickets: res?.data?.data,
  }
}

export const getTransactionDetail = async (
  transactionNo: string,
) : Promise<IGetTransactionDetailsResponse> => {
  const instance = await backofficeAxiosInstance()
  const res = await instance.get(`transactions/${transactionNo}`)

  return {
    transaction: res?.data?.data,
  }
}

export const getTransferCashTransactionDetail = async (
  transactionNo: string,
) : Promise<IGetTransferCashDetailsResponse> => {
  const instance = await backofficeAxiosInstance()
  const res = await instance.get(`transfer-cash/${transactionNo}`)

  return {
    transaction: res?.data?.data,
  }
}

export const getTransferCashTransaction = async (
  transactionNo: string
): Promise<IGetTransferCashTransactionResponse> => {
  const transactionDetails = await getTransferCashTransactionDetail(transactionNo)
  const tickets = await getTicketDetails(transactionNo)
  const pendingTicketNo = tickets?.tickets?.find(ticket => ticket?.status?.toLowerCase() === 'pending')?.ticketNo

  const makerType: ITicket[] = tickets?.tickets?.map(ticket => ({
    timestamp: ticket?.requestedAt ? new Date(ticket?.requestedAt) : null,
    ticketId: ticket?.ticketNo,
    name: `${ticket?.maker?.firstName} ${ticket?.maker?.lastName}`,
    email: ticket?.maker?.email?.split('@')[0],
    status: ticket?.makerAction?.name,
    remark: ticket?.makerRemark,
    actionBy: 'Maker',
    ticketDescription: ticket?.responseCode?.description,
    ticketStatus: ticket?.status && ticket.status?.charAt(0).toUpperCase() + ticket.status?.slice(1),
  }))

  const checkerType: ITicket[] = tickets?.tickets
    ?.filter(ticket => ticket.checker !== null)
    .map(ticket => ({
      timestamp: ticket?.checkedAt ? new Date(ticket?.checkedAt) : null,
      ticketId: ticket?.ticketNo,
      name: `${ticket?.checker?.firstName} ${ticket?.checker?.lastName}`,
      email: ticket?.checker?.email?.split('@')[0],
      status: ticket?.checkerAction?.name,
      remark: ticket?.checkerRemark,
      actionBy: 'Checker',
      ticketDescription: ticket?.responseCode?.description,
      ticketStatus: ticket?.status && ticket.status?.charAt(0).toUpperCase() + ticket.status?.slice(1),
    }))

  return {
    customerProfile: {
      customerName: transactionDetails?.transaction?.customerName,
    },
    accountInfo: {
      transferFromAccountCode: transactionDetails?.transaction?.transferFromAccountCode,
      transferFromExchangeMarket: transactionDetails?.transaction?.transferFromExchangeMarket?.name,
      transferToAccountCode: transactionDetails?.transaction?.transferToAccountCode,
      transferToExchangeMarket: transactionDetails?.transaction?.transferToExchangeMarket?.name,
    },
    transactionDetail: {
      transactionNo: transactionDetails?.transaction?.transactionNo,
      transactionType: 'TransferCash',
      requestedAmount: transactionDetails?.transaction?.amount
        ? new Decimal(transactionDetails?.transaction?.amount)
        : null,
      effectiveDate: transactionDetails?.transaction?.createdAt
        ? new Date(transactionDetails?.transaction?.createdAt)
        : null,
      createdAt: transactionDetails?.transaction?.createdAt
        ? new Date(transactionDetails?.transaction?.createdAt)
        : null,
    },
    statusAction: {
      status: transactionDetails?.transaction?.status,
      errorCode: transactionDetails?.transaction?.responseCode?.description,
      callToAction: transactionDetails?.transaction?.responseCode?.suggestion,
      failedReason: transactionDetails?.transaction?.failedReason ?? null,
      requestedAmount: transactionDetails?.transaction?.amount
        ? new Decimal(transactionDetails?.transaction?.amount)
        : null,
    },
    actions: transactionDetails?.transaction?.responseCode?.actions?.map(
      (action: PiBackofficeServiceAPIModelsResponseCodeActionsResponse) => ({
        label: action?.name,
        value: action?.alias,
      })
    ),
    pendingTicketNo,
    makerType,
    checkerType,
  }
}

export const getTransferCashTransactions = async (payload: IGetTransferCashTransactionsRequest):
  Promise<IGetTransferCashTransactionsResponse> => {
  const queryParams = getRequestQueryParams(payload)
  const instance = await backofficeAxiosInstance()
  const res = await instance.get(`transfer-cash/paginate${queryParams}`)

  return {
    transactions: res?.data?.data,
    page: res?.data?.page,
    pageSize: res?.data?.pageSize,
    total: res?.data?.total,
    orderBy: res?.data?.orderBy,
    orderDir: res?.data?.orderDir,
  }
}

const getRequestQueryParams = (request: IGetTransferCashTransactionsRequest): string => {
  const { filters, page, pageSize, orderBy, orderDir } = request

  const filtersQueryString = Object.entries(filters)
    .map(([key, value]) => {
      if (value !== undefined && value !== null) {
        return `Filters.${key}=${encodeURIComponent(value)}`
      }
      return null
    })
    .filter(Boolean)
    .join("&")

  const otherQueryString = `Page=${page}&PageSize=${pageSize}&OrderBy=${encodeURIComponent(orderBy)}&OrderDir=${encodeURIComponent(orderDir)}`

  const queryString = [filtersQueryString, otherQueryString].filter(Boolean).join("&")

  return `?${queryString}`
};

export const getTransaction = async (
  transactionNo: string
): Promise<IGetTransactionResponse> => {
  const transactionDetails = await getTransactionDetail(transactionNo)
  const tickets = await getTicketDetails(transactionNo)
  const pendingTicketNo = tickets?.tickets?.find(ticket => ticket?.status?.toLowerCase() === 'pending')?.ticketNo

  const makerType: ITicket[] = tickets?.tickets?.map(ticket => ({
    timestamp: ticket?.requestedAt ? new Date(ticket?.requestedAt) : null,
    ticketId: ticket?.ticketNo,
    name: `${ticket?.maker?.firstName} ${ticket?.maker?.lastName}`,
    email: ticket?.maker?.email?.split('@')[0],
    status: ticket?.makerAction?.name,
    remark: ticket?.makerRemark,
    actionBy: 'Maker',
    ticketDescription: ticket?.responseCode?.description,
    ticketStatus: ticket?.status && ticket.status?.charAt(0).toUpperCase() + ticket.status?.slice(1),
  }))

  const checkerType: ITicket[] = tickets?.tickets
    ?.filter(ticket => ticket.checker !== null)
    .map(ticket => ({
      timestamp: ticket?.checkedAt ? new Date(ticket?.checkedAt) : null,
      ticketId: ticket?.ticketNo,
      name: `${ticket?.checker?.firstName} ${ticket?.checker?.lastName}`,
      email: ticket?.checker?.email?.split('@')[0],
      status: ticket?.checkerAction?.name,
      remark: ticket?.checkerRemark,
      actionBy: 'Checker',
      ticketDescription: ticket?.responseCode?.description,
      ticketStatus: ticket?.status && ticket.status?.charAt(0).toUpperCase() + ticket.status?.slice(1),
    }))

  return {
    customerProfile: {
      customerCode: transactionDetails?.transaction?.customerCode ?? null,
      customerAccountName: transactionDetails?.transaction?.customerName ?? null,
    },
    statusAction: {
      status: transactionDetails?.transaction?.status,
      errorCode: transactionDetails?.transaction?.responseCode?.description,
      callToAction: transactionDetails?.transaction?.responseCode?.suggestion,
      failedReason: transactionDetails?.transaction?.failedReason ?? null,
      refundSuccessfulAt: transactionDetails?.transaction?.refundInfo?.createdAt
        ? new Date(transactionDetails?.transaction?.refundInfo?.createdAt)
        : null,
      requestedAmount: transactionDetails?.transaction?.requestedAmount
        ? new Decimal(transactionDetails?.transaction?.requestedAmount)
        : null,
      paymentReceivedAmount: transactionDetails?.transaction?.paymentReceivedAmount
        ? new Decimal(transactionDetails?.transaction?.paymentReceivedAmount)
        : null,
    },
    transactionDetail: {
      transactionNo: transactionDetails?.transaction?.transactionNo,
      senderBankName: transactionDetails?.transaction?.bankName,
      senderBankAccountNumber: transactionDetails?.transaction?.bankAccountNo,
      senderBankAccountName: transactionDetails?.transaction?.bankAccountName,
      requestedCurrency: transactionDetails?.transaction?.requestedCurrency,
      transactionType: transactionDetails?.transaction?.transactionType,
      requestedAmount: transactionDetails?.transaction?.requestedAmount
        ? new Decimal(transactionDetails?.transaction?.requestedAmount)
        : null,
      effectiveDate: transactionDetails?.transaction?.effectiveDateTime
        ? new Date(transactionDetails?.transaction?.effectiveDateTime)
        : null,
      createdAt: transactionDetails?.transaction?.createdAt
        ? new Date(transactionDetails?.transaction?.createdAt)
        : null,
    },
    productDetail: {
      channelName: transactionDetails?.transaction?.channel?.name,
      customerAccountNumber: transactionDetails?.transaction?.accountCode,
      accountType: transactionDetails?.transaction?.product?.name,
    },
    qrDeposit: transactionDetails?.transaction?.qrDeposit
      ? {
          state: transactionDetails?.transaction?.qrDeposit.state ?? null,
          paymentReceivedAmount: transactionDetails?.transaction?.qrDeposit.paymentReceivedAmount
            ? new Decimal(transactionDetails?.transaction?.qrDeposit.paymentReceivedAmount)
            : null,
          paymentReceivedDateTime: transactionDetails?.transaction?.qrDeposit.paymentReceivedDateTime
            ? new Date(transactionDetails?.transaction?.qrDeposit.paymentReceivedDateTime)
            : null,
          fee: transactionDetails?.transaction?.qrDeposit.fee
            ? new Decimal(transactionDetails?.transaction?.qrDeposit.fee)
            : null,
          failedReason: transactionDetails?.transaction?.qrDeposit.failedReason ?? null,
          createdAt: transactionDetails?.transaction?.qrDeposit.createdAt
            ? new Date(transactionDetails?.transaction?.qrDeposit.createdAt)
            : null,
        }
      : null,
    oddDeposit: transactionDetails?.transaction?.oddDeposit
      ? {
          state: transactionDetails?.transaction?.oddDeposit.state ?? null,
          paymentReceivedAmount: transactionDetails?.transaction?.oddDeposit.paymentReceivedAmount
            ? new Decimal(transactionDetails?.transaction?.oddDeposit.paymentReceivedAmount)
            : null,
          paymentReceivedDateTime: transactionDetails?.transaction?.oddDeposit.paymentReceivedDateTime
            ? new Date(transactionDetails?.transaction?.oddDeposit.paymentReceivedDateTime)
            : null,
          fee: transactionDetails?.transaction?.oddDeposit.fee
            ? new Decimal(transactionDetails?.transaction?.oddDeposit.fee)
            : null,
          failedReason: transactionDetails?.transaction?.oddDeposit.failedReason ?? null,
          createdAt: transactionDetails?.transaction?.createdAt
            ? new Date(transactionDetails?.transaction?.createdAt)
            : null,
        }
      : null,
    atsDeposit: transactionDetails?.transaction?.atsDeposit
      ? {
          state: transactionDetails?.transaction?.atsDeposit.state ?? null,
          paymentReceivedAmount: transactionDetails?.transaction?.atsDeposit.paymentReceivedAmount
            ? new Decimal(transactionDetails?.transaction?.atsDeposit.paymentReceivedAmount)
            : null,
          paymentReceivedDateTime: transactionDetails?.transaction?.atsDeposit.paymentReceivedDateTime
            ? new Date(transactionDetails?.transaction?.atsDeposit.paymentReceivedDateTime)
            : null,
          fee: transactionDetails?.transaction?.atsDeposit.fee
            ? new Decimal(transactionDetails?.transaction?.atsDeposit.fee)
            : null,
          failedReason: transactionDetails?.transaction?.atsDeposit.failedReason ?? null,
          createdAt: transactionDetails?.transaction?.createdAt
            ? new Date(transactionDetails?.transaction?.createdAt)
            : null,
        }
      : null,
    billPaymentDeposit: transactionDetails?.transaction?.billPayment
      ? {
          state: transactionDetails?.transaction?.billPayment.state ?? null,
          paymentReceivedAmount: transactionDetails?.transaction?.billPayment.paymentReceivedAmount
            ? new Decimal(transactionDetails?.transaction?.billPayment.paymentReceivedAmount)
            : null,
          paymentReceivedDateTime: transactionDetails?.transaction?.billPayment.paymentReceivedDateTime
            ? new Date(transactionDetails?.transaction?.billPayment.paymentReceivedDateTime)
            : null,
          fee: transactionDetails?.transaction?.billPayment.fee
            ? new Decimal(transactionDetails?.transaction?.billPayment.fee)
            : null,
          reference1: transactionDetails?.transaction?.billPayment.reference1 ?? null,
          reference2: transactionDetails?.transaction?.billPayment.reference2 ?? null,
          billPaymentTransactionRef: transactionDetails?.transaction?.billPayment.billPaymentTransactionRef ?? null,
          failedReason: transactionDetails?.transaction?.billPayment.failedReason ?? null,
        }: null,
    oddWithdraw: transactionDetails?.transaction?.oddWithdraw
      ? {
          state: transactionDetails?.transaction?.oddWithdraw.state ?? null,
          paymentDisbursedAmount: transactionDetails?.transaction?.oddWithdraw.paymentDisbursedAmount
            ? new Decimal(transactionDetails?.transaction?.oddWithdraw.paymentDisbursedAmount)
            : null,
          paymentDisbursedDateTime: transactionDetails?.transaction?.oddWithdraw.paymentDisbursedDateTime
            ? new Date(transactionDetails?.transaction?.oddWithdraw.paymentDisbursedDateTime)
            : null,
          fee: transactionDetails?.transaction?.oddWithdraw.fee
            ? new Decimal(transactionDetails?.transaction?.oddWithdraw.fee)
            : null,
          failedReason: transactionDetails?.transaction?.oddWithdraw.failedReason ?? null,
          createdAt: transactionDetails?.transaction?.createdAt
            ? new Date(transactionDetails?.transaction?.createdAt)
            : null,
        }
      : null,
    atsWithdraw: transactionDetails?.transaction?.atsWithdraw
      ? {
          state: transactionDetails?.transaction?.atsWithdraw.state ?? null,
          paymentDisbursedAmount: transactionDetails?.transaction?.atsWithdraw.paymentDisbursedAmount
            ? new Decimal(transactionDetails?.transaction?.atsWithdraw.paymentDisbursedAmount)
            : null,
          paymentDisbursedDateTime: transactionDetails?.transaction?.atsWithdraw.paymentDisbursedDateTime
            ? new Date(transactionDetails?.transaction?.atsWithdraw.paymentDisbursedDateTime)
            : null,
          fee: transactionDetails?.transaction?.atsWithdraw.fee
            ? new Decimal(transactionDetails?.transaction?.atsWithdraw.fee)
            : null,
          failedReason: transactionDetails?.transaction?.atsWithdraw.failedReason ?? null,
          createdAt: transactionDetails?.transaction?.createdAt
            ? new Date(transactionDetails?.transaction?.createdAt)
            : null,
        }
      : null,
    upback: transactionDetails?.transaction?.upBack
      ? {
          state: transactionDetails?.transaction?.upBack.state ?? null,
          failedReason: transactionDetails?.transaction?.upBack.failedReason ?? null,
          createdAt: transactionDetails?.transaction?.createdAt
            ? new Date(transactionDetails?.transaction?.createdAt)
            : null,
        }
      : null,
    globalTransfer: transactionDetails?.transaction?.globalTransfer
      ? {
          state: transactionDetails?.transaction?.globalTransfer.state ?? null,
          globalAccount: transactionDetails?.transaction?.globalTransfer.globalAccount ?? null,
          requestedFxCurrency: transactionDetails?.transaction?.globalTransfer.requestedFxCurrency?.toUpperCase() ?? null,
          requestedFxRate: transactionDetails?.transaction?.globalTransfer.requestedFxRate
            ? new Decimal(transactionDetails?.transaction?.globalTransfer.requestedFxRate)
            : null,
          fxConfirmedExchangeRate: transactionDetails?.transaction?.globalTransfer.fxConfirmedExchangeRate
            ? new Decimal(transactionDetails?.transaction?.globalTransfer.fxConfirmedExchangeRate)
            : null,
          fxConfirmedDateTime: transactionDetails?.transaction?.globalTransfer.fxConfirmedDateTime
            ? new Date(transactionDetails?.transaction?.globalTransfer.fxConfirmedDateTime)
            : null,
          transferAmount: transactionDetails?.transaction?.globalTransfer.transferAmount
            ? new Decimal(transactionDetails?.transaction?.globalTransfer.transferAmount)
            : null,
          transferCurrency: transactionDetails?.transaction?.globalTransfer.transferCurrency?.toUpperCase() ?? null,
          transferFee: transactionDetails?.transaction?.globalTransfer.transferFee
            ? new Decimal(transactionDetails?.transaction?.globalTransfer.transferFee)
            : null,
          transferCompleteTime: transactionDetails?.transaction?.globalTransfer.transferCompleteTime
            ? new Date(transactionDetails?.transaction?.globalTransfer.transferCompleteTime)
            : null,
          failedReason: transactionDetails?.transaction?.globalTransfer.failedReason ?? null,
          createdAt: transactionDetails?.transaction?.createdAt
            ? new Date(transactionDetails?.transaction?.createdAt)
            : null,
        }
      : null,
    recovery: transactionDetails?.transaction?.recovery
      ? {
          state: transactionDetails?.transaction?.recovery.state ?? null,
          globalAccount: transactionDetails?.transaction?.recovery.globalAccount ?? null,
          transferAmount: transactionDetails?.transaction?.recovery.transferAmount
            ? new Decimal(transactionDetails?.transaction?.recovery.transferAmount)
            : null,
          transferCurrency: transactionDetails?.transaction?.recovery.transferCurrency?.toUpperCase() ?? null,
          transferFromAccount: transactionDetails?.transaction?.recovery.transferFromAccount ?? null,
          transferToAccount: transactionDetails?.transaction?.recovery.transferToAccount ?? null,
          transferCompleteTime: transactionDetails?.transaction?.recovery.transferCompleteTime
            ? new Date(transactionDetails?.transaction?.recovery.transferCompleteTime)
            : null,
          failedReason: transactionDetails?.transaction?.recovery.failedReason ?? null,
          createdAt: transactionDetails?.transaction?.createdAt
            ? new Date(transactionDetails?.transaction?.createdAt)
            : null,
        }
      : null,
    refundInfo: transactionDetails?.transaction?.refundInfo
      ? {
          id: transactionDetails?.transaction?.refundInfo.refundId ?? null,
          amount: transactionDetails?.transaction?.refundInfo.amount
            ? new Decimal(transactionDetails?.transaction?.refundInfo.amount)
            : null,
          transferToAccountName: transactionDetails?.transaction?.refundInfo.transferToAccountName ?? null,
          transferToAccountNo: transactionDetails?.transaction?.refundInfo.transferToAccountNo ?? null,
          fee: transactionDetails?.transaction?.refundInfo.fee
            ? new Decimal(transactionDetails?.transaction?.refundInfo.fee)
            : null,
          createdAt: transactionDetails?.transaction?.refundInfo.createdAt
            ? new Date(transactionDetails?.transaction?.refundInfo.createdAt)
            : null,
        }
      : null,
    actions: transactionDetails?.transaction?.responseCode?.actions?.map(
      (action: PiBackofficeServiceAPIModelsResponseCodeActionsResponse) => ({
        label: action?.name,
        value: action?.alias,
      })
    ),
    pendingTicketNo,
    makerType,
    checkerType,
  }
}

export const createTicket = async (payload: ICreateTicketRequest): Promise<ICreateTicketResponse> => {
  const instance = await backofficeAxiosInstance()
  const res = await instance.post('tickets', payload)

  return {
    ticket: res?.data?.data,
  }
}

export const verifyTicket = async (ticketNo: string, payload: IVerifyTicketRequest): Promise<IVerifyTicketResponse> => {
  const instance = await backofficeAxiosInstance()
  const res = await instance.post(`tickets/${ticketNo}/check`, payload)

  return {
    ticket: res?.data?.data,
    errorMsg: res?.data?.errorMsg,
    isSuccess: res?.data?.isSuccess,
  }
}

export const getTicketPayload = async (ticketNo: string): Promise<IPayloadResponse> => {
  const instance = await backofficeAxiosInstance()
  const res = await instance.get(`tickets/${ticketNo}/payload`)

  return res.data?.data
}
