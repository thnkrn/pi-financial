import {
  getTransactionSummary,
  Product,
  Transaction,
  TransactionSummary,
} from '@api/wallet/transaction-summary';
import schema from '@functions/fund/fund-account-opening-state-report/schema';
import {
  formatJSONResponse,
  ValidatedEventAPIGatewayProxyEvent,
} from '@libs/api-gateway';
import { convertDataToBuffer } from '@libs/csv-utils';
import {
  areDatesEqual,
  isDateBetween,
  isTimeGreaterThan,
  toTHDateTime,
} from '@libs/date-utils';
import { convertFilesToAttachments, sendEmailToSES } from '@libs/email-utils';
import { middyfy } from '@libs/lambda';
import { storeFileToS3 } from '@libs/s3-utils';
import { getConfigFromSsm } from '@libs/ssm-config';
import { getTimeFromString } from '@libs/time-utils';
import { APIGatewayProxyResult } from 'aws-lambda';
import * as console from 'console';
import * as process from 'process';

const allDayFilePrefix = 'reconcile_report_all_day_';

const reconcileTransactionColumnHeaders = [
  'AccountCode',
  'CustomerName',
  'TransactionId',
  'TransactionType',
  'Channel',
  'Product',
  'BankName',
  'BankAccountName',
  'BankAccountNo',
  'AmountThb',
  'BankFeeRate',
  'BankRequestDatetime',
  'BankResponseDateTime',
  'FxRate',
  'Actual FxRate',
  'ConvertedAmountUsd',
  'FxRequestDateTime',
  'FxResponseDateTime',
  'FromAccountExante',
  'ToAccountExante',
  'AmountUsd',
  'TransactionFee',
  'ExanteRequestDateTime',
  'ExanteResponseDateTime',
  'RefundAmountThb',
  'NetAmountThb',
  'Status',
  'Reason',
  'FxMarkUp',
];

const reconcileTransactionTotalColumnHeaders = [
  '',
  'Number of transactions',
  'Amount in THB',
  'Amount in USD',
];

const fxMarkupSummary = {
  depositTransaction: 0,
  withdrawTransaction: 0,
  depositFxMarkupAmount: 0,
  withdrawFxMarkupAmount: 0,
};

interface Config {
  walletServiceHost: string;
  reconcileRecipient: string;
  geCutOffTimeUtc: number[];
  daylightSavingStartString: string;
  daylightSavingEndString: string;
  firstIntraS3Bucket: string;
  allDayS3Bucket: string;
}

async function getConfig(): Promise<Config> {
  const [
    walletServiceHost,
    reconcileRecipient,
    geCutOffTimeUtcString,
    daylightSavingStartString,
    daylightSavingEndString,
    firstIntraS3Bucket,
    allDayS3Bucket,
  ] = await getConfigFromSsm('wallet', [
    'wallet-srv-host',
    'reconcile-recipient',
    'ge-cutoff-time-utc',
    'daylight-saving-start-datetime',
    'daylight-saving-end-datetime',
    'first-intra-s3-bucket',
    'all-day-s3-bucket',
  ]);

  const geCutOffTimeUtc = getTimeFromString(geCutOffTimeUtcString);

  return {
    walletServiceHost,
    reconcileRecipient,
    geCutOffTimeUtc,
    daylightSavingStartString,
    daylightSavingEndString,
    firstIntraS3Bucket,
    allDayS3Bucket,
  };
}

const _run: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  const config = await getConfig();
  const date = new Date(event.body.requestDate);
  const subject = getGlobalEmailTitle(date);

  console.info(`Calling _getReport date: ${date} config: ${config}`);
  const report = await _getReport(date, config);

  console.info(`Calling _sendReport`);
  await _sendReport(
    report.fileName,
    subject,
    report.csvBuffer,
    config.reconcileRecipient
  );
  console.info(`Done`);

  return formatJSONResponse({
    status: 'done',
  });
};

const sendGlobalReconcileReport = async () => {
  const config = await getConfig();
  const currentDate = new Date();

  console.info(
    `[sendGlobalReconcileReport] Calling _getReport date: ${currentDate} config: ${config}`
  );
  const subject = getGlobalEmailTitle(currentDate);
  const report = await _getReport(currentDate, config);

  console.info(`[sendGlobalReconcileReport] Calling _sendReport`);
  await _sendReport(
    report.fileName,
    subject,
    report.csvBuffer,
    config.reconcileRecipient
  );

  const bucket = config.allDayS3Bucket;
  await _storeReportToS3(bucket, report.csvBuffer, report.fileName);

  console.info(`[sendGlobalReconcileReport] Done`);
};

