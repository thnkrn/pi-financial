import {
  getTransactionSummary,
  Product,
  Transaction,
  TransactionType,
} from '@api/wallet/transaction-summary';
import schema from '@functions/fund/fund-account-opening-state-report/schema';
import {
  formatJSONResponse,
  ValidatedEventAPIGatewayProxyEvent,
} from '@libs/api-gateway';
import { convertDataToBuffer } from '@libs/csv-utils';
import { formatDate, formatDateTimeUTC, toTHDateTime } from '@libs/date-utils';
import { convertFilesToAttachments, sendEmailToSES } from '@libs/email-utils';
import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';
import { getTimeFromString } from '@libs/time-utils';
import console from 'console';

const fxTransactionReportHeaders = [
  'FI Arrangement Number',
  'Previous FI Arrangement Number',
  'Cancellation Flag',
  'Set Up Reason Type',
  'FX Arrangement Type',
  'Trade Date',
  'Maturity Date',
  'Underlying Flag',
  'Transaction Purpose',
  'Counterparty Unique ID Type',
  'Counterparty Unique ID',
  'Counterparty Names',
  'Counterparty Country Code',
  'Buy Currency',
  'Buy Amount',
  'Sell Currency',
  'Sell Amount',
  'Exchange Rate',
  'Description',
];

const NetFxPositionReportHeaders = [
  'Data Set Date',
  'Accumulated Net of the previous day',
  'Total Buy Amount (จำนวนเงินตราต่างประเทศซื้อ)',
  'Total Sell Amount (จำนวนเงินตราต่างประเทศขาย)',
  'Accumulated Net at the end of the day (Accumulate net of the previous day Total Buy - Total Sell)',
];

const FCDReportHeaders = [
  'Account Number/swift codes',
  'Arrangement Contract Date',
  'FI Unique ID',
  'FI Name',
  'Account Name',
  'Currency',
  'Description',
];

async function getConfig() {
  const [walletServiceHost, GlobalBotFxReportRecipients, geCutOffTimeUtc] =
    await getConfigFromSsm('wallet', [
      'wallet-srv-host',
      'global-bot-fx-report-recipients',
      'ge-cutoff-time-utc',
    ]);

  return {
    walletServiceHost,
    GlobalBotFxReportRecipients,
    geCutOffTimeUtc,
  };
}

const _run: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  const month = Number(event.body.month);
  const year = Number(event.body.year);

  console.log('Running Global Bot FX Report');
  console.log('input month ' + month);
  console.log('input year ' + year);

  await _handle(month, year);
  return formatJSONResponse({
    status: 'done',
  });
};

const scheduleRunner = async () => {
  const currentDate = new Date();
  await _handle(currentDate.getMonth() + 1, currentDate.getUTCFullYear());
};

const _handle = async (month: number, year: number) => {
  const config = await getConfig();
  const [fromDate, toDate] = getDateTimeFromAndDateTimeTo(
    month,
    year,
    config.geCutOffTimeUtc
  );
  console.log(
    `Getting data from ${fromDate.toISOString()} to ${toDate.toISOString()}`
  );

  const resp = await getTransactionSummary(
    config.walletServiceHost,
    Product.GlobalEquities,
    fromDate,
    toDate,
    true
  );

  const transactions = resp.data.transactions
    .filter(
      (t) =>
        // Withdraw - Only Success & MarkUp > 0
        (t.transactionType == TransactionType.Withdraw.toString() &&
          t.status == 'Success' &&
          t.globalTransfer?.fxMarkUpRate > 0) ||
        // Deposit - Success + Pending on FxTransferFailed or FxTransferInsufficientBalance & MarkUp > 0
        (t.transactionType == TransactionType.Deposit.toString() &&
          t.globalTransfer?.fxMarkUpRate > 0 &&
          (t.status == 'Success' ||
            (t.status == 'Pending' &&
              (t.currentState == 'FxTransferFailed' ||
                t.currentState == 'FxTransferInsufficientBalance'))))
    )
    .sort(
      (a, b) =>
        toTHDateTime(a.createdAt).getTime() -
        toTHDateTime(b.createdAt).getTime()
    );

  // Report Name
  const reportDate = formatDateTimeUTC(toDate, 'YYYYMMDD');
  const fxTransactionsReportName = `MFXSC_${reportDate}_FX_transaction.csv`;
  const fcdReportName = `MFXSC_${reportDate}_FCD.csv`;
  const netFxPositionReportName = `MFXSC_${reportDate}_Net_FX_Position.csv`;

  // Build Data
  const fxTransactionsReportData = buildFxTransactionsReportData(
    transactions,
    toDate
  );
  const fcdReportData = buildFCDReportDate();
  const netFxPositionReportData = buildNetFxPositionReportData(transactions);

  // To buffer
  const fxTransactionsReportDataBuffer = await convertDataToBuffer(
    fxTransactionsReportData
  );
  const fcdReportDataBuffer = await convertDataToBuffer(fcdReportData);
  const netFxPositionReportDataBuffer = await convertDataToBuffer(
    netFxPositionReportData
  );

  await sendEmail(
    formatDateTimeUTC(toDate, 'YYYY-MM-DD'),
    config.GlobalBotFxReportRecipients,
    [fxTransactionsReportName, netFxPositionReportName, fcdReportName],
    [
      fxTransactionsReportDataBuffer,
      netFxPositionReportDataBuffer,
      fcdReportDataBuffer,
    ]
  );
};

