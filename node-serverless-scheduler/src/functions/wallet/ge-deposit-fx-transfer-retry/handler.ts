import {
  getFailedTransactions,
  sendDepositRetry,
} from '@functions/wallet/ge-deposit-fx-transfer-retry/api';
import { getConfigFromSsm } from '@libs/ssm-config';

async function getConfig() {
  const [walletServiceHost] = await getConfigFromSsm('wallet', [
    'wallet-srv-host',
  ]);

  return {
    walletServiceHost,
  };
}

export async function retryFailedDepositFxTransfer() {
  try {
    const config = await getConfig();
    const response = await getFailedTransactions(
      config.walletServiceHost,
      'FxTransferInsufficientBalance'
    );
    console.log(`Got ${response.data.length} transactions`);
    await Promise.all(
      response.data.map(async (transactionHistory) => {
        const txnId = transactionHistory.transactionNo;
        console.log(`Sending retry request for transaction ${txnId}`);
        try {
          await sendDepositRetry({ transactionNo: txnId });
          console.log(`Retry request for transaction ${txnId} sent`);
        } catch (e) {
          console.log(`Error occurred when sending retry request for ${txnId}`);
        }
      })
    );
  } catch (e) {
    console.error('Failed to send deposit retry\n' + JSON.stringify(e));
    throw e;
  }
}
