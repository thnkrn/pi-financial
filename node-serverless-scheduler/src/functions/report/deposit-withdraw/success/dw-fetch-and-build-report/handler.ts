import { formatJSONResponse } from '@libs/api-gateway';
import {
  convertDataToBuffer,
  generateBlankColumns,
  reportHeaders,
} from '@libs/csv-utils';
import { formatDate } from '@libs/date-utils';
import { middyfy } from '@libs/lambda';
import { storeFileToS3 } from '@libs/s3-utils';
import { getConfigFromSsm } from '@libs/ssm-config';
import {
  DepositTransactions,
  RefundTransactions,
  TransactionData,
  WithdrawTransactions,
  getDepositTransactionsByDateType,
  getRefundTransactions,
  getWithdrawTransactions,
} from '@libs/wallet-api';
import dayjs from 'dayjs';
import isToday from 'dayjs/plugin/isToday';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import { ReportStatus } from '../../../../../constants/report';

dayjs.extend(utc);
dayjs.extend(timezone);
dayjs.extend(isToday);

const transactionDataHeaders = [
  'No',
  'Transaction Date',
  'Transaction Time',
  'Bank Account Number',
  'Bank Code',
  'Channel',
  'Customer Account Code',
  'Product',
  'Transaction Type',
  'Transaction Number',
  'Currency',
  'Amount',
];

async function getConfig() {
  const [walletServiceHost] = await getConfigFromSsm('report', [
    'wallet-srv-host',
  ]);

  return {
    walletServiceHost,
  };
}

interface RequestData {
  id: string;
  dateFrom: string;
  dateTo: string;
}

const amountSummary = ({
  depositTransactions,
  withdrawTransactions,
  refundTransactions,
}: {
  depositTransactions: DepositTransactions[];
  withdrawTransactions: WithdrawTransactions[];
  refundTransactions: RefundTransactions[];
}) => [
  [
    ...generateBlankColumns(),
    'Total Deposit Amount',
    totalAmountPerTransactionType(depositTransactions, 'Deposit'),
  ],
  [
    ...generateBlankColumns(),
    'Total CASHBALANCE Deposit Amount',
    totalAmountPerTransactionType(
      depositTransactions,
      'Deposit',
      'CashBalance'
    ),
  ],
  [
    ...generateBlankColumns(),
    'Total DERIVATIVES Deposit Amount',
    totalAmountPerTransactionType(
      depositTransactions,
      'Deposit',
      'Derivatives'
    ),
  ],
  [
    ...generateBlankColumns(),
    'Total CASH Deposit Amount',
    totalAmountPerTransactionType(depositTransactions, 'Deposit', 'Cash'),
  ],
  [
    ...generateBlankColumns(),
    'Total CreditBalanceSbl Amount',
    totalAmountPerTransactionType(
      depositTransactions,
      'Deposit',
      'CreditBalanceSbl'
    ),
  ],
  [
    ...generateBlankColumns(),
    'Total GlobalEquity Deposit Amount',
    totalAmountPerTransactionType(
      depositTransactions,
      'Deposit',
      'GlobalEquities'
    ),
  ],
  [],
  [
    ...generateBlankColumns(),
    'Total Withdraw Amount',
    totalAmountPerTransactionType(withdrawTransactions, 'Withdraw'),
  ],
  [
    ...generateBlankColumns(),
    'Total CASHBALANCE Withdraw Amount',
    totalAmountPerTransactionType(
      withdrawTransactions,
      'Withdraw',
      'CashBalance'
    ),
  ],
  [
    ...generateBlankColumns(),
    'Total DERIVATIVES Withdraw Amount',
    totalAmountPerTransactionType(
      withdrawTransactions,
      'Withdraw',
      'Derivatives'
    ),
  ],
  [
    ...generateBlankColumns(),
    'Total CASH Withdraw Amount',
    totalAmountPerTransactionType(withdrawTransactions, 'Withdraw', 'Cash'),
  ],
  [
    ...generateBlankColumns(),
    'Total CreditBalanceSbl Withdraw Amount',
    totalAmountPerTransactionType(
      withdrawTransactions,
      'Withdraw',
      'CreditBalanceSbl'
    ),
  ],
  [
    ...generateBlankColumns(),
    'Total GlobalEquity Withdraw Amount',
    totalAmountPerTransactionType(
      withdrawTransactions,
      'Withdraw',
      'GlobalEquities'
    ),
  ],
  [],
  [
    ...generateBlankColumns(),
    'Total Refund Amount',
    totalAmountPerTransactionType(refundTransactions, 'Refund'),
  ],
  [
    ...generateBlankColumns(),
    'Total CASHBALANCE Refund Amount',
    totalAmountPerTransactionType(refundTransactions, 'Refund', 'CashBalance'),
  ],
  [
    ...generateBlankColumns(),
    'Total DERIVATIVES Refund Amount',
    totalAmountPerTransactionType(refundTransactions, 'Refund', 'Derivatives'),
  ],
  [
    ...generateBlankColumns(),
    'Total CASH Refund Amount',
    totalAmountPerTransactionType(refundTransactions, 'Refund', 'Cash'),
  ],
  [
    ...generateBlankColumns(),
    'Total CreditBalanceSbl Refund Amount',
    totalAmountPerTransactionType(
      refundTransactions,
      'Refund',
      'CreditBalanceSbl'
    ),
  ],
  [
    ...generateBlankColumns(),
    'Total GlobalEquity Refund Amount',
    totalAmountPerTransactionType(
      refundTransactions,
      'Refund',
      'GlobalEquities'
    ),
  ],
];

