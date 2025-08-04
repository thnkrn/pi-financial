import { formatDateTimeUTC } from '@libs/date-utils';
import console from 'console';
import got from 'got';

export type TransactionsResponse = {
  data: Transaction[];
  page: number;
  pageSize: number;
  total: number;
  orderBy: string;
  orderDir: string;
};

export type Transaction = {
  state: string;
  product: string;
  accountCode: string;
  customerName: string;
  bankAccountNo: string;
  bankAccountName: string;
  bankName: string;
  effectiveDateTime: Date;
  globalAccount: string;
  transactionNo: string;
  transactionType: string;
  requestedAmount: string;
  requestedCurrency: string;
  status: string;
  createdAt: Date;
  toCurrency: string;
  transferAmount: string;
  channel: string;
  bankAccount: string;
  fee?: string;
  transferFee?: string;
  failedReason?: string;
  externalReference?: string;
  externalChannel?: string;
  purpose?: string;
};

const timeoutConfig = { request: Number(process.env.API_TIMEOUT || 100000) };

export async function getTransactions(
  walletServiceHost: string,
  createdAtFrom: Date,
  createdAtTo: Date,
  pageSize: number
) {
  const url = new URL('internal/transactions', walletServiceHost);
  url.searchParams.append('CreatedAtFrom', formatDateTimeUTC(createdAtFrom));
  url.searchParams.append('CreatedAtTo', formatDateTimeUTC(createdAtTo));
  url.searchParams.append('PageSize', pageSize.toString());

  return got
    .get(url, {
      timeout: timeoutConfig,
      hooks: {
        beforeRequest: [
          (request) => console.info(`HTTP Requesting ${request.url}`),
        ],
      },
    })
    .json<TransactionsResponse>()
    .catch((e) => {
      console.error(`Failed to get transactions. Exception: ${e}`);
      throw e;
    });
}
