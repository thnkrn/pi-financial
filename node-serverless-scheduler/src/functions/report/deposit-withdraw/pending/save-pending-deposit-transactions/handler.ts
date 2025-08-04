import { ResponseCodes, getResponseCodes } from '@libs/backoffice-api';
import { encrypt } from '@libs/crypto-utils';
import { getmySqlClient } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import { getConfigFromSsm } from '@libs/ssm-config';
import { getDepositTransactionsByDateType } from '@libs/wallet-api';
import { v4 as guid } from 'uuid';

async function getWalletConfig() {
  const [walletServiceHost] = await getConfigFromSsm('report', [
    'wallet-srv-host',
  ]);

  return {
    walletServiceHost,
  };
}

async function getBackOfficeConfig() {
  const [backOfficeServiceHost] = await getConfigFromSsm('report', [
    'backoffice-srv-host',
  ]);

  return {
    backOfficeServiceHost,
  };
}

const findDescriptionByResponseCode = (
  responseCodes: ResponseCodes[],
  currentState: string
): string | null => {
  const responseCode = responseCodes.find(
    (responseCode) =>
      responseCode.state.toLowerCase() === currentState.toLowerCase()
  );
  return responseCode ? responseCode.description : null;
};

const snapshotPendingDepositTransactions = async () => {
  console.info(
    `Getting Pending Deposit Transactions at ${new Date().toISOString()}`
  );

  const mysql = await getmySqlClient({
    parameterName: 'report',
    dbHost: 'backoffice-db-host',
    dbPassword: 'backoffice-db-password',
    dbUsername: 'backoffice-db-username',
    dbName: 'report_db',
  });

  try {
    const walletConfig = await getWalletConfig();
    const backOfficeConfig = await getBackOfficeConfig();

    // Need to fetch snapshot of past 24 hours according to payment received date time
    const currentDateTime = new Date();
    const previousDateTime = new Date(
      currentDateTime.getTime() - 24 * 60 * 60 * 1000
    );

    const depositTransactionsForThaiEquity =
      await getDepositTransactionsByDateType(
        'PaymentReceived',
        walletConfig.walletServiceHost,
        previousDateTime,
        currentDateTime,
        'Pending',
        'ThaiEquity'
      );

    const depositTransactionsForGlobalEquity =
      await getDepositTransactionsByDateType(
        'PaymentReceived',
        walletConfig.walletServiceHost,
        previousDateTime,
        currentDateTime,
        'Pending',
        'GlobalEquity'
      );

    const depositTransactions = [
      ...depositTransactionsForThaiEquity,
      ...depositTransactionsForGlobalEquity,
    ];

    console.log('DepositTransaction Length', depositTransactions.length);

    const responseCodes = await getResponseCodes(
      backOfficeConfig.backOfficeServiceHost,
      'deposit'
    );

    for (const depositTransaction of depositTransactions) {
      const data = {
        id: guid(),
        payment_received_datetime: depositTransaction.paymentReceivedDateTime,
        sender_account: depositTransaction.bankAccountNo,
        bank_code: depositTransaction.bankCode,
        sender_channel: depositTransaction.channel,
        qr_transaction_no: depositTransaction.qrTransactionNo,
        payment_received_amount: depositTransaction.amount,
        transaction_number: depositTransaction.transactionNo,
        customer_bank_name: await encrypt(depositTransaction.bankAccountName),
        latest_response: findDescriptionByResponseCode(
          responseCodes.data,
          depositTransaction.currentState
        ),
        created_at: new Date(),
      };
      await mysql.query('INSERT INTO pending_deposit_snapshot SET ?', data);
    }
  } catch (e) {
    console.error(
      'Failed to snapshot deposit transaction\n',
      +JSON.stringify(e)
    );
    throw e;
  } finally {
    await mysql.end();
  }
};

export const main = middyfy(snapshotPendingDepositTransactions);
