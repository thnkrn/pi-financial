import { getConfigFromSsm } from '@libs/ssm-config';

import { formatJSONResponse } from '@libs/api-gateway';
import { middyfy } from '@libs/lambda';

import { decrypt } from '@libs/crypto-utils';
import { convertDataToBuffer, reportHeaders } from '@libs/csv-utils';
import { formatDate } from '@libs/date-utils';
import { getmySqlClient } from '@libs/db-utils';
import { storeFileToS3 } from '@libs/s3-utils';
import {
  getDepositTransactionsByDateType,
  getRefundTransactions,
} from '@libs/wallet-api';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import { ReportStatus } from '../../../../../constants/report';

dayjs.extend(utc);

const transactionDataHeaders = [
  'No',
  'Deposit Transaction Date',
  'Deposit Transaction Time',
  'Sender Bank Account Number',
  'Sender Bank Code',
  'Sender Channel',
  'Deposit Transaction Number',
  'Payment Received Amount',
  'Latest Response',
  'Sender Bank Account Name',
  'Post Action Transaction Date',
  'Post Action Transaction Time',
  'Reciever Channel',
  'Receiver Bank Account No',
  'Receiver Bank',
  'Transaction Number',
  'Customer Account Code',
  'Product',
  'Transaction Type',
  'Currency',
  'Amount',
];

interface DepositSnapshot {
  paymentReceivedDateTime: string;
  bankAccountNo: string;
  bankCode: string;
  channel: string;
  qrTransactionNo: string;
  paymentReceivedAmount: number;
  transactionNo: string;
  bankAccountName: string;
  latestResponse: string;
}

interface ReconciledTransaction {
  effectiveDateTime: string;
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
}

interface Result {
  depositTransaction: DepositSnapshot;
  reconciledTransaction: ReconciledTransaction;
}

