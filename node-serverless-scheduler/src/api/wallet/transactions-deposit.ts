import { formatDateTimeUTC } from '@libs/date-utils';
import console from 'console';
import got from 'got';

export type DepositTransactionsResponse = {
  data: DepositTransaction[];
  page: number;
  pageSize: number;
  total: number;
};

export type DepositTransaction = {
  purpose: string;
  requestedAmount: number;
  customerName: string;
  qrGenerateDateTime: Date;
  qrExpiredTime: Date;
  qrCodeExpiredTimeInMinute: number;
  qrTransactionNo: string;
  qrValue: string;
  qrTransactionRef: string;
  id: string;
  userId: string;
  transactionNo: string;
  accountCode: string;
  customerCode: string;
  amount: number;
  product: string;
  transactionType: string;
  currentState: string;
  status: string;
  currency: string;
  channel: string;
  createdAt: Date;
};

const timeoutConfig = { request: Number(process.env.API_TIMEOUT || 100000) };

// Passing parameters required for globalPaymentNotReceived
export async function getDepositTransactions(
  walletServiceHost: string,
  state: string,
  productType: string,
  createdAtFrom: Date,
  createdAtTo: Date
) {
  const url = new URL('internal/transactions/deposit', walletServiceHost);
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
    .json<DepositTransactionsResponse>()
    .catch((e) => {
      console.error(`Failed to get transactions. Exception: ${e}`);
      throw e;
    });
}
