import got from 'got';
import { v4 as guid } from 'uuid';
import { getSnsParam, sendMessageToSNS } from '@libs/sns-client';

export type DepositRetryRequest = {
  transactionNo: string;
};

export type TransactionHistory = {
  transactionNo: string;
  transactionType: string;
  requestedAmount: number;
  requestedCurrency: string;
  status: string;
  createdAt: Date;
  toCurrency: string;
  transferAmount: number;
  channel: string;
};

export type GetFailedTransactionResponse = {
  data: TransactionHistory[];
  page: number;
  pageSize: number;
  total: number;
  orderBy: string;
  orderDirection: string;
};

const timeoutConfig = { request: 5000 };

export async function getFailedTransactions(
  walletServiceHost: string,
  state: string
) {
  const url = new URL('internal/transaction/GlobalEquities', walletServiceHost);
  url.searchParams.append('State', state);
  url.searchParams.append('Page', '1');
  url.searchParams.append('PageSize', '100000');

  return got
    .get(url, { timeout: timeoutConfig })
    .json<GetFailedTransactionResponse>()
    .catch((e) => {
      console.error(`Failed to get failed transactions. Exception: ${e}`);
      throw e;
    });
}

////////////////////////////////////////////////////////////////////////////////

const depositRetryParams = getSnsParam(
  'Pi.WalletService.Domain.Events.Deposit',
  'DepositRetryFxTransfer'
);

export async function sendDepositRetry(
  payload: DepositRetryRequest
): Promise<void> {
  const messageId = guid();
  await sendMessageToSNS(depositRetryParams.topicName, {
    messageId,
    messageType: [depositRetryParams.urn],
    message: { ...payload, ticketId: messageId },
  });
}
