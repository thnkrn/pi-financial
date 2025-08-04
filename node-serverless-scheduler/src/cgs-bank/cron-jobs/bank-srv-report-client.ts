import dbConnection from '@cgs-bank/database-services/db-connection';
import { MySqlKKPConfirmPaymentResult } from '@cgs-bank/models/database/kkp-confirm-payment-result';
import { getTimestamp, TimestampFormat } from '@cgs-bank/utils/timestamp';
import dayjs from 'dayjs';
import { Op } from 'sequelize';

export interface ReconcileRecordInterface {
  createdTime: string;
  lastUpdateTime: string;
  transactionType: string;
  channel: string;
  fromBank: string;
  bankCode: string; // external_reference
  fromBankAccount: string;
  fromBankAccountName: string;
  fromAccountType: string;
  fromCustomerCode: string;
  toBank: string;
  toBankAccountNo: string;
  toAccountType: string;
  toCustomerCode: string;
  transactionDateTime: string;
  transactionNo: string;
  transactionRef: string;
  amount: number;
  fee: number;
}

const functions = {
  checkEnvironment: () => {
    let isCompleted = true;
    const requireKeys = [
      'DB_HOST',
      'DB_NAME',
      'DB_PORT',
      'DB_USER',
      'DB_PASSWORD',
    ];
    for (const key of requireKeys) {
      if (process.env[key] == undefined) {
        console.log(`Missing value of ${key}`);
        isCompleted = false;
      } else {
        console.log(`${key} = ${process.env[key]}`);
      }
    }

    if (!isCompleted) {
      throw 'Environment is not set';
    } else {
      console.log('All required environment has set.');
    }
  },

  generateDepositReport: async (
    startDateTime: string,
    endDateTime: string,
    productList: string[]
  ): Promise<ReconcileRecordInterface[]> => {
    try {
      const sql = `SELECT *
                   from kkp_qr_generated_result AS qr_generate
                            INNER JOIN (SELECT *
                                        from kkp_qr_payment_result
                                        WHERE payment_date BETWEEN '${startDateTime}' and '${endDateTime}') AS qr_result
                                       ON qr_generate.transaction_no = qr_result.biller_reference_no
                   WHERE product IN (${productList.map(
                     (product) => `'${product}'`
                   )})`;
      const result = await dbConnection.query(sql);
      return result[0].map<ReconcileRecordInterface>((item) => {
        const renderedItem: ReconcileRecordInterface = {
          amount: item['payment_amount'],
          bankCode: '',
          channel: 'QR',
          createdTime: getTimestamp(
            TimestampFormat.Report,
            dayjs(item['created_at'])
          ),
          fee: 0,
          fromAccountType: '',
          fromBank: item['account_bank'],
          fromBankAccount: item['account_no'],
          fromBankAccountName: item['customer_name'],
          fromCustomerCode: '',
          lastUpdateTime: getTimestamp(
            TimestampFormat.Report,
            dayjs(item['updated_at'])
          ),
          toAccountType: item['product'],
          toBank: '069', // FIX ACCOUNT
          toBankAccountNo: '2003795907', // FIX ACCOUNT
          toCustomerCode: item['customer_code'],
          transactionDateTime: item['transaction_date_time'],
          transactionNo: item['transaction_no'],
          transactionRef: item['transaction_ref_code'],
          transactionType: 'DEPOSIT',
        };
        return renderedItem;
      });
    } catch (error) {
      console.log(error);
      throw error;
    }
  },
  generateWithdrawReport: async (
    startDateTime: string,
    endDateTime: string,
    productList: string[]
  ): Promise<ReconcileRecordInterface[]> => {
    try {
      const result = await MySqlKKPConfirmPaymentResult.findAll({
        where: {
          TransactionDateTime: {
            [Op.between]: [startDateTime, endDateTime],
          },
          product: {
            [Op.in]: productList,
          },
        },
      });

      return result.map((item) => {
        const renderedItem: ReconcileRecordInterface = {
          amount: item.amount,
          bankCode: item.txnReferenceNo,
          channel: 'SINGLE_TRANSFER',
          createdTime: getTimestamp(
            TimestampFormat.Report,
            dayjs(item.createdAt)
          ),
          fee: item.FeeAmount,
          fromAccountType: item.product,
          fromBank: '069', // FIX ACCOUNT
          fromBankAccount: '2003795907', // FIX ACCOUNT
          fromBankAccountName: '',
          fromCustomerCode: item.customerCode,
          lastUpdateTime: getTimestamp(
            TimestampFormat.Report,
            dayjs(item.updatedAt)
          ),
          toAccountType: '',
          toBank: item.destinationBankCode,
          toBankAccountNo: item.accountNo,
          toCustomerCode: '',
          transactionDateTime: item.TransactionDateTime,
          transactionNo: item.transactionNo,
          transactionRef: item.transactionRefCode,
          transactionType: 'WITHDRAW',
        };
        return renderedItem;
      });
    } catch (error) {
      console.log(error);
      throw error;
    }
  },
};

export default functions;