const fetchReportDataAndUploadToS3 = async (event) => {
  const request = JSON.parse(event.body) as RequestData;
  try {
    const config = await getConfig();
    const dateFrom = new Date(request.dateFrom);
    const dateTo = new Date(request.dateTo);

    /** Convert TimeZone */
    // If dateFrom is 2023-11-07 to 2023-11-08 BKK TimeZone (UTC+ 7)
    // It queries the API from 2023-11-06T17:00:00 UTC To 2023-11-08T17:00:00 UTC

    // If dateFrom is 2023-11-09 to 2023-11-09 BKK TimeZone (UTC+ 7)
    // It queries the API from 2023-11-08T17:00:00 UTC To 2023-11-09T17:00:00 UTC
    const filterDateFrom = new Date(dateFrom.getTime() - 7 * 60 * 60 * 1000);
    const filterDateTo = new Date(dateTo.getTime() + 17 * 60 * 60 * 1000);

    const depositTransactions = await getDepositTransactionsByDateType(
      'PaymentReceived',
      config.walletServiceHost,
      filterDateFrom,
      filterDateTo
    );

    const withdrawTransactions = await getWithdrawTransactions(
      config.walletServiceHost,
      filterDateFrom,
      filterDateTo
    );

    const refundTransactions = await getRefundTransactions(
      config.walletServiceHost,
      filterDateFrom,
      filterDateTo
    );

    const index = 1;
    const depositTransactionsLength = depositTransactions.length;
    const withdrawTransactionsLength = withdrawTransactions.length;

    const transactionsCsvBuffer = await convertDataToBuffer([
      ...reportHeaders(
        request.id,
        'Deposit/Withdraw Reconcile Report',
        'Daily Reconcile by the Total Amount Break Down by Account Type'
      ),
      [],
      transactionDataHeaders,
      ..._formatDepositData(
        index,
        depositTransactions.sort((r1, r2) =>
          r1.paymentReceivedDateTime > r2.paymentReceivedDateTime ? 1 : -1
        )
      ),
      ..._formatWithdrawData(
        depositTransactionsLength + 1,
        orderTransactions(withdrawTransactions)
      ),
      ..._formatRefundData(
        depositTransactionsLength + withdrawTransactionsLength + 1,
        orderTransactions(refundTransactions)
      ),
      [],
      ...amountSummary({
        depositTransactions,
        withdrawTransactions,
        refundTransactions,
      }),
    ]);

    const s3fileKey = getS3KeyForReport(
      'dw-success-reconcile/dw_success_all',
      dateFrom,
      dateTo
    );

    await storeFileToS3(
      `backoffice-reports-${process.env.ENVIRONMENT}`,
      transactionsCsvBuffer,
      s3fileKey
    );

    return formatJSONResponse({
      id: request.id,
      status: ReportStatus.Done,
      fileName: s3fileKey,
    });
  } catch (e) {
    console.error('Failed to build or upload report \n', +JSON.stringify(e));
    return formatJSONResponse({
      id: request.id,
      status: ReportStatus.Failed,
      fileName: '',
    });
  }
};

