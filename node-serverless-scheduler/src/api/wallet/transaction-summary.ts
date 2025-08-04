import { formatDateTimeUTC } from '@libs/date-utils';
import console from 'console';
import got from 'got';

export enum Product {
  Cash,
  CashBalance,
  CreditBalance,
  CreditBalanceSbl,
  Crypto,
  Derivatives,
  GlobalEquities,
  Funds,
}

export enum TransactionType {
  Withdraw = 'Withdraw',
  Deposit = 'Deposit',
}

export type TransactionSummaryResponse = {
  data: {
    transactionSummary: TransactionSummary;
    transactions: Transaction[];
  };
};

export type Transaction = {
  correlationId: string;
  currentState: string;
  transactionNo: string;
  userId: string;
  accountCode: string;
  customerCode: string;
  channel: string;
  product: string;
  purpose: string;
  requestedAmount: string;
  requestedCurrency: string;
  toCurrency: string;
  transferAmount: string;
  fee: string;
  netAmount: string;
  customerName: string;
  bankAccountName: string;
  bankAccountNo: string;
  bankName: string;
  bankCode: string;
  failedReason: string;
  status: string;
  transactionType: string;
  createdAt: Date;
  paymentAt: Date;
  updatedAt: Date;
  depositEntrypoint: BaseEntryPoint;
  withdrawEntrypoint: BaseEntryPoint;
  qrDeposit: QrDepositState;
  oddDeposit: OddDepositState;
  atsDeposit: AtsDepositState;
  oddWithdraw: OddWithdrawState;
  upBack: UpBackState;
  globalTransfer: GlobalTransferState;
  recovery: RecoveryState;
  refundInfo: RefundInfo;
  customerAdditionInfo: CustomerAdditionInfo;
};

export type BaseEntryPoint = {
  correlationId: string;
  currentState: string;
  transactionNo: string;
  userId: string;
  accountCode: string;
  customerCode: string;
  channel: string;
  product: string;
  purpose: string;
  requestedAmount: number;
  confirmedAmount: number;
  netAmount: number;
  customerName: number;
  bankAccountName: number;
  bankAccountTaxId: number;
  bankAccountNo: number;
  bankName: string;
  bankCode: string;
  bankBranchCode: string;
  failedReason: string;
  responseAddress: string;
  requestId: string;
  requesterDeviceId: string;
};

export type QrDepositState = {
  correlationId: string;
  currentState: string;
  transactionNo: string;
  product: string;
  channel: string;
  paymentReceivedAmount: number;
  paymentReceivedDateTime: Date;
  fee: number;
  depositQrGenerateDateTime: Date;
  qrCodeExpiredTimeInMinute: number;
  qrTransactionNo: string;
  qrValue: string;
  qrTransactionRef: string;
  failedReason: string;
  responseAddress: string;
};

export type OddDepositState = {
  correlationId: string;
  currentState: string;
  product: string;
  channel: string;
  paymentReceivedDateTime: Date;
  paymentReceivedAmount: number;
  fee: number;
  otpRequestRef: string;
  otpRequestId: string;
  otpConfirmedDateTime: Date;
  failedReason: string;
  requestId: string;
  responseAddress: string;
};

export type AtsDepositState = {
  correlationId: string;
  currentState: string;
  product: string;
  channel: string;
  paymentReceivedDateTime: Date;
  paymentReceivedAmount: number;
  fee: number;
  otpRequestRef: string;
  otpRequestId: string;
  otpConfirmedDateTime: Date;
  failedReason: string;
  requestId: string;
  responseAddress: string;
};

export type OddWithdrawState = {
  correlationId: string;
  currentState: string;
  product: string;
  channel: string;
  paymentDisbursedDateTime: Date;
  paymentDisbursedAmount: number;
  fee: number;
  otpRequestRef: string;
  otpRequestId: string;
  otpConfirmedDateTime: Date;
  failedReason: string;
  requestId: string;
  responseAddress: string;
};

export type UpBackState = {
  correlationId: string;
  currentState: string;
  product: string;
  channel: string;
  transactionType: string;
  failedReason: string;
};

