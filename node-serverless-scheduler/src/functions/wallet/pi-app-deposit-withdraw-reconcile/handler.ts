import {
  getTransferHistory,
  TransferHistory,
  TransferHistoryRequest,
  TransferType,
  TransferVia,
} from '@api/payment/transfer-history';
import { Product } from '@api/wallet/transaction-summary';
import { getTransactions, Transaction } from '@api/wallet/transactions-v2';
import schema from '@functions/fund/fund-account-opening-state-report/schema';
import {
  formatJSONResponse,
  ValidatedEventAPIGatewayProxyEvent,
} from '@libs/api-gateway';
import { convertDataToBuffer } from '@libs/csv-utils';
import { toTHDateTime } from '@libs/date-utils';
import { convertFilesToAttachments, sendEmailToSES } from '@libs/email-utils';
import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';
import { APIGatewayProxyResult } from 'aws-lambda';
import * as console from 'console';
import * as process from 'process';

// reconcile non-global constant
const reconcileTransactionColumnHeaders = [
  'Account Code',
  'Transaction Id',
  'Transaction Type',
  'Channel',
  'Product',
  'Bank Name',
  'Bank Account Name',
  'Bank Account No',
  'Amount THB',
  'Transaction Created At',
  'Refund Amount THB',
  'Net Amount THB',
  'Status',
  'Reason',
];

const reconcileTransactionTotalColumnHeaders = ['', 'Amount in THB'];

// deposit odd constant
const depositOddHeaders = [
  'Transaction Id',
  'Transaction Type',
  'Product',
  'Bank Name',
  'Bank Account Name',
  'Bank Account No.',
  'Amount THB',
  'Transaction Created Date & Time',
  'Reference no.',
];

const totalSummaries = {
  totalDeposit: { display: 'Total Deposit Amount', product: null },
  totalCashBalanceDeposit: {
    display: 'Total CashBalance Deposit Amount',
    product: Product.CashBalance,
  },
  totalDERIVATIVESDeposit: {
    display: 'Total DERIVATIVES Deposit Amount',
    product: Product.Derivatives,
  },
  totalCASHDeposit: {
    display: 'Total CASH Deposit Amount',
    product: Product.Cash,
  },
  totalCreditBalanceDeposit: {
    display: 'Total CreditBalance Amount',
    product: Product.CreditBalance,
  },
  totalCreditBalanceSblDeposit: {
    display: 'Total CreditBalanceSbl Amount',
    product: Product.CreditBalanceSbl,
  },
  totalGlobalEquitiesDeposit: {
    display: 'Total Global Equities Deposit Amount',
    product: Product.GlobalEquities,
  },
};

const oddBank = {
  '002': 'BBL',
  '004': 'KBANK',
  '006': 'KTB',
  '014': 'SCB',
  '025': 'BAY',
};

interface Config {
  walletServiceHost: string;
  paymentServiceHost: string;
  reconcileRecipient: string;
}

async function getConfig(): Promise<Config> {
  const [walletServiceHost, paymentServiceHost, reconcileRecipient] =
    await getConfigFromSsm('wallet', [
      'wallet-srv-host',
      'payment-srv-host',
      'pi-app-dw-report-recipient',
    ]);

  return {
    walletServiceHost,
    paymentServiceHost,
    reconcileRecipient,
  };
}

const _run: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  const config = await getConfig();
  const date = new Date(event.body.requestDate);
  const subject = getEmailTitle(date);

  console.info(`Calling _getReports date: ${date} config: ${config}`);
  const reports = await _getReports(date, config);

  console.info(`Calling _sendReports`);
  await _sendReports(
    reports.fileNames,
    subject,
    reports.csvBufferList,
    config.reconcileRecipient
  );
  console.info(`Done`);

  return formatJSONResponse({
    status: 'done',
  });
};

const sendPiAppDepositWithdrawReport = async () => {
  const config = await getConfig();
  const currentDate = new Date();

  const subject = getEmailTitle(currentDate);

  console.info(`Calling _getReports date: ${currentDate} config: ${config}`);
  const report = await _getReports(currentDate, config);

  console.info(`Calling _sendReports`);
  await _sendReports(
    report.fileNames,
    subject,
    report.csvBufferList,
    config.reconcileRecipient
  );

  console.info(`Done`);
};