async function _storeReportToS3(
  bucketName: string,
  fileContent: Buffer,
  fileName: string
) {
  await storeFileToS3(bucketName, fileContent, fileName);
}

async function _getReport(date: Date, config: Config) {
  try {
    const dateStr = date.toISOString().slice(0, 10);
    const [createdAtFrom, createdAtTo] = getDateTimeFromAndDateTimeTo(
      date,
      config.geCutOffTimeUtc,
      config.daylightSavingStartString,
      config.daylightSavingEndString
    );

    console.log(
      `Getting repost from Date ${date.toDateString()} Time ${createdAtFrom.toISOString()} to ${createdAtTo.toISOString()}`
    );

    const response = await getTransactionSummary(
      config.walletServiceHost,
      Product.GlobalEquities,
      createdAtFrom,
      createdAtTo
    );

    console.info(`[_getReport] Formatting transaction`);
    const reconcileTxns = _reformatTransactions(response.data.transactions);
    const reconcileTxnTotal = _reformatTransactionSummary(
      response.data.transactionSummary
    );

    console.info(`[_getReport] Combining transaction data`);
    const combinedData = _combineData(reconcileTxns, reconcileTxnTotal);

    console.info(`[_getReport] Converting data to buffer`);
    const csvBuffer = await convertDataToBuffer(combinedData);

    const fileName = allDayFilePrefix + dateStr + '.csv';

    return { fileName, csvBuffer };
    //   }
    // }
  } catch (e) {
    console.error('Failed get reports\n' + JSON.stringify(e));
    throw e;
  }
}

async function _sendReport(
  fileName: string,
  subject: string,
  fileContents: Buffer,
  recipient: string
) {
  try {
    console.info('[_sendReport] convert files to attachments');
    const attachment = convertFilesToAttachments([fileName], [fileContents]);
    console.info('[_sendReport] calling sendEmailToSES');
    await sendEmailToSES(recipient, subject, attachment);
  } catch (e) {
    console.error('Failed to send reconcile report\n' + JSON.stringify(e));
    throw e;
  }
}

function getDateTimeFromAndDateTimeTo(
  date: Date,
  cutOffTime: number[],
  daylightSavingStartDateString: string,
  daylightSavingEndDateString: string
): Date[] {
  const fromTime = [...cutOffTime];
  const toTime = [...cutOffTime];
  const daylightSavingStartDate = new Date(daylightSavingStartDateString);
  const daylightSavingEndDate = new Date(daylightSavingEndDateString);
  const createdAtFrom = new Date(date);
  const createdAtTo = new Date(date);

  console.log('Daylight Saving Start ', daylightSavingStartDate);
  console.log('Daylight Saving End ', daylightSavingEndDate);
  console.log('Current Date ', date);

  // add date offset when daylight saving hours is later than cutoff time
  if (
    isTimeGreaterThan(daylightSavingStartDate, cutOffTime[0], cutOffTime[1])
  ) {
    console.log('add dst offset for start date');
    daylightSavingStartDate.setDate(daylightSavingStartDate.getDate() + 1);
  }
  if (isTimeGreaterThan(daylightSavingEndDate, cutOffTime[0], cutOffTime[1])) {
    console.log('add dst offset for end date');
    daylightSavingEndDate.setDate(daylightSavingEndDate.getDate() + 1);
  }

  // calculate from/to time based on daylight saving
  if (areDatesEqual(date, daylightSavingStartDate)) {
    console.log('First day of Daylight Saving Period');
    toTime[0] = toTime[0] - 1;
  }
  if (isDateBetween(date, daylightSavingStartDate, daylightSavingEndDate)) {
    console.log('During Daylight Saving Period');
    toTime[0] = toTime[0] - 1;
    fromTime[0] = fromTime[0] - 1;
  }
  if (areDatesEqual(date, daylightSavingEndDate)) {
    console.log('Last day of Daylight Saving Period');
    fromTime[0] = fromTime[0] - 1;
  }

  createdAtFrom.setDate(date.getDate() - 1);
  createdAtFrom.setHours(fromTime[0], fromTime[1], fromTime[2], fromTime[3]);
  createdAtTo.setHours(toTime[0], toTime[1], toTime[2], toTime[3]);

  return [createdAtFrom, createdAtTo];
}