export type GlobalTransferState = {
  correlationId: string;
  currentState: string;
  product: string;
  channel: string;
  transactionType: string;
  customerId: string;
  globalAccount: string;
  requestedCurrency: string;
  requestedFxRate: number;
  actualFxRate: number;
  fxMarkUpRate: number;
  requestedFxCurrency: string;
  exchangeAmount: number;
  exchangeCurrency: string;
  fxInitiateRequestDateTime: Date;
  fxTransactionId: string;
  fxConfirmedAmount: number;
  fxConfirmedExchangeRate: number;
  fxConfirmedCurrency: string;
  fxConfirmedExchangeAmount: string;
  fxConfirmedExchangeCurrency: string;
  fxConfirmedDateTime: Date;
  transferFromAccount: string;
  transferAmount: number;
  transferFee: number;
  transferToAccount: string;
  transferCurrency: string;
  transferRequestTime: Date;
  transferCompleteTime: Date;
  failedReason: string;
  requestId: string;
  responseAddress: string;
};

export type RecoveryState = {
  correlationId: string;
  currentState: string;
  product: string;
  transactionType: string;
  globalAccount: string;
  transferFromAccount: string;
  transferAmount: number;
  transferToAccount: string;
  transferCurrency: string;
  transferRequestTime: Date;
  transferCompleteTime: Date;
  failedReason: string;
  requestId: string;
  responseAddress: string;
};

export type RefundInfo = {
  id: string;
  amount: number;
  transferToAccountNo: string;
  transferToAccountName: string;
  fee: number;
  createdAt: Date;
};

export type TransactionSummary = {
  successQrDepositCount: number;
  successOddKBankDepositCount: number;
  successOddScbDepositCount: number;
  successOddKtbDepositCount: number;
  successOddBblDepositCount: number;
  successOddBayDepositCount: number;
  successWithdrawCount: number;
  failedDepositCount: number;
  exanteDepositCount: number;
  exanteWithdrawCount: number;
  refundCount: number;
  netAmountDepositCount: number;
  netAmountWithdrawCount: number;
  transactionCount: number;
  successQrDepositAmountThb: number;
  successOddKBankDepositAmountThb: number;
  successOddScbDepositAmountThb: number;
  successOddKtbDepositAmountThb: number;
  successOddBblDepositAmountThb: number;
  successOddBayDepositAmountThb: number;
  successWithdrawAmountThb: number;
  failedDepositAmountThb: number;
  exanteDepositAmountThb: number;
  exanteWithdrawAmountThb: number;
  refundAmountThb: number;
  netAmountDepositAmountThb: number;
  netAmountWithdrawAmountThb: number;
  transactionAmountThb: number;
  successQrDepositAmountUsd: number;
  successOddKBankDepositAmountUsd: number;
  successOddScbDepositAmountUsd: number;
  successOddKtbDepositAmountUsd: number;
  successOddBblDepositAmountUsd: number;
  successOddBayDepositAmountUsd: number;
  successWithdrawAmountUsd: number;
  failedDepositAmountUsd: number;
  exanteDepositAmountUsd: number;
  exanteWithdrawAmountUsd: number;
  refundAmountUsd: number;
  netAmountDepositAmountUsd: number;
  netAmountWithdrawAmountUsd: number;
  transactionAmountUsd: number;
  totalPendingDepositCount: number;
  totalPendingDepositAmountThb: number;
  totalPendingDepositAmountUsd: number;
  totalPendingWithdrawCount: number;
  totalPendingWithdrawAmountThb: number;
  totalPendingWithdrawAmountUsd: number;
};

export type CustomerAdditionInfo = {
  customerTypeId: string;
  idNumber: string;
  nationality: string;
  fullName: string;
};

const timeoutConfig = { request: Number(process.env.API_TIMEOUT || 100000) };

export async function getTransactionSummary(
  walletServiceHost: string,
  product: Product,
  createdAtFrom: Date,
  createdAtTo: Date,
  isGetCustomerAdditionInfo = false
) {
  const url = new URL('internal/transaction/summary', walletServiceHost);
  url.searchParams.append('Product', Product[product].toString());
  url.searchParams.append('CreatedAtFrom', formatDateTimeUTC(createdAtFrom));
  url.searchParams.append('CreatedAtTo', formatDateTimeUTC(createdAtTo));
  url.searchParams.append(
    'IsGetCustomerAdditionInfo',
    isGetCustomerAdditionInfo.toString()
  );

  return got
    .get(url, {
      timeout: timeoutConfig,
      hooks: {
        beforeRequest: [
          (request) => console.info(`HTTP Requesting ${request.url}`),
        ],
      },
    })
    .json<TransactionSummaryResponse>()
    .catch((e) => {
      console.error(`Failed to get failed transactions. Exception: ${e}`);
      throw e;
    });
}