// Function to fetch data from DB
async function _getReports(date: Date, config: Config) {
  try {
    const [createdAtFrom, createdAtTo] = getDateTimeFromAndDateTimeTo(date);
    const piAppData = await getPiAppDepositWithdrawReportData(
      config,
      createdAtFrom,
      createdAtTo
    );
    const depositOddData = await getDepositOddReportData(
      config,
      createdAtFrom,
      createdAtTo
    );

    const fileNames = [...piAppData.fileNames, ...depositOddData.fileNames];
    const csvBufferList = [
      ...piAppData.csvBufferList,
      ...depositOddData.csvBufferList,
    ];

    return { fileNames, csvBufferList };
  } catch (e) {
    console.error('Failed get reports\n' + JSON.stringify(e));
    throw e;
  }
}

async function getPiAppDepositWithdrawReportData(
  config: Config,
  createdAtFrom: Date,
  createdAtTo: Date
) {
  const dateStr = createdAtTo.toISOString().slice(0, 10);

  const resp = await getTransactions(
    config.walletServiceHost,
    createdAtFrom,
    createdAtTo,
    100000 // force to get all transactions
  );

  const nonGlobalTransactions = getNonGlobalTransactions(resp.data);

  console.info(`[_getReports] Formatting transaction`);
  const reconcileTxns = _reformatTransactions(nonGlobalTransactions);
  const reconcileTxnTotal = _reformatTransactionSummary(nonGlobalTransactions);

  console.info(`[_getReports] Combining transaction data`);
  const combinedData = _combineData(reconcileTxns, reconcileTxnTotal);
  const fileNames = ['PI_APP_DW_Daily_Report_' + dateStr + '.csv'];

  console.info(`[_getReports] Converting data to buffer`);
  const csvBufferList = [await convertDataToBuffer(combinedData)];

  return { fileNames, csvBufferList };
}

async function getDepositOddReportData(
  config: Config,
  createdAtFrom: Date,
  createdAtTo: Date
) {
  const request: TransferHistoryRequest = {
    createdAtFrom,
    createdAtTo,
    transferType: TransferType.Debit,
    transferVia: TransferVia.Finnet,
    success: true,
  };

  const transactionHistory = await getTransferHistory(
    config.paymentServiceHost,
    request
  );

  const transactionsByBank: { [key: string]: TransferHistory[] } = {};
  for (const bankCode of Object.keys(oddBank)) {
    const bankName = oddBank[bankCode];
    const transactions = transactionHistory.data
      .filter((t) => t.customerBankCode == bankCode)
      .sort(
        (a, b) =>
          toTHDateTime(a.completedAt).getTime() -
          toTHDateTime(b.completedAt).getTime()
      );
    transactionsByBank[bankName] = [...transactions];
  }

  // Create csv report for each bank
  const reports = {};
  for (const [bankName, transactions] of Object.entries(transactionsByBank)) {
    // sort transactions by completedAt

    // Add headers to the csv data
    const csvData = [];
    csvData.push(depositOddHeaders);
    // Calculate the sum of the `Amount THB` field
    transactions.forEach((transaction) => {
      const amount = Number(transaction.transferAmount);
      csvData.push([
        transaction.transactionNo,
        'Deposit',
        transaction.product,
        bankName,
        transaction.customerBankAccountName,
        transaction.customerBankAccountNo,
        amount.toFixed(2),
        transaction.completedAt,
        transaction.bankReferenceNo,
      ]);
    });

    csvData.push([]);
    for (const key of Object.keys(totalSummaries)) {
      const total = totalSummaries[key];
      csvData.push(
        generateTotalRow(
          total.display,
          getTotalAmountByProduct(transactions, total.product)
        )
      );
    }

    // Convert the CSV data into CSV format
    reports[bankName] = await convertDataToBuffer(csvData);
  }

  const fileNames = [];
  const csvBufferList = [];

  for (const [bankName, report] of Object.entries(reports)) {
    const fileName = `Deposit_ODD_Reconcile_report_${bankName}_${createdAtTo
      .toISOString()
      .slice(0, 10)}.csv`;
    fileNames.push(fileName);
    csvBufferList.push(report);
  }

  return { fileNames, csvBufferList };
}

