import { getTransactions, Transaction } from '@api/wallet/transactions-v2';
import schema from '@functions/wallet/deposit-bill-payment-daily-report/schema';
import {
  formatJSONResponse,
  ValidatedEventAPIGatewayProxyEvent,
} from '@libs/api-gateway';
import { convertDataToBuffer } from '@libs/csv-utils';
import { convertFilesToAttachments, sendEmailToSES } from '@libs/email-utils';
import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';
import * as console from 'console';
import * as process from 'process';
import { toTHDateTime } from '@libs/date-utils';
import { Product } from '@api/wallet/transaction-summary';
import { APIGatewayProxyResult } from 'aws-lambda';

// reconcile non-global constant
const transactionSectionHeader = [
  'Account Code',
  'KKP Transaction Id',
  'Pi Transaction Id',
  'Transaction Type',
  'Channel',
  'Deposit By',
  'Purpose Of Deposit',
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

const amountHeaders = ['', 'Amount in THB'];

const totalDepositSummaries = {
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
    display: 'Total CreditBalance Deposit Amount',
    product: Product.CreditBalance,
  },
  totalCreditBalanceSblDeposit: {
    display: 'Total CreditBalanceSbl Deposit Amount',
    product: Product.CreditBalanceSbl,
  },
};

const totalRefundSummaries = {
  totalRefund: { display: 'Total Refund Amount', product: null },
  totalCashBalanceDeposit: {
    display: 'Total CashBalance Refund Amount',
    product: Product.CashBalance,
  },
  totalDERIVATIVESDeposit: {
    display: 'Total DERIVATIVES Refund Amount',
    product: Product.Derivatives,
  },
  totalCASHDeposit: {
    display: 'Total CASH Refund Amount',
    product: Product.Cash,
  },
  totalCreditBalanceDeposit: {
    display: 'Total CreditBalance Refund Amount',
    product: Product.CreditBalance,
  },
  totalCreditBalanceSblDeposit: {
    display: 'Total CreditBalanceSbl Refund Amount',
    product: Product.CreditBalanceSbl,
  },
};

const totalPendingSummaries = {
  totalPending: {
    display: 'Total Pending Deposit Transaction Amount',
    product: null,
  },
};

interface Config {
  walletServiceHost: string;
  reconcileRecipient: string;
}