function _reformatTransactions(data: Transaction[]) {
  fxMarkupSummary.depositFxMarkupAmount =
    fxMarkupSummary.depositTransaction =
    fxMarkupSummary.withdrawFxMarkupAmount =
    fxMarkupSummary.withdrawTransaction =
      0;

  return data.map((obj) => {
    const amountThb =
      obj.transactionType == 'Deposit'
        ? Number(obj.requestedAmount)
        : -obj.transferAmount;
    const amountUsd =
      obj.transactionType == 'Deposit'
        ? Number(obj.transferAmount)
        : -obj.requestedAmount;
    let actualFxRate =
      obj.transactionType == 'Deposit'
        ? obj.globalTransfer.fxConfirmedExchangeRate
        : obj.globalTransfer.requestedFxRate;
    const fxRate = obj.globalTransfer.requestedFxRate;
    let fxMarkUp = '0';
    if (obj.globalTransfer && obj.globalTransfer.fxMarkUpRate > 0) {
      actualFxRate = obj.globalTransfer.actualFxRate;
      fxMarkUp =
        obj.transactionType == 'Deposit'
          ? calculateFxMarkUpForDeposit(
              amountThb,
              obj.globalTransfer?.fxConfirmedExchangeAmount
            )
          : calculateFxMarkUpForWithdraw(fxRate, actualFxRate, amountUsd);

      if (obj.status == 'Success') {
        if (obj.transactionType == 'Deposit') {
          fxMarkupSummary.depositTransaction++;
          fxMarkupSummary.depositFxMarkupAmount += +fxMarkUp;
        } else {
          fxMarkupSummary.withdrawTransaction++;
          fxMarkupSummary.withdrawFxMarkupAmount += +fxMarkUp;
        }
      }
    }

    const bankRequestTime =
      obj.qrDeposit?.depositQrGenerateDateTime ??
      obj.oddDeposit?.otpConfirmedDateTime ??
      obj.atsDeposit?.otpConfirmedDateTime ??
      obj.oddWithdraw?.otpConfirmedDateTime;

    const bankResponseTime =
      obj.qrDeposit?.paymentReceivedDateTime ??
      obj.oddDeposit?.paymentReceivedDateTime ??
      obj.atsDeposit?.paymentReceivedDateTime ??
      obj.oddWithdraw?.paymentDisbursedDateTime;

    return [
      obj.customerCode,
      obj.customerName,
      obj.transactionNo,
      obj.transactionType,
      obj.channel,
      obj.product,
      obj.bankName,
      obj.bankAccountName,
      obj.bankAccountNo,
      amountThb,
      obj.fee ?? '0.00',
      bankRequestTime ? toTHDateTime(bankRequestTime).toISOString() : '',
      bankResponseTime ? toTHDateTime(bankResponseTime).toISOString() : '',
      fxRate,
      actualFxRate,
      obj.globalTransfer.fxConfirmedAmount &&
        obj.globalTransfer.fxConfirmedAmount.toFixed(2),
      obj.globalTransfer.fxInitiateRequestDateTime &&
        toTHDateTime(
          obj.globalTransfer.fxInitiateRequestDateTime
        ).toISOString(),
      obj.globalTransfer.fxConfirmedDateTime &&
        toTHDateTime(obj.globalTransfer.fxConfirmedDateTime).toISOString(),
      obj.globalTransfer.transferFromAccount,
      obj.globalTransfer.transferToAccount,
      amountUsd,
      obj.globalTransfer?.transferFee?.toFixed(2) ?? '0.00',
      obj.globalTransfer.transferRequestTime &&
        toTHDateTime(obj.globalTransfer.transferRequestTime).toISOString(),
      obj.globalTransfer.transferCompleteTime &&
        toTHDateTime(obj.globalTransfer.transferCompleteTime).toISOString(),
      (obj.refundInfo?.amount && obj.refundInfo?.amount.toFixed(2)) ?? '0.00',
      obj.transactionType == 'Deposit'
        ? formatAmount(obj.netAmount, false)
        : formatAmount(obj.netAmount, true),
      obj.status,
      obj.failedReason,
      fxMarkUp,
    ];
  });
}

