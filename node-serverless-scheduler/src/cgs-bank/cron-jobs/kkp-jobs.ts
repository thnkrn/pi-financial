import dbConnection from '@cgs-bank/database-services/db-connection';
import { qrInquiryProcess } from '@cgs-bank/functions/kkp/qr/payment-inquiry/handler';
import { QRPaymentInquiry } from '@cgs-bank/models/common/qr-payment';
import { createKKPQRPaymentResult } from '@cgs-bank/database-services/kkp-db-services';
import { getConfigFromSsm } from '@libs/ssm-config';

const functions = {
  start: async () => {
    console.log('----- KKP Process START -----');
    console.log(process.env.DB_HOST);
    console.log(process.env.DB_NAME);
    console.log(process.env.DB_PORT);
    console.log(process.env.DB_USER);
    console.log(process.env.DB_PASSWORD);

    await functions.recheckData();
    await functions.checkMismatchedRecord();

    console.log('----- KKP Process END -----');
  },

  recheckData: async () => {
    console.log('----- KKP Start Recheck -----');
    try {
      const queryResult = await dbConnection.query(`SELECT transaction_no
                                                  FROM kkp_qr_generated_result
                                                  WHERE kkp_qr_generated_result.transaction_no NOT IN
                                                        (SELECT biller_reference_no FROM kkp_qr_payment_result)`);
      const missingTransactions = queryResult[0];
      for (const record of missingTransactions) {
        const input: QRPaymentInquiry = {
          transactionNo: record['transaction_no'],
        };
        const [kkpBillerId] = await getConfigFromSsm('cgs/bank-services/kkp', [
          'biller-id',
        ]);
        const response = await qrInquiryProcess(input, kkpBillerId);

        const inquiryResponseItem = response.Data.ResultList[0];
        console.log(inquiryResponseItem);

        await createKKPQRPaymentResult({
          BillerId: '',
          AccountBank: '',
          AccountNo: '',
          BillerReferenceNo: input.transactionNo,
          PaymentAmount: inquiryResponseItem.PaymentAmount.toString(),
          PaymentDate: inquiryResponseItem.PaymentDate,
          PaymentType: inquiryResponseItem.PaymentType,
          CustomerName: inquiryResponseItem.CustomerName,
        });
      }
    } catch (error) {
      console.log(error);
    }

    console.log('----- KKP End Recheck -----');
  },

  checkMismatchedRecord: async () => {
    console.log('----- KKP Start Check Mismatched -----');
    try {
      const queryResult = await dbConnection.query(`
          SELECT transaction_no, transaction_amount, qr_result.payment_amount
          FROM kkp_qr_generated_result AS qr_generate
                   INNER JOIN (SELECT biller_reference_no, payment_amount FROM kkp_qr_payment_result) AS qr_result
                              ON qr_generate.transaction_no = qr_result.biller_reference_no
          WHERE qr_generate.transaction_amount <> qr_result.payment_amount`);
      const mismatchedTransactions = queryResult[0];
      for (const record of mismatchedTransactions) {
        // @TODO CREATE MISMATCH RECORD REPORT
        console.log(record);
      }
    } catch (error) {
      console.log(error);
    }

    console.log('----- KKP End Check Mismatched -----');
  },
};

export default functions;