function getTotalAmountByProduct(
  transferHistories: TransferHistory[],
  product: string = null
) {
  let transferByProduct = transferHistories;
  if (product !== null && product !== undefined) {
    transferByProduct = transferByProduct.filter(
      (t) =>
        t.product?.toLowerCase() == Product[product]?.toString().toLowerCase()
    );
  }
  return transferByProduct.reduce(
    (total, transaction) => total + Number(transaction.transferAmount),
    0
  );
}

function generateTotalRow(displayText: string, amount: number) {
  return [displayText, amount.toFixed(2)];
}

async function _sendReports(
  fileNames: string[],
  subject: string,
  fileContents: Buffer[],
  recipient: string
) {
  try {
    console.info('[_sendReports] convert files to attachments');
    const attachments = convertFilesToAttachments(fileNames, fileContents);
    console.info('[_sendReports] calling sendEmailToSES');
    await sendEmailToSES(recipient, subject, attachments);
  } catch (e) {
    console.error('Failed to send reconcile reports\n' + JSON.stringify(e));
    throw e;
  }
}

function getDateTimeFromAndDateTimeTo(date: Date): Date[] {
  // Set createdAtFrom and createdAtTo to query from 00:00 to 00:00 (+1 Day) BKK TIME
  const createdAtFrom = new Date(date);
  const createdAtTo = new Date(date);
  createdAtFrom.setDate(createdAtFrom.getDate() - 1);
  createdAtFrom.setHours(17, 0, 0, 0);
  createdAtTo.setHours(17, 0, 0, 0);

  return [createdAtFrom, createdAtTo];
}

function getDateTimeFromAndDateTimeToDownloadEndpoint(
  dateFrom: Date,
  dateTo: Date
): Date[] {
  // Set dateFrom to 00:00 and dateTo to 23.59 BKK TIME
  const createdAtFrom = new Date(dateFrom);
  const createdAtTo = new Date(dateTo);

  createdAtFrom.setDate(createdAtFrom.getDate() - 1);
  createdAtFrom.setHours(17, 0, 0, 0);
  createdAtTo.setHours(16, 59, 59, 999);

  return [createdAtFrom, createdAtTo];
}

// Format transaction data
function _reformatTransactions(data: Transaction[]) {
  // sort data by createdAt
  const sortedData = data.sort(
    (a, b) =>
      toTHDateTime(a.createdAt).getTime() - toTHDateTime(b.createdAt).getTime()
  );
  return sortedData.map((obj) => [
    obj.accountCode,
    obj.transactionNo,
    obj.transactionType,
    obj.channel,
    obj.product,
    obj.bankName,
    obj.bankAccountName,
    obj.bankAccountNo,
    obj.requestedAmount,
    toTHDateTime(obj.createdAt).toISOString(),
    obj.state.toLowerCase() !== 'refundsuccess'
      ? '0.00'
      : (Number(obj.requestedAmount) - Number(obj.transferAmount)).toFixed(2),
    obj.state.toLowerCase() !== 'refundsuccess'
      ? Number(obj.transferAmount).toFixed(2)
      : '0.00',
    obj.status,
    obj.failedReason ?? '',
  ]);
}

