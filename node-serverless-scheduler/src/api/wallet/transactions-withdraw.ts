import { formatDateTimeUTC } from '@libs/date-utils';
import console from 'console';
import got from 'got';

export type WithdrawTransactionsResponse = {
  data: WithdrawTransaction[];
  page: number;
  pageSize: number;
  total: number;
};

export type WithdrawTransaction = {
  otpRequestRef: string;
  otpRequestId: string;
  id: string;
  userId: string;
  transactionNo: string;
  accountCode: string;
  customerCode: string;
  product: string;
  transactionType: string;
  currentState: string;
  bankName: string;
  status: string;
  bankCode: string;
  bankAccountNo: string;
  currency: string;
  channel: string;
  createdAt: Date;
};

const timeoutConfig = { request: Number(process.env.API_TIMEOUT || 100000) };

// Passing parameters required for globalPaymentNotReceived
export async function getWithdrawTransactions(
  walletServiceHost: string,
  state: string,
  productType: string,
  createdAtFrom: Date,
  createdAtTo: Date
) {
  const url = new URL('internal/transactions/withdraw', walletServiceHost);
  url.searchParams.append('State', state);
  url.searchParams.append('ProductType', productType);
  url.searchParams.append('CreatedAtFrom', formatDateTimeUTC(createdAtFrom));
  url.searchParams.append('CreatedAtTo', formatDateTimeUTC(createdAtTo));

  return got
    .get(url, {
      timeout: timeoutConfig,
      hooks: {
        beforeRequest: [
          (request) => console.info(`HTTP Requesting ${request.url}`),
        ],
      },
    })
    .json<WithdrawTransactionsResponse>()
    .catch((e) => {
      console.error(`Failed to get transactions. Exception: ${e}`);
      throw e;
    });
}