async function getWalletConfig() {
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

const fetchReportDataAndUploadToS3 = async (event) => {
  const request = JSON.parse(event.body) as RequestData;
  const dateFrom = new Date(request.dateFrom);

  // Suppose, if dateFrom is 5th Ncv. Trigger time would be 5.00 PM BKK Time.
  // We have fetch snapshot data from pending_deposit_snapshot table where deleted_at is NULL
  // Snapshots are saved according to BKK TimeZone but in UTC Format.
  // So we convert to UTC. They will be reconciled from 4th Nov 5.00 PM to 5th Nov requested time (which is 5 PM).
  const reconcileDateFrom = new Date(dateFrom.getTime() - 24 * 60 * 60 * 1000);
  reconcileDateFrom.setHours(10, 0, 0, 0); // 5.00 PM BKK Time
  const reconcileDateTo = new Date();

  console.log('reconcileDateTimeFrom', reconcileDateFrom);
  console.log('reconcileDateTimeTo', reconcileDateTo);

  console.info('Fetching Pending Deposit Snapshot Data');

  const mysql = await getmySqlClient({
    parameterName: 'report',
    dbHost: 'backoffice-db-host',
    dbPassword: 'backoffice-db-password',
    dbUsername: 'backoffice-db-username',
    dbName: 'report_db',
  });
  try {
    const walletConfig = await getWalletConfig();
    const queryResults = await mysql.query<unknown[]>(
      'SELECT id, payment_received_datetime, sender_account, payment_received_amount, latest_response, bank_code, sender_channel, qr_transaction_no, transaction_number, customer_bank_name FROM pending_deposit_snapshot WHERE deleted_at IS NULL'
    );

    const pendingDepositTransactions = queryResults.map((result: any) => ({
      id: result.id,
      paymentReceivedDateTime: result.payment_received_datetime,
      bankAccountNo: result.sender_account,
      bankCode: result.bank_code,
      channel: result.sender_channel,
      paymentReceivedAmount: result.payment_received_amount,
      qrTransactionNo: result.qr_transaction_no,
      transactionNo: result.transaction_number,
      bankAccountName: result.customer_bank_name,
      latestResponse: result.latest_response,
    }));

    console.log(
      'pendingDepositTransactions.length',
      pendingDepositTransactions.length
    );
    const depositTransactionsByEffectiveDateForThaiEquity =
      await getDepositTransactionsByDateType(
        'EffectiveDate',
        walletConfig.walletServiceHost,
        reconcileDateFrom,
        reconcileDateTo,
        'Success',
        'ThaiEquity'
      );

    const depositTransactionsByEffectiveDateForGlobalEquity =
      await getDepositTransactionsByDateType(
        'EffectiveDate',
        walletConfig.walletServiceHost,
        reconcileDateFrom,
        reconcileDateTo,
        'Success',
        'GlobalEquity'
      );

    const depositTransactionsByEffectiveDate = [
      ...depositTransactionsByEffectiveDateForThaiEquity,
      ...depositTransactionsByEffectiveDateForGlobalEquity,
    ];

    const refundTransactions = await getRefundTransactions(
      walletConfig.walletServiceHost,
      reconcileDateFrom,
      reconcileDateTo
    );

    const result: Result[] = [];
    let isTransactionMatched: boolean;
    for (const depositTransaction of pendingDepositTransactions) {
      isTransactionMatched = false;
      const matchingDepositTransaction =
        depositTransactionsByEffectiveDate.find(
          (depositTransactionByEffectiveDate) =>
            depositTransactionByEffectiveDate.transactionNo ===
            depositTransaction.transactionNo
        );

      if (matchingDepositTransaction) {
        result.push({
          depositTransaction: {
            ...depositTransaction,
            bankAccountName: await decrypt(depositTransaction.bankAccountName),
          },
          reconciledTransaction: {
            effectiveDateTime: matchingDepositTransaction.effectiveDateTime,
            // Need to display these 3 fields only in case of Refund
            channel: '-',
            bankAccountNo: '-',
            bankCode: '-',
            transactionNo: matchingDepositTransaction.transactionNo,
            accountCode: matchingDepositTransaction.accountCode,
            customerCode: matchingDepositTransaction.customerCode,
            product: matchingDepositTransaction.product,
            transactionType: 'Deposit Completed',
            currency: matchingDepositTransaction.currency,
            amount: matchingDepositTransaction.amount,
          },
        });

        isTransactionMatched = true;
      }

      if (!isTransactionMatched) {
        const matchingRefundTransaction = refundTransactions.find(
          (refundTransaction) =>
            refundTransaction.depositTransactionNo ===
            depositTransaction.transactionNo
        );

        if (matchingRefundTransaction) {
          result.push({
            depositTransaction: {
              ...depositTransaction,
              bankAccountName: await decrypt(
                depositTransaction.bankAccountName
              ),
            },
            reconciledTransaction: {
              effectiveDateTime: matchingRefundTransaction.effectiveDateTime,
              channel: matchingRefundTransaction.channel,
              bankAccountNo: matchingRefundTransaction.bankAccountNo,
              bankCode: matchingRefundTransaction.bankCode,
              transactionNo: matchingRefundTransaction.depositTransactionNo,
              accountCode: matchingRefundTransaction.accountCode,
              customerCode: matchingRefundTransaction.customerCode,
              product: matchingRefundTransaction.product,
              transactionType: 'Refund (Success)',
              currency: matchingRefundTransaction.currency,
              amount: matchingRefundTransaction.amount,
            },
          });

          isTransactionMatched = true;
        }
      }

      if (isTransactionMatched) {
        await mysql.query(
          'UPDATE pending_deposit_snapshot SET deleted_at=? WHERE id=?',
          [new Date(), depositTransaction.id]
        );
      } else {
        // Put all pending transactions in the report
        result.push({
          depositTransaction: {
            ...depositTransaction,
            bankAccountName: await decrypt(depositTransaction.bankAccountName),
          },
          reconciledTransaction: {
            effectiveDateTime: '-',
            channel: '-',
            bankAccountNo: '-',
            bankCode: '-',
            transactionNo: '-',
            accountCode: '-',
            customerCode: '-',
            product: '-',
            transactionType: '-',
            currency: '-',
            amount: 0,
          },
        });
      }
    }

    console.log('result.length', result.length);
    const index = 1;
    const transactionsCsvBuffer = await convertDataToBuffer([
      ...reportHeaders(
        request.id,
        'Pending Transaction Report',
        'Daily Bank Reconcile Report'
      ),
      [],
      transactionDataHeaders,
      ..._formatResultData(
        index,
        result.sort((r1, r2) =>
          r1.depositTransaction.paymentReceivedDateTime >
          r2.depositTransaction.paymentReceivedDateTime
            ? 1
            : -1
        )
      ),
    ]);

    const s3fileKey = getS3KeyForReport('dw-pending-reconcile/pending_all');

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
  } finally {
    await mysql.end();
  }
};

function _formatResultData(index: number, transactions: Result[]) {
  return transactions.map((t) => [
    index++,
    formatDate(t.depositTransaction.paymentReceivedDateTime).split(' ')[0],
    formatDate(t.depositTransaction.paymentReceivedDateTime).split(' ')[1],
    t.depositTransaction.bankAccountNo,
    t.depositTransaction.bankCode,
    t.depositTransaction.channel,
    t.depositTransaction.qrTransactionNo,
    Math.abs(t.depositTransaction.paymentReceivedAmount),
    t.depositTransaction.latestResponse,
    t.depositTransaction.bankAccountName,
    formatDate(t.reconciledTransaction.effectiveDateTime).split(' ')[0],
    formatDate(t.reconciledTransaction.effectiveDateTime).split(' ')[1],
    t.reconciledTransaction.channel,
    t.reconciledTransaction.bankAccountNo,
    t.reconciledTransaction.bankCode,
    t.reconciledTransaction.transactionNo,
    t.reconciledTransaction.accountCode,
    t.reconciledTransaction.product,
    t.reconciledTransaction.transactionType,
    t.reconciledTransaction.currency,
    Math.abs(t.reconciledTransaction.amount),
  ]);
}

const getS3KeyForReport = (prefix: string): string => {
  const currentTime = new Date();
  // Time from pending snapshot lambda trigger to report execution time
  const dateFrom = new Date(currentTime.getTime() - 24 * 60 * 60 * 1000);
  const formatDateFrom = dayjs(dateFrom.setHours(17, 0, 1)).format(
    'YYYY-MM-DDTHH:mm:ss'
  );

  const formatDateTo = dayjs(currentTime.setHours(17, 0, 1)).format(
    'YYYY-MM-DDTHH:mm:ss'
  );

  return `${prefix}_${formatDateFrom}--${formatDateTo}.csv`;
};

export const main = middyfy(fetchReportDataAndUploadToS3);