// Format transaction summary data
function _reformatTransactionSummary(data: Transaction[]) {
  // Sort transaction summary data
  const totalDepositAmount = getTotalDepositAmount(data);
  const cashBalanceTotalDepositAmount = getCashBalanceTotalDepositAmount(data);
  const derivativesTotalDepositAmount = getDerivativesTotalDepositAmount(data);
  const cashTotalDepositAmount = getCashTotalDepositAmount(data);
  const creditBalanceTotalDepositAmount =
    getCreditBalanceTotalDepositAmount(data);
  const creditBalanceSblTotalDepositAmount =
    getCreditBalanceSblTotalDepositAmount(data);
  const totalDepositAmountQR = getTotalDepositAmountQR(data);
  const cashBalanceTotalDepositAmountQR =
    getCashBalanceTotalDepositAmountQR(data);
  const derivativesTotalDepositAmountQR =
    getDerivativesTotalDepositAmountQR(data);
  const cashTotalDepositAmountQR = getCashTotalDepositAmountQR(data);
  const creditBalanceTotalDepositAmountQR =
    getCreditBalanceTotalDepositAmountQR(data);
  const creditBalanceSblTotalDepositAmountQR =
    getCreditBalanceSblTotalDepositAmountQR(data);
  const totalDepositAmountODD = getTotalDepositAmountODD(data);
  const cashBalanceTotalDepositAmountODD =
    getCashBalanceTotalDepositAmountODD(data);
  const derivativesTotalDepositAmountODD =
    getDerivativesTotalDepositAmountODD(data);
  const cashTotalDepositAmountODD = getCashTotalDepositAmountODD(data);
  const creditBalanceTotalDepositAmountODD =
    getCreditBalanceTotalDepositAmountODD(data);
  const creditBalanceSblTotalDepositAmountODD =
    getCreditBalanceSblTotalDepositAmountODD(data);
  const totalWithdrawAmount = getTotalWithdrawAmount(data);
  const kkpTotalWithdrawAmount = getKkpTotalWithdrawAmount(data);
  const atsTotalWithdrawAmount = getAtsTotalWithdrawAmount(data);
  const cashBalanceTotalWithdrawAmount =
    getCashBalanceTotalWithdrawAmount(data);
  const derivativesTotalWithdrawAmount =
    getDerivativesTotalWithdrawAmount(data);
  const cashTotalWithdrawAmount = getCashTotalWithdrawAmount(data);
  const creditBalanceTotalWithdrawAmount =
    getCreditBalanceTotalWithdrawAmount(data);
  const creditBalanceSblTotalWithdrawAmount =
    getCreditBalanceSblTotalWithdrawAmount(data);
  const totalRefundAmount = getTotalRefundAmount(data);
  const cashBalanceTotalRefundAmount = getCashBalanceTotalRefundAmount(data);
  const derivativesTotalRefundAmount = getDerivativesTotalRefundAmount(data);
  const cashTotalRefundAmount = getCashTotalRefundAmount(data);
  const creditBalanceTotalRefundAmount =
    getCreditBalanceTotalRefundAmount(data);
  const creditBalanceSblTotalRefundAmount =
    getCreditBalanceSblTotalRefundAmount(data);
  const totalPendingDepositAmount = getPendingDepositAmount(data);
  const totalPendingWithdrawAmount = getPendingWithdrawAmount(data);

  return [
    ['Total Deposit Amount', totalDepositAmount.toFixed(2)],
    [
      'Total CashBalance Deposit Amount',
      cashBalanceTotalDepositAmount.toFixed(2),
    ],
    [
      'Total DERIVATIVES Deposit Amount',
      derivativesTotalDepositAmount.toFixed(2),
    ],
    ['Total Cash Deposit Amount', cashTotalDepositAmount.toFixed(2)],
    [
      'Total CreditBalance Deposit Amount',
      creditBalanceTotalDepositAmount.toFixed(2),
    ],
    [
      'Total CreditBalanceSbl Deposit Amount',
      creditBalanceSblTotalDepositAmount.toFixed(2),
    ],
    [' ', ' '],
    ['Total Deposit QR', totalDepositAmountQR.toFixed(2)],
    [
      'Total CashBalance Deposit Amount',
      cashBalanceTotalDepositAmountQR.toFixed(2),
    ],
    [
      'Total DERIVATIVES Deposit Amount',
      derivativesTotalDepositAmountQR.toFixed(2),
    ],
    ['Total CASH Deposit Amount', cashTotalDepositAmountQR.toFixed(2)],
    [
      'Total CreditBalance Amount',
      creditBalanceTotalDepositAmountQR.toFixed(2),
    ],
    [
      'Total CreditBalanceSbl Amount',
      creditBalanceSblTotalDepositAmountQR.toFixed(2),
    ],
    [' ', ' '],
    ['Total Deposit ODD', totalDepositAmountODD.toFixed(2)],
    [
      'Total CashBalance Deposit Amount',
      cashBalanceTotalDepositAmountODD.toFixed(2),
    ],
    [
      'Total DERIVATIVES Deposit Amount',
      derivativesTotalDepositAmountODD.toFixed(2),
    ],
    ['Total CASH Deposit Amount', cashTotalDepositAmountODD.toFixed(2)],
    [
      'Total CreditBalance Amount',
      creditBalanceTotalDepositAmountODD.toFixed(2),
    ],
    [
      'Total CreditBalanceSbl Amount',
      creditBalanceSblTotalDepositAmountODD.toFixed(2),
    ],
    [' ', ' '],
    ['Total Withdraw Amount', totalWithdrawAmount.toFixed(2)],
    ['Total Withdraw KKP (less than 2M)', kkpTotalWithdrawAmount.toFixed(2)],
    ['Total Withdraw ATS (more than 2M)', atsTotalWithdrawAmount.toFixed(2)],
    [
      'Total CashBalance Withdraw Amount',
      cashBalanceTotalWithdrawAmount.toFixed(2),
    ],
    [
      'Total DERIVATIVES Withdraw Amount',
      derivativesTotalWithdrawAmount.toFixed(2),
    ],
    ['Total CASH Withdraw Amount', cashTotalWithdrawAmount.toFixed(2)],
    [
      'Total CreditBalance Withdraw Amount',
      creditBalanceTotalWithdrawAmount.toFixed(2),
    ],
    [
      'Total CreditBalanceSbl Withdraw Amount',
      creditBalanceSblTotalWithdrawAmount.toFixed(2),
    ],
    [' ', ' '],
    ['Total Refund Amount', totalRefundAmount.toFixed(2)],
    [
      'Total CashBalance Refund Amount',
      cashBalanceTotalRefundAmount.toFixed(2),
    ],
    [
      'Total DERIVATIVES Refund Amount',
      derivativesTotalRefundAmount.toFixed(2),
    ],
    ['Total CASH Refund Amount', cashTotalRefundAmount.toFixed(2)],
    [
      'Total CreditBalance Refund Amount',
      creditBalanceTotalRefundAmount.toFixed(2),
    ],
    [
      'Total CreditBalanceSbl Refund Amount',
      creditBalanceSblTotalRefundAmount.toFixed(2),
    ],
    [' ', ' '],
    [
      'Total Pending Deposit Transaction Amount',
      totalPendingDepositAmount.toFixed(2),
    ],
    [
      'Total Pending Withdraw Transaction Amount',
      totalPendingWithdrawAmount.toFixed(2),
    ],
  ];
}