async function getConfig(): Promise<Config> {
  const [walletServiceHost, reconcileRecipient] = await getConfigFromSsm(
    'wallet',
    ['wallet-srv-host', 'pi-app-dw-report-recipient']
  );

  return {
    walletServiceHost,
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

// Function to fetch data from DB
async function _getReports(date: Date, config: Config) {
  try {
    const [createdAtFrom, createdAtTo] = getDateTimeFromAndDateTimeTo(date);
    const piAppData = await getBillPaymentDepositReportData(
      config,
      createdAtFrom,
      createdAtTo
    );

    const fileNames = [...piAppData.fileNames];
    const csvBufferList = [...piAppData.csvBufferList];

    return { fileNames, csvBufferList };
  } catch (e) {
    console.error('Failed get reports\n' + JSON.stringify(e));
    throw e;
  }
}

async function getBillPaymentDepositReportData(
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
  const transactionSection = getransactionSection(nonGlobalTransactions);
  const depositSummarySection = getDepositSummarySections(
    nonGlobalTransactions
  );
  const refundSummarySection = getRefundSummarySections(nonGlobalTransactions);
  const pendingSummarySection = getPendingSummarySections(
    nonGlobalTransactions
  );

  console.info(`[_getReports] Combining transaction data`);
  const csvData = [
    transactionSectionHeader,
    ...transactionSection,
    [],
    amountHeaders,
    ...depositSummarySection,
    [],
    ...refundSummarySection,
    [],
    ...pendingSummarySection,
  ];
  const fileNames = [
    'Cross_Bank_Bill_Payment_Deposit_Daily_Report_' + dateStr + '.csv',
  ];

  console.info(`[_getReports] Converting data to buffer`);
  const csvBufferList = [await convertDataToBuffer(csvData)];

  return { fileNames, csvBufferList };
}

function getDepositSummarySections(transactions: Transaction[]) {
  const data = [];
  for (const key of Object.keys(totalDepositSummaries)) {
    const total = totalDepositSummaries[key];
    data.push(
      generateTotalRow(
        total.display,
        getTotalDepositByProduct(transactions, total.product)
      )
    );
  }
  return data;
}

function getRefundSummarySections(transactions: Transaction[]) {
  const data = [];
  for (const key of Object.keys(totalRefundSummaries)) {
    const total = totalRefundSummaries[key];
    data.push(
      generateTotalRow(
        total.display,
        getTotalRefundByProduct(transactions, total.product)
      )
    );
  }
  return data;
}

function getPendingSummarySections(transactions: Transaction[]) {
  const data = [];
  for (const key of Object.keys(totalPendingSummaries)) {
    const total = totalPendingSummaries[key];
    data.push(
      generateTotalRow(
        total.display,
        getTotalPendingByProduct(transactions, total.product)
      )
    );
  }
  return data;
}

function getTotalRefundByProduct(
  transactions: Transaction[],
  product: string = null
) {
  const transactionByProduct = transactions.filter((t) => {
    const isRefundSuccess = t.state.toLowerCase() === 'refundsuccess';
    const isProductMatch =
      product === null ||
      product === undefined ||
      t.product?.toLowerCase() === Product[product]?.toString().toLowerCase();

    return isRefundSuccess && isProductMatch;
  });

  return transactionByProduct.reduce(
    (total, transaction) =>
      total +
      (Number(transaction.requestedAmount) -
        Number(transaction.transferAmount)),
    0
  );
}

function getTotalDepositByProduct(
  transactions: Transaction[],
  product: string = null
): number {
  const transactionByProduct = transactions.filter((t) => {
    const isRefundSuccess = t.state.toLowerCase() !== 'refundsuccess';
    const isProductMatch =
      product === null ||
      product === undefined ||
      t.product?.toLowerCase() === Product[product]?.toString().toLowerCase();

    return isRefundSuccess && isProductMatch;
  });

  return transactionByProduct.reduce(
    (total, transaction) => total + Number(transaction.transferAmount),
    0
  );
}

function getTotalPendingByProduct(
  transactions: Transaction[],
  product: string = null
): number {
  const transactionByProduct = transactions.filter((t) => {
    const isPending = t.status.toLowerCase() === 'pending';
    const isProductMatch =
      product === null ||
      product === undefined ||
      t.product?.toLowerCase() === Product[product]?.toString().toLowerCase();

    return isPending && isProductMatch;
  });

  return transactionByProduct.reduce(
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
function getransactionSection(data: Transaction[]) {
  // sort data by createdAt
  const sortedData = data.sort(
    (a, b) =>
      toTHDateTime(a.createdAt).getTime() - toTHDateTime(b.createdAt).getTime()
  );
  return sortedData.map((obj) => [
    obj.accountCode,
    obj.externalReference,
    obj.transactionNo,
    obj.transactionType,
    obj.channel,
    mapChannel(obj.externalChannel),
    obj.purpose,
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

function mapChannel(channel: string) {
  if (
    channel.toLowerCase() === 'ribmobile' ||
    channel.toLowerCase() === 'itmx'
  ) {
    return 'Mobile Banking';
  }
  return channel;
}

function getNonGlobalTransactions(data: Transaction[]): Transaction[] {
  return data.filter(
    (transaction) =>
      transaction.channel.toLowerCase() === 'billpayment' &&
      transaction.transactionType.toLowerCase() === 'deposit' &&
      transaction.status.toLocaleLowerCase() !== 'processing' &&
      (transaction.status.toLowerCase() !== 'fail' ||
        transaction.state.toLowerCase() === 'refundsuccess')
  );
}

// Function to generate email title
function getEmailTitle(currentDate: Date): string {
  const currentDateISO = currentDate.toISOString().substring(0, 10);
  const title = `Cross Bank Bill Payment Reconcile Report ${currentDateISO}`;

  return `[${process.env.ENVIRONMENT}] - ${title}`;
}

const downloadBillPaymentDepositReport = async (
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

    const piAppData = await getBillPaymentDepositReportData(
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

const sendBillPaymentDepositReport = async () => {
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

const run = middyfy(_run);
export { run, sendBillPaymentDepositReport };

export const main = middyfy(downloadBillPaymentDepositReport);