function calculateFxMarkUpForWithdraw(
  markedUpFxRate: number,
  actualFxRate: number,
  amount: number
): string {
  const fxMarkUp = Math.abs((markedUpFxRate - actualFxRate) * amount);
  if (isNaN(fxMarkUp)) {
    return '0';
  }
  return formatDecimal(fxMarkUp);
}

function calculateFxMarkUpForDeposit(
  receivedAmount: string | number,
  exchangeAmount: string | number
): string {
  if (receivedAmount == null || exchangeAmount == null) {
    return '0';
  }
  return formatDecimal(Number(receivedAmount) - Number(exchangeAmount));
}

function formatAmount(value: string, negative: boolean): string {
  if (isNaN(Number(value))) {
    return '0.00';
  }
  if (negative) {
    return (-Number(value)).toFixed(2);
  }
  return Number(value).toFixed(2);
}

function _reformatTransactionSummary(data: TransactionSummary) {
  return [
    [
      'Total Success Deposit QR',
      data.successQrDepositCount,
      data.successQrDepositAmountThb.toFixed(2),
      data.successQrDepositAmountUsd.toFixed(2),
    ],
    [
      'Total Success Deposit ODD - KBANK',
      data.successOddKBankDepositCount,
      data.successOddKBankDepositAmountThb.toFixed(2),
      data.successOddKBankDepositAmountUsd.toFixed(2),
    ],
    [
      'Total Success Deposit ODD - SCB',
      data.successOddScbDepositCount,
      data.successOddScbDepositAmountThb.toFixed(2),
      data.successOddScbDepositAmountUsd.toFixed(2),
    ],
    [
      'Total Success Deposit ODD - KTB',
      data.successOddKtbDepositCount,
      data.successOddKtbDepositAmountThb.toFixed(2),
      data.successOddKtbDepositAmountUsd.toFixed(2),
    ],
    [
      'Total Success Deposit ODD - BBL',
      data.successOddBblDepositCount,
      data.successOddBblDepositAmountThb.toFixed(2),
      data.successOddBblDepositAmountUsd.toFixed(2),
    ],
    [
      'Total Success Deposit ODD - BAY',
      data.successOddBayDepositCount,
      data.successOddBayDepositAmountThb.toFixed(2),
      data.successOddBayDepositAmountUsd.toFixed(2),
    ],
    [
      'Total Success Withdraw',
      data.successWithdrawCount,
      (-Math.abs(data.successWithdrawAmountThb)).toFixed(2),
      (-Math.abs(data.successWithdrawAmountUsd)).toFixed(2),
    ],
    [
      'Total Pending Manual Allocation in XNT. Withdraw',
      data.exanteWithdrawCount,
      data.exanteWithdrawAmountThb.toFixed(2),
      data.exanteWithdrawAmountUsd.toFixed(2),
    ],
    [
      'Total Pending Manual Allocation in XNT. Deposit',
      data.exanteDepositCount,
      data.exanteDepositAmountThb.toFixed(2),
      data.exanteDepositAmountUsd.toFixed(2),
    ],
    [
      'Total Refund',
      data.refundCount,
      data.refundAmountThb.toFixed(2),
      data.refundAmountUsd.toFixed(2),
    ],
    [
      'Total Pending Execution Deposit',
      data.totalPendingDepositCount,
      data.totalPendingDepositAmountThb.toFixed(2),
      data.totalPendingDepositAmountUsd.toFixed(2),
    ],
    [
      'Total Pending Execution Withdraw',
      data.totalPendingWithdrawCount,
      (-Math.abs(data.totalPendingWithdrawAmountThb)).toFixed(2),
      (-Math.abs(data.totalPendingWithdrawAmountUsd)).toFixed(2),
    ],
    [
      'Total Net Amount Deposit',
      data.netAmountDepositCount,
      data.netAmountDepositAmountThb.toFixed(2),
      data.netAmountDepositAmountUsd.toFixed(2),
    ],
    [
      'Total Net Amount Withdraw',
      data.netAmountWithdrawCount,
      (-Math.abs(data.netAmountWithdrawAmountThb)).toFixed(2),
      (-Math.abs(data.netAmountWithdrawAmountUsd)).toFixed(2),
    ],
    [
      'Total Transactions',
      data.transactionCount,
      data.transactionAmountThb.toFixed(2),
      data.transactionAmountUsd.toFixed(2),
    ],
    [
      'Total FX Mark up Success Deposit',
      fxMarkupSummary.depositTransaction,
      formatDecimal(fxMarkupSummary.depositFxMarkupAmount),
      0,
    ],
    [
      'Total FX Mark up Success Withdraw',
      fxMarkupSummary.withdrawTransaction,
      formatDecimal(fxMarkupSummary.withdrawFxMarkupAmount),
      0,
    ],
  ];
}