function getNonGlobalTransactions(data: Transaction[]): Transaction[] {
  return data.filter(
    (transaction) =>
      transaction.channel.toLowerCase() !== 'billpayment' &&
      transaction.product.toLowerCase() !== 'globalequities' &&
      transaction.status.toLocaleLowerCase() !== 'processing' &&
      (transaction.status.toLowerCase() !== 'fail' ||
        transaction.state.toLowerCase() === 'refundsuccess')
  );
}

function getTotalDepositAmount(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.product.toLowerCase() !== 'globalequities'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getCashBalanceTotalDepositAmount(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.product.toLowerCase() === 'cashbalance'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getDerivativesTotalDepositAmount(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.product.toLowerCase() === 'derivatives'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getCashTotalDepositAmount(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.product.toLowerCase() === 'cash'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getCreditBalanceTotalDepositAmount(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.product.toLowerCase() === 'creditbalance'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getCreditBalanceSblTotalDepositAmount(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.product.toLowerCase() === 'creditbalancesbl'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getTotalDepositAmountQR(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.channel.toLowerCase() === 'qr'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getCashBalanceTotalDepositAmountQR(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.channel.toLowerCase() === 'qr' &&
      transaction.product.toLowerCase() === 'cashbalance'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getDerivativesTotalDepositAmountQR(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.channel.toLowerCase() === 'qr' &&
      transaction.product.toLowerCase() === 'derivatives'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getCashTotalDepositAmountQR(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.channel.toLowerCase() === 'qr' &&
      transaction.product.toLowerCase() === 'cash'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getCreditBalanceTotalDepositAmountQR(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.channel.toLowerCase() === 'qr' &&
      transaction.product.toLowerCase() === 'creditbalance'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getCreditBalanceSblTotalDepositAmountQR(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.channel.toLowerCase() === 'qr' &&
      transaction.product.toLowerCase() === 'creditbalancesbl'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getTotalDepositAmountODD(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.channel.toLowerCase() === 'odd'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getCashBalanceTotalDepositAmountODD(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.channel.toLowerCase() === 'odd' &&
      transaction.product.toLowerCase() === 'cashbalance'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getDerivativesTotalDepositAmountODD(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.channel.toLowerCase() === 'odd' &&
      transaction.product.toLowerCase() === 'derivatives'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getCashTotalDepositAmountODD(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.channel.toLowerCase() === 'odd' &&
      transaction.product.toLowerCase() === 'cash'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getCreditBalanceTotalDepositAmountODD(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.channel.toLowerCase() === 'odd' &&
      transaction.product.toLowerCase() === 'creditbalance'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getCreditBalanceSblTotalDepositAmountODD(data: Transaction[]): number {
  let totalDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.channel.toLowerCase() === 'odd' &&
      transaction.product.toLowerCase() === 'creditbalancesbl'
    ) {
      totalDepositAmount =
        totalDepositAmount + Number(transaction.transferAmount);
    }
  });

  return totalDepositAmount;
}

function getTotalWithdrawAmount(data: Transaction[]): number {
  let totalWithdrawAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'withdraw' &&
      transaction.product.toLowerCase() !== 'globalequities'
    ) {
      totalWithdrawAmount =
        totalWithdrawAmount + Number(transaction.transferAmount);
    }
  });

  return totalWithdrawAmount;
}

function getKkpTotalWithdrawAmount(data: Transaction[]): number {
  let totalWithdrawAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'withdraw' &&
      Number(transaction.transferAmount) <= 2000000
    ) {
      totalWithdrawAmount =
        totalWithdrawAmount + Number(transaction.transferAmount);
    }
  });

  return totalWithdrawAmount;
}

function getAtsTotalWithdrawAmount(data: Transaction[]): number {
  let totalWithdrawAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'withdraw' &&
      Number(transaction.transferAmount) > 2000000
    ) {
      totalWithdrawAmount =
        totalWithdrawAmount + Number(transaction.transferAmount);
    }
  });

  return totalWithdrawAmount;
}

function getCashBalanceTotalWithdrawAmount(data: Transaction[]): number {
  let totalWithdrawAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'withdraw' &&
      transaction.product.toLowerCase() === 'cashbalance'
    ) {
      totalWithdrawAmount =
        totalWithdrawAmount + Number(transaction.transferAmount);
    }
  });

  return totalWithdrawAmount;
}

function getDerivativesTotalWithdrawAmount(data: Transaction[]): number {
  let totalWithdrawAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'withdraw' &&
      transaction.product.toLowerCase() === 'derivatives'
    ) {
      totalWithdrawAmount =
        totalWithdrawAmount + Number(transaction.transferAmount);
    }
  });

  return totalWithdrawAmount;
}

