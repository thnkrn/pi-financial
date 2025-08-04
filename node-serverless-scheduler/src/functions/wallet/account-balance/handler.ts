import {
  getTransactionSummary,
  Product,
  Transaction,
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
} from '@libs/date-utils';
import { convertFilesToAttachments, sendEmailToSES } from '@libs/email-utils';
import { middyfy } from '@libs/lambda';
import { storeFileToS3 } from '@libs/s3-utils';
import { getConfigFromSsm } from '@libs/ssm-config';
import { getTimeFromString } from '@libs/time-utils';
import console from 'console';

const transactionHeaders = [
  'trans_id',
  'account',
  'src_ccy',
  'src_amt',
  'src_fee',
  'des_ccy',
  'des_amt',
  'des_fee',
  'fxrate',
  'paytype',
  'remark',
];

const typeDeposit = 'Deposit';
const typeWithdraw = 'Withdraw';

async function getConfig() {
  const [
    walletServiceHost,
    sbaRecipient,
    sbaBucket,
    geCutOffTimeUtc,
    daylightSavingStartString,
    daylightSavingEndString,
  ] = await getConfigFromSsm('wallet', [
    'wallet-srv-host',
    'sba-recipient',
    'sba-bucket',
    'ge-cutoff-time-utc',
    'daylight-saving-start-datetime',
    'daylight-saving-end-datetime',
  ]);

  return {
    walletServiceHost,
    sbaRecipient,
    sbaBucket,
    geCutOffTimeUtc,
    daylightSavingStartString,
    daylightSavingEndString,
  };
}

const _run: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  const date = new Date(event.body.requestDate);

  await _handle(date);

  return formatJSONResponse({
    status: 'done',
  });
};

const updateFreewillAccountBalance = async () => {
  const currentDate = new Date();
  await _handle(currentDate);
};

const _handle = async (date: Date) => {
  const config = await getConfig();
  const [previousDate, currentDate] = getDateTimeFromAndDateTimeTo(
    date,
    config.geCutOffTimeUtc,
    config.daylightSavingStartString,
    config.daylightSavingEndString
  );
  console.log(
    `Getting data for Date ${date.toDateString()} from ${previousDate.toISOString()} to ${currentDate.toISOString()}`
  );

  const resp = await getTransactionSummary(
    config.walletServiceHost,
    Product.GlobalEquities,
    previousDate,
    currentDate
  );
  const withdrawTransactions = resp.data.transactions.filter(
    (t) => t.transactionType == typeWithdraw && t.status == 'Success'
  );
  const depositTransactions = resp.data.transactions.filter(
    (t) =>
      t.transactionType == typeDeposit &&
      (t.status == 'Success' ||
        (t.status == 'Pending' &&
          (t.currentState == 'FxTransferFailed' ||
            t.currentState == 'FxTransferInsufficientBalance')))
  );

  const combinedWithdrawData = [
    transactionHeaders,
    ..._formatData(withdrawTransactions, typeWithdraw),
  ];
  const combinedDepositData = [
    transactionHeaders,
    ..._formatData(depositTransactions, typeDeposit),
  ];

  const withdrawCsvBuffer = await convertDataToBuffer(combinedWithdrawData);
  const depositCsvBuffer = await convertDataToBuffer(combinedDepositData);

  const dateStr = date.toISOString().substring(0, 10);
  const formattedDate = String(dateStr).replace(/\//g, '');

  const withdrawFileName = `XM_Withdraw_FCcash_${formattedDate}.csv`;
  const depositFileName = `XM_Deposit_FCcash_${formattedDate}.csv`;

  const bucket = config.sbaBucket;
  await storeFileToS3(bucket, withdrawCsvBuffer, withdrawFileName);
  await storeFileToS3(bucket, depositCsvBuffer, depositFileName);

  await sendEmail(
    dateStr,
    config.sbaRecipient,
    [withdrawFileName, depositFileName],
    [withdrawCsvBuffer, depositCsvBuffer]
  );
};

async function sendEmail(
  dateStr: string,
  recipient: string,
  fileNames: string[],
  fileContents: Buffer[]
) {
  const emailSubject = `[${process.env.ENVIRONMENT}] - Global SBA Report ${dateStr}`;
  const attachments = convertFilesToAttachments(fileNames, fileContents);
  await sendEmailToSES(recipient, emailSubject, attachments);
}

function getDateTimeFromAndDateTimeTo(
  date: Date,
  cutOffTimeString: string,
  daylightSavingStartDateString: string,
  daylightSavingEndDateString: string
): Date[] {
  // report to query from 5am (UTC+7) - 5am (UTC+7) of the next day
  const previousDate = new Date(date);
  const cutOffTime = getTimeFromString(cutOffTimeString);
  const fromTime = [...cutOffTime];
  const toTime = [...cutOffTime];
  const daylightSavingStartDate = new Date(daylightSavingStartDateString);
  const daylightSavingEndDate = new Date(daylightSavingEndDateString);

  // add date offset when daylight saving hours is later than cutoff time
  if (
    isTimeGreaterThan(daylightSavingStartDate, cutOffTime[0], cutOffTime[1])
  ) {
    daylightSavingStartDate.setDate(daylightSavingStartDate.getDate() + 1);
  }
  if (isTimeGreaterThan(daylightSavingEndDate, cutOffTime[0], cutOffTime[1])) {
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

  previousDate.setDate(previousDate.getDate() - 1);
  previousDate.setHours(fromTime[0], fromTime[1], fromTime[2], fromTime[3]);
  date.setHours(toTime[0], toTime[1], toTime[2], toTime[3]);

  return [previousDate, date];
}

function _formatData(transactions: Transaction[], type: string) {
  switch (type) {
    case typeWithdraw: {
      return transactions.map((t) => [
        t.transactionNo,
        formatAccountNo(t.accountCode),
        'USD',
        t.requestedAmount,
        t.globalTransfer?.transferFee?.toFixed(2) ?? '0.00',
        'THB',
        t.transferAmount,
        t.fee ?? '0.00',
        t.globalTransfer.requestedFxRate,
        mapPayTypeCode(t.channel),
        '',
      ]);
    }
    case typeDeposit: {
      return transactions.map((t) => [
        t.transactionNo,
        formatAccountNo(t.accountCode),
        'THB',
        t.requestedAmount,
        t.fee ?? '0.00',
        'USD',
        t.transferAmount,
        t.globalTransfer?.transferFee?.toFixed(2) ?? '0.00',
        t.globalTransfer.fxMarkUpRate > 0
          ? t.globalTransfer.requestedFxRate
          : t.globalTransfer.fxConfirmedExchangeRate,
        mapPayTypeCode(t.channel),
        '',
      ]);
    }
  }
}

function formatAccountNo(accountNo: string): string {
  const lastIndex = accountNo.length - 1;
  return accountNo.slice(0, lastIndex) + '-' + accountNo.slice(lastIndex);
}

function mapPayTypeCode(channel: string): string {
  switch (channel) {
    case 'SetTrade':
      return '01';
    case 'ATS':
      return '02';
    case 'QR':
    case 'ODD':
    case 'OnlineViaKKP':
      return '18';
    case 'TransferApp':
      return '17';
    default:
      return '';
  }
}

const run = middyfy(_run);

export { run, updateFreewillAccountBalance };
