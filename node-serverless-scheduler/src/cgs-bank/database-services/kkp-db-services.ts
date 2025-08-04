import {
  MySqlKKPGenerateQrResult,
  KKPGenerateQrResultInput,
} from '@cgs-bank/models/database/generate-qr-result';
import {
  MySqlKKPQrPaymentResult,
  KKPQrPaymentResultInput,
} from '@cgs-bank/models/database/qr-payment-result';
import {
  MySqlKKPConfirmPaymentResult,
  KKPConfirmPaymentResultInput,
} from '@cgs-bank/models/database/kkp-confirm-payment-result';
import {
  MySqlKKPDDPaymentInquiryResult,
  KKPDDPaymentInquiryResultInput,
} from '@cgs-bank/models/database/kkp-dd-payment-inquiry-result';
import { Op } from 'sequelize';
import { KkpBillPaymentResponse } from '@cgs-bank/external-api-services/kkp-api/response/kkp-bill-payment-response';
import {
  KKPBillPaymentResultInput,
  MySqlKKPBillPaymentResult,
} from '@cgs-bank/models/database/kkp-bill-payment-result';

export const createKKPQRPayment = async (
  data: KKPGenerateQrResultInput
): Promise<MySqlKKPGenerateQrResult> => {
  return await MySqlKKPGenerateQrResult.create(data);
};

export const createKKPQRPaymentResult = async (
  data: KKPQrPaymentResultInput
): Promise<MySqlKKPQrPaymentResult> => {
  return await MySqlKKPQrPaymentResult.create(data);
};

export const createKKPConfirmPaymentResult = async (
  data: KKPConfirmPaymentResultInput
): Promise<MySqlKKPConfirmPaymentResult> => {
  return await MySqlKKPConfirmPaymentResult.create(data);
};

export const createKKPDDPaymentInquiryResult = async (
  data: KKPDDPaymentInquiryResultInput
): Promise<MySqlKKPDDPaymentInquiryResult> => {
  return await MySqlKKPDDPaymentInquiryResult.create(data);
};

export const findKKPGenerateQrResultByTransactionNo = async (
  transactionNo: string
): Promise<MySqlKKPGenerateQrResult> =>
  await MySqlKKPGenerateQrResult.findOne({
    where: {
      ...{ transactionNo: { [Op.eq]: transactionNo } },
    },
  });

export const createKkpBillPaymentResult = async (
  data: KKPBillPaymentResultInput
): Promise<KkpBillPaymentResponse> => {
  return await MySqlKKPBillPaymentResult.create(data);
};