function getCashTotalWithdrawAmount(data: Transaction[]): number {
  let totalWithdrawAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'withdraw' &&
      transaction.product.toLowerCase() === 'cash'
    ) {
      totalWithdrawAmount =
        totalWithdrawAmount + Number(transaction.transferAmount);
    }
  });

  return totalWithdrawAmount;
}

function getCreditBalanceTotalWithdrawAmount(data: Transaction[]): number {
  let totalWithdrawAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'withdraw' &&
      transaction.product.toLowerCase() === 'creditbalance'
    ) {
      totalWithdrawAmount =
        totalWithdrawAmount + Number(transaction.transferAmount);
    }
  });

  return totalWithdrawAmount;
}

function getCreditBalanceSblTotalWithdrawAmount(data: Transaction[]): number {
  let totalWithdrawAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'withdraw' &&
      transaction.product.toLowerCase() === 'creditbalancesbl'
    ) {
      totalWithdrawAmount =
        totalWithdrawAmount + Number(transaction.transferAmount);
    }
  });

  return totalWithdrawAmount;
}

function getTotalRefundAmount(data: Transaction[]): number {
  let totalRefundAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.state.toLowerCase() === 'refundsuccess' &&
      transaction.product.toLowerCase() !== 'globalequities'
    ) {
      const refundAmount =
        Number(transaction.requestedAmount) -
        Number(transaction.transferAmount);
      totalRefundAmount = totalRefundAmount + refundAmount;
    }
  });

  return totalRefundAmount;
}

function getCashBalanceTotalRefundAmount(data: Transaction[]): number {
  let totalRefundAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.state.toLowerCase() === 'refundsuccess' &&
      transaction.product.toLowerCase() === 'cashbalance'
    ) {
      const refundAmount =
        Number(transaction.requestedAmount) -
        Number(transaction.transferAmount);
      totalRefundAmount = totalRefundAmount + refundAmount;
    }
  });

  return totalRefundAmount;
}

function getDerivativesTotalRefundAmount(data: Transaction[]): number {
  let totalRefundAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.state.toLowerCase() === 'refundsuccess' &&
      transaction.product.toLowerCase() === 'derivatives'
    ) {
      const refundAmount =
        Number(transaction.requestedAmount) -
        Number(transaction.transferAmount);
      totalRefundAmount = totalRefundAmount + refundAmount;
    }
  });

  return totalRefundAmount;
}