function _combineData(transactions: unknown[][], summary: unknown[][]) {
  return [
    reconcileTransactionColumnHeaders,
    ...transactions,
    [],
    reconcileTransactionTotalColumnHeaders,
    ...summary,
  ];
}

function getGlobalEmailTitle(currentDate: Date): string {
  const currentDateISO = currentDate.toISOString().substring(0, 10);

  return `[${process.env.ENVIRONMENT}] - All day Global reconcile report ${currentDateISO}`;
}

function formatDecimal(number: number): string {
  if (Number.isInteger(number * 10)) {
    return number.toFixed(2);
  } else {
    return number.toString();
  }
}

const getGlobalReconcileReportDownloadDateTime = (
  dateFrom: Date,
  dateTo: Date,
  cutOffTime: number[]
): [Date, Date] => {
  const createdAtFrom = new Date(dateFrom);
  const createdAtTo = new Date(dateTo);

  createdAtFrom.setUTCDate(createdAtFrom.getUTCDate() - 1);
  createdAtFrom.setUTCHours(
    cutOffTime[0],
    cutOffTime[1],
    cutOffTime[2],
    cutOffTime[3]
  );
  createdAtTo.setUTCHours(
    cutOffTime[0],
    cutOffTime[1],
    cutOffTime[2],
    cutOffTime[3]
  );

  return [createdAtFrom, createdAtTo];
};

const getGlobalReconcileReportData = async (
  config: Config,
  createdAtFrom: Date,
  createdAtTo: Date
) => {
  const report = await getTransactionSummary(
    config.walletServiceHost,
    Product.GlobalEquities,
    createdAtFrom,
    createdAtTo
  );

  console.info(`[getGlobalReconcileReportData] Formatting transaction`);
  const reconcileTxnsList = _reformatTransactions(report.data.transactions);

  const reconcileTxnTotalList = _reformatTransactionSummary(
    report.data.transactionSummary
  );

  console.info(`[getGlobalReconcileReportData] Combining transaction data`);
  const combinedDataList = _combineData(
    reconcileTxnsList,
    reconcileTxnTotalList
  );
  const fromStr = createdAtFrom.toISOString().slice(0, 10);
  const toStr = createdAtTo.toISOString().slice(0, 10);
  const fileName = `PI_APP_GLOBAL_RECONCILE_REPORT-${fromStr}-to-${toStr}.csv`;

  console.info(`[getGlobalReconcileReportData] Converting data to buffer`);
  const csvBufferList = [await convertDataToBuffer(combinedDataList)];

  return { fileName, csvBufferList };
};

const downloadGlobalReconcileReport = async (
  event
): Promise<APIGatewayProxyResult> => {
  try {
    console.info('calling downloadGlobalReconcileReport');
    const config = await getConfig();
    const [createdAtFrom, createdAtTo] =
      getGlobalReconcileReportDownloadDateTime(
        new Date(event.body.dateFrom),
        new Date(event.body.dateTo),
        config.geCutOffTimeUtc
      );
    console.log(
      `parameters cutOffTime: ${config.geCutOffTimeUtc} dateFrom: ${createdAtFrom}, dateTo: ${createdAtTo}`
    );

    const reportData = await getGlobalReconcileReportData(
      config,
      createdAtFrom,
      createdAtTo
    );

    const combinedBuffer = Buffer.concat(reportData.csvBufferList);
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
    console.error(
      'Failed to download global reconcile report\n' + JSON.stringify(e)
    );
    throw e;
  }
};

const run = middyfy(_run);

export { run, sendGlobalReconcileReport };

export const main = middyfy(downloadGlobalReconcileReport);
