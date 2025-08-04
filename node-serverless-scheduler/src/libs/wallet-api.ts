import got from 'got';

export interface TransactionData {
  bankAccountNo: string;
  bankCode: string;
  channel: string;
  accountCode: string;
  customerCode: string;
  product: string;
  transactionNo: string;
  transactionType: string;
  currency: string;
  amount: number;
  createdAt: Date;
}

interface TransactionWithEffectiveDate extends TransactionData {
  effectiveDateTime: string;
}

export interface DepositTransactions extends TransactionWithEffectiveDate {
  paymentReceivedDateTime: string;
  bankAccountName: string;
  currentState: string;
  qrTransactionNo: string;
}

export type WithdrawTransactions = TransactionWithEffectiveDate;

export interface RefundTransactions extends TransactionWithEffectiveDate {
  depositTransactionNo: string;
}

export interface TransactionResponse<T> {
  data: T[];
  page: number;
  pageSize: number;
  total: number;
  orderBy: string;
  orderDirection: string;
}

export type WithdrawTransactionResponse =
  TransactionResponse<WithdrawTransactions>;
export type RefundTransactionResponse = TransactionResponse<RefundTransactions>;
export type DepositTransactionResponse =
  TransactionResponse<DepositTransactions>;

const timeoutConfig = { request: Number(process.env.API_TIMEOUT || 100000) };

async function fetchDepositTransactionByDateType(
  dateType: 'EffectiveDate' | 'PaymentReceived',
  walletServiceHost: string,
  dateFrom: Date,
  dateTo: Date,
  page: number,
  pageSize: number,
  status?: string,
  productType?: string
) {
  const url = new URL(`internal/transactions/deposit`, walletServiceHost);

  url.searchParams.append(`${dateType}From`, dateFrom.toISOString());
  url.searchParams.append(`${dateType}To`, dateTo.toISOString());
  url.searchParams.append('Page', page.toString());
  url.searchParams.append('PageSize', pageSize.toString());

  if (status) {
    url.searchParams.append('Status', status);
  }

  if (productType) {
    url.searchParams.append('ProductType', productType);
  }

  console.log('url', url);
  return got
    .get(url, { timeout: timeoutConfig })
    .json<DepositTransactionResponse>()
    .catch((e) => {
      console.error(`Failed to get deposit transactions. Exception: ${e}`);
      throw e;
    });
}

async function fetchRefundTransaction(
  walletServiceHost: string,
  effectiveDateFrom: Date,
  effectiveDateTo: Date,
  page: number,
  pageSize: number,
  status?: string
) {
  const url = new URL(`internal/transactions/refund`, walletServiceHost);

  url.searchParams.append('EffectiveDateFrom', effectiveDateFrom.toISOString());
  url.searchParams.append('EffectiveDateTo', effectiveDateTo.toISOString());
  url.searchParams.append('Page', page.toString());
  url.searchParams.append('PageSize', pageSize.toString());

  if (status) {
    url.searchParams.append('Status', status);
  }

  console.log('url', url);
  return got
    .get(url, { timeout: timeoutConfig })
    .json<RefundTransactionResponse>()
    .catch((e) => {
      console.error(`Failed to get refund transactions. Exception: ${e}`);
      throw e;
    });
}

export async function getDepositTransactionsByDateType(
  dateType: 'EffectiveDate' | 'PaymentReceived',
  walletServiceHost: string,
  dateFrom: Date,
  dateTo: Date,
  status?: string,
  productType?: string
) {
  let allDepositTransactions: DepositTransactions[] = [];
  let page = 1;
  const pageSize = 1000;

  const depositTransactions = await fetchDepositTransactionByDateType(
    dateType,
    walletServiceHost,
    dateFrom,
    dateTo,
    page,
    pageSize,
    status,
    productType
  );

  allDepositTransactions = allDepositTransactions.concat(
    depositTransactions.data
  );

  while (page < Math.ceil(depositTransactions.total / pageSize)) {
    const depositTransactions = await fetchDepositTransactionByDateType(
      dateType,
      walletServiceHost,
      dateFrom,
      dateTo,
      page,
      pageSize,
      status,
      productType
    );

    allDepositTransactions = allDepositTransactions.concat(
      depositTransactions.data
    );
    page++;
  }

  return allDepositTransactions;
}

async function fetchWithdrawTransaction(
  walletServiceHost: string,
  effectiveDateFrom: Date,
  effectiveDateTo: Date,
  page: number,
  pageSize: number,
  status?: string
) {
  const url = new URL(`internal/transactions/withdraw`, walletServiceHost);

  url.searchParams.append('EffectiveDateFrom', effectiveDateFrom.toISOString());
  url.searchParams.append('EffectiveDateTo', effectiveDateTo.toISOString());
  url.searchParams.append('Page', page.toString());
  url.searchParams.append('PageSize', pageSize.toString());

  if (status) {
    url.searchParams.append('Status', status);
  }

  console.log('url', url);
  return got
    .get(url, { timeout: timeoutConfig })
    .json<WithdrawTransactionResponse>()
    .catch((e) => {
      console.error(`Failed to get withdraw transactions. Exception: ${e}`);
      throw e;
    });
}

export async function getWithdrawTransactions(
  walletServiceHost: string,
  effectiveDateFrom: Date,
  effectiveDateTo: Date,
  status?: string
) {
  let allWithdrawTransactions: WithdrawTransactions[] = [];
  let page = 1;
  const pageSize = 1000;

  const withdrawTransactions = await fetchWithdrawTransaction(
    walletServiceHost,
    effectiveDateFrom,
    effectiveDateTo,
    page,
    pageSize,
    status
  );

  allWithdrawTransactions = allWithdrawTransactions.concat(
    withdrawTransactions.data
  );

  while (page < Math.ceil(withdrawTransactions.total / pageSize)) {
    const withdrawTransactions = await fetchWithdrawTransaction(
      walletServiceHost,
      effectiveDateFrom,
      effectiveDateTo,
      page,
      pageSize,
      status
    );

    allWithdrawTransactions = allWithdrawTransactions.concat(
      withdrawTransactions.data
    );
    page++;
  }

  return allWithdrawTransactions;
}

export async function getRefundTransactions(
  walletServiceHost: string,
  effectiveDateFrom: Date,
  effectiveDateTo: Date,
  status?: string
) {
  let allRefundTransactions: RefundTransactions[] = [];
  let page = 1;
  const pageSize = 1000;

  const refundTransactions = await fetchRefundTransaction(
    walletServiceHost,
    effectiveDateFrom,
    effectiveDateTo,
    page,
    pageSize,
    status
  );

  allRefundTransactions = allRefundTransactions.concat(refundTransactions.data);

  while (page < Math.ceil(refundTransactions.total / pageSize)) {
    const refundTransactions = await fetchRefundTransaction(
      walletServiceHost,
      effectiveDateFrom,
      effectiveDateTo,
      page,
      pageSize,
      status
    );

    allRefundTransactions = allRefundTransactions.concat(
      refundTransactions.data
    );
    page++;
  }

  return allRefundTransactions;
}