function _formatDepositData(
  index: number,
  transactions: DepositTransactions[]
) {
  return transactions.map((t) => [
    index++,
    formatDate(t.paymentReceivedDateTime).split(' ')[0],
    formatDate(t.paymentReceivedDateTime).split(' ')[1],
    t.bankAccountNo,
    t.bankCode,
    t.channel,
    t.accountCode,
    t.product,
    t.transactionType,
    t.transactionNo,
    t.currency,
    Math.abs(t.amount),
  ]);
}

function _formatWithdrawData(
  index: number,
  transactions: WithdrawTransactions[]
) {
  return transactions.map((t) => [
    index++,
    formatDate(t.effectiveDateTime).split(' ')[0],
    formatDate(t.effectiveDateTime).split(' ')[1],
    t.bankAccountNo,
    t.bankCode,
    t.channel,
    t.accountCode,
    t.product,
    t.transactionType,
    t.transactionNo,
    t.currency,
    Math.abs(t.amount),
  ]);
}

function _formatRefundData(index: number, transactions: RefundTransactions[]) {
  return transactions.map((t) => [
    index++,
    formatDate(t.effectiveDateTime).split(' ')[0],
    formatDate(t.effectiveDateTime).split(' ')[1],
    t.bankAccountNo,
    t.bankCode,
    t.channel,
    t.accountCode,
    t.product,
    t.transactionType,
    t.depositTransactionNo,
    t.currency,
    Math.abs(t.amount),
  ]);
}

const totalAmountPerTransactionType = (
  transactions: TransactionData[],
  type: string,
  subType?: string
): number => {
  return transactions
    .filter(
      (transaction) =>
        transaction.transactionType === type &&
        (!subType || transaction.product === subType)
    )
    .reduce((total, transaction) => total + transaction.amount, 0);
};

type WithdrawRefundTransactions = WithdrawTransactions | RefundTransactions;

const orderTransactions = <T extends WithdrawRefundTransactions>(
  transactions: Array<T>
) => {
  return transactions.sort((r1, r2) =>
    r1.effectiveDateTime > r2.effectiveDateTime ? 1 : -1
  );
};

export const getS3KeyForReport = (
  prefix: string,
  dateFrom: Date,
  dateTo: Date
): string => {
  const currentTime = new Date();
  let formatDateTo: string;
  const formatDateFrom = dayjs(dateFrom).utc().format('YYYY-MM-DDTHH:mm:ss');

  // Check if it is today
  if (dayjs(dateTo).isToday()) {
    formatDateTo = dayjs(
      dateTo.setHours(
        currentTime.getHours(),
        currentTime.getMinutes(),
        currentTime.getSeconds()
      )
    )
      .tz('Asia/Bangkok')
      .format('YYYY-MM-DDTHH:mm:ss');
  } else {
    formatDateTo = dayjs(dateTo.setHours(23, 59, 59)).format(
      'YYYY-MM-DDTHH:mm:ss'
    );
  }

  return `${prefix}_${formatDateFrom}--${formatDateTo}.csv`;
};

export const main = middyfy(fetchReportDataAndUploadToS3);