async function sendEmail(
  dateStr: string,
  recipient: string,
  fileNames: string[],
  fileContents: Buffer[]
) {
  const emailSubject = `[${process.env.ENVIRONMENT}] - BOT Report For GE FX Spread ${dateStr}`;
  const attachments = convertFilesToAttachments(fileNames, fileContents);
  await sendEmailToSES(recipient, emailSubject, attachments);
}

function getDateTimeFromAndDateTimeTo(
  month: number,
  year: number,
  cutOffTimeString: string
): Date[] {
  // From Last Day of Previous Month to Last Day of Current Month on 22:00 UTC time
  // e.g. 2024-03-31 22:00:00 UTC to 2024-04-30 22:00:00 UTC
  // Using month - 1 to get the last day of the previous month (month as zero-based index)
  const fromDate = new Date(year, month - 1, 0);
  const toDate = new Date(year, month, 0);

  console.log('month ' + month);
  console.log('date before set hour ' + fromDate.toISOString());
  console.log('date before set hour ' + toDate.toISOString());
  const cutOffTime = getTimeFromString(cutOffTimeString);
  fromDate.setUTCHours(cutOffTime[0], cutOffTime[1], cutOffTime[2]);
  toDate.setUTCHours(cutOffTime[0], cutOffTime[1], cutOffTime[2]);

  return [fromDate, toDate];
}

function buildFxTransactionsReportData(
  transactions: Transaction[],
  reportDate: Date
) {
  const preData = [
    ['แบบรายงานธุรกรรมซื้อ-ขาย แลกเปลี่ยนเงินตราต่างประเทศ'],
    ['Organization ID:', 'A94'],
    [
      'วันที่ Data Set Date  (YYYY-MM-DD) :',
      formatDateTimeUTC(reportDate, 'YYYY-MM-DD'),
    ],
    [],
  ];

  const data = transactions.map((t) => [
    t.transactionNo, // FI Arrangement Number
    '', // Previous FI Arrangement Number
    0, // Cancellation Flag
    'New Contract', // Set Up Reason Type
    '018101 Spot – Today', // FX Arrangement Type
    formatDate(t.createdAt, 'YYYY-MM-DD'), // Trade Date
    formatDate(t.createdAt, 'YYYY-MM-DD'), // Maturity Date
    '1', // Underlying Flag
    '318075 เงินลงทุนในหลักทรัพย์', // Transaction Purpose
    t.customerAdditionInfo?.customerTypeId ?? '', // Counterparty Unique ID Type
    t.customerAdditionInfo?.idNumber ?? '', // Counterparty Unique ID
    t.customerAdditionInfo?.fullName ?? '', // Counterparty Names
    t.customerAdditionInfo?.nationality, // Counterparty Country Code
    t.toCurrency, // Buy Currency
    t.transferAmount, // Buy Amount
    t.requestedCurrency, // Sell Currency
    t.requestedAmount, // Sell Amount
    t.globalTransfer?.requestedFxRate, // Exchange Rate
    '', // Description
  ]);
  return [...preData, fxTransactionReportHeaders, ...data];
}

function buildNetFxPositionReportData(transactions: Transaction[]) {
  const preData = [['แบบรายงานฐานะเงินตราต่างประเทศสุทธิ'], []];
  const data = [];
  const dailyData: { [date: string]: { totalBuy: number; totalSell: number } } =
    {};

  // accumulate total buy and sell amount
  transactions.forEach((t) => {
    const date = formatDate(t.createdAt, 'YYYY-MM-DD');
    if (!dailyData[date]) {
      dailyData[date] = { totalBuy: 0, totalSell: 0 };
    }
    if (t.transactionType == TransactionType.Withdraw.toString()) {
      dailyData[date].totalSell += Number(t.requestedAmount);
    } else {
      dailyData[date].totalBuy += Number(t.transferAmount);
    }
  });

  // sort by date
  const dates = Object.keys(dailyData).sort();

  // calculate report data
  let previousDateAccumulatedNet = 0;
  for (let i = 0; i < dates.length; i++) {
    const d = dates[i];
    const totalBuy = dailyData[d].totalBuy;
    const totalSell = dailyData[d].totalSell;
    const accumulatedNet = previousDateAccumulatedNet + totalBuy - totalSell;
    data.push([
      d, // Data Set Date
      previousDateAccumulatedNet.toFixed(2), // Accumulated Net of the previous day
      totalBuy.toFixed(2), // Total Buy Amount
      totalSell.toFixed(2), // Total Sell Amount
      accumulatedNet.toFixed(2), // Accumulated Net at the end of the day
    ]);
    previousDateAccumulatedNet = accumulatedNet;
  }

  return [...preData, NetFxPositionReportHeaders, ...data];
}

function buildFCDReportDate() {
  const preData = [
    [
      'แบบรายงานการเปิดบัญชีเงินฝากเงินตราต่างประเทศเพื่อการประกอบธุรกิจเงินตราต่างประเทศ ',
    ],
    [],
  ];
  const data = [
    [
      '4680000556840 / SICOTHBK',
      '2022-11-02',
      '014',
      'The Siam Commercial Bank Pcl.',
      'Pi Securities Public Company Limited for Client',
      'USD',
      '',
    ],
  ];
  return [...preData, FCDReportHeaders, ...data];
}

const run = middyfy(_run);

export { run, scheduleRunner };