function getCashTotalRefundAmount(data: Transaction[]): number {
  let totalRefundAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.state.toLowerCase() === 'refundsuccess' &&
      transaction.product.toLowerCase() === 'cash'
    ) {
      const refundAmount =
        Number(transaction.requestedAmount) -
        Number(transaction.transferAmount);
      totalRefundAmount = totalRefundAmount + refundAmount;
    }
  });

  return totalRefundAmount;
}

function getCreditBalanceTotalRefundAmount(data: Transaction[]): number {
  let totalRefundAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.state.toLowerCase() === 'refundsuccess' &&
      transaction.product.toLowerCase() === 'creditbalance'
    ) {
      const refundAmount =
        Number(transaction.requestedAmount) -
        Number(transaction.transferAmount);
      totalRefundAmount = totalRefundAmount + refundAmount;
    }
  });

  return totalRefundAmount;
}

function getCreditBalanceSblTotalRefundAmount(data: Transaction[]): number {
  let totalRefundAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.state.toLowerCase() === 'refundsuccess' &&
      transaction.product.toLowerCase() === 'creditbalancesbl'
    ) {
      const refundAmount =
        Number(transaction.requestedAmount) -
        Number(transaction.transferAmount);
      totalRefundAmount = totalRefundAmount + refundAmount;
    }
  });

  return totalRefundAmount;
}

function getPendingDepositAmount(data: Transaction[]): number {
  let totalPendingDepositAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.product.toLowerCase() !== 'globalequities' &&
      transaction.status.toLowerCase() === 'pending'
    ) {
      totalPendingDepositAmount += Number(transaction.transferAmount);
    }
  });

  return totalPendingDepositAmount;
}

function getPendingWithdrawAmount(data: Transaction[]): number {
  let totalPendingWithdrawAmount = 0;
  data.forEach((transaction) => {
    if (
      transaction.transactionType.toLowerCase() === 'withdraw' &&
      transaction.state.toLowerCase() !== 'refundsuccess' &&
      transaction.product.toLowerCase() !== 'globalequities' &&
      transaction.status.toLowerCase() === 'pending'
    ) {
      totalPendingWithdrawAmount += Number(transaction.transferAmount);
    }
  });

  return totalPendingWithdrawAmount;
}

// Combine transaction and summary data
function _combineData(transactions: unknown[][], summary: unknown[][]) {
  return [
    reconcileTransactionColumnHeaders,
    ...transactions,
    [],
    reconcileTransactionTotalColumnHeaders,
    ...summary,
  ];
}

// Function to generate email title
function getEmailTitle(currentDate: Date): string {
  const currentDateISO = currentDate.toISOString().substring(0, 10);
  const title = `PI_APP_DW_Daily Reconcile Report ${currentDateISO}`;

  return `[${process.env.ENVIRONMENT}] - ${title}`;
}

const downloadDepositWithdrawDailyReport = async (
  event
): Promise<APIGatewayProxyResult> => {
  try {
    console.info('calling downloadDepositWithdrawDailyReport');
    const config = await getConfig();
    const [createdAtFrom, createdAtTo] =
      getDateTimeFromAndDateTimeToDownloadEndpoint(
        new Date(event.body.dateFrom),
        new Date(event.body.dateTo)
      );
    console.log(
      `parameters dateFrom: ${createdAtFrom}, dateTo: ${createdAtTo}`
    );

    const piAppData = await getPiAppDepositWithdrawReportData(
      config,
      createdAtFrom,
      createdAtTo
    );

    const combinedBuffer = Buffer.concat(piAppData.csvBufferList);
    const base64Data = combinedBuffer.toString('base64');

    return {
      statusCode: 200,
      headers: {
        'Access-Control-Allow-Origin': '*',
        'Content-Type': 'application/csv; charset=shift_jis',
        'Content-Disposition': 'attachment; filename="report.csv"',
      },
      body: base64Data,
      isBase64Encoded: true,
    };
  } catch (e) {
    console.error('Failed get reports\n' + JSON.stringify(e));
    throw e;
  }
};

const run = middyfy(_run);
export { run, sendPiAppDepositWithdrawReport };

export const main = middyfy(downloadDepositWithdrawDailyReport);
