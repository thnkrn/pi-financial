import { getTimestamp, TimestampFormat } from '@cgs-bank/utils/timestamp';
import * as soap from 'soap';
import { getConfigFromSsm } from '@libs/ssm-config';

const encoding = 'UTF8';

export interface KBANKPaymentInput {
  submitterLoginName?: string;
  transactionNo: string;
  payeeShortName?: string;
  payerShortName?: string;
  accountNumber: string;
  amount: number;
  ref1?: string;
  ref2?: string;
  ref3?: string;
  ref4?: string;
}

export interface KBANKPaymentResponse {
  externalRef: string;
  oddTranNo?: string;
  tranTimestamp: string;
  payeeShortName: string;
  payerShortName: string;
  payerBankAccount?: string;
  amount: string;
  returnStatus: string;
  returnCode: string;
  returnMessage: string;
  ref1?: string;
  ref2?: string;
  ref3?: string;
  ref4?: string;
}

function getPaymentStr(input: KBANKPaymentInput): Array<string> {
  const header = `00100${process.env.ExternalSystemShortName.padEnd(12)}
      ${input.submitterLoginName?.padEnd(50) ?? ''.padEnd(50)}
      ${'th_TH'.padEnd(15)}
      0001`;
  const record = `1
  ${input.transactionNo.substring(0, 20).padEnd(20)}
  ${input.payeeShortName.padEnd(12)}
  ${input.payerShortName.padEnd(30)}
  R
  ${input.accountNumber.padEnd(20)}
  ${
    input.amount.toString().split('.')[0].padStart(13, '0') +
    input.amount.toString().split('.')[1].padEnd(2, '0')
  }
  ${getTimestamp(TimestampFormat.Normal)}
  ${getTimestamp(TimestampFormat.OnlyDate)}`;

  const detail = `2
  ${input.ref1?.padEnd(20) ?? ''.padEnd(20)}
  ${input.ref2?.padEnd(20) ?? ''.padEnd(20)}
  ${input.ref3?.padEnd(20) ?? ''.padEnd(20)}
  ${input.ref4?.padEnd(20) ?? ''.padEnd(20)}
  `;
  const trailing = '91';

  return [header, record, detail, trailing];
}

export interface KBANKPaymentInquiryResponse {
  oddTransactionNo: string;
  oddTransactionDatetime: string;
  accountNumber: string;
  amount: string;
  returnCode: string;
  returnMessage: string;
}

const functions = {
  async payment(input: KBANKPaymentInput) {
    const [baseUrl, externalSystemShortName] = await getConfigFromSsm(
      'cgs/bank-services/kbank',
      ['base-api', 'external-system-short-name']
    );
    const args = {
      sz_Pay: getPaymentStr(input),
      sz_ExsysShortName: externalSystemShortName,
      sz_LanguageEncoding: encoding,
      sz_Locale: 'th_TH',
    };

    return new Promise<KBANKPaymentResponse>((resolve, reject) => {
      soap.createClient(
        `${baseUrl}/services/EBXPGSSOv1?wsdl}`,
        (err, kResApi) => {
          if (err) reject(err);
          kResApi.sendSSO(args, (err, kRes) => {
            if (err) reject(err);
            const payRes = kRes.sendSSOResponse;
            const response: KBANKPaymentResponse = {
              externalRef: payRes[1].substring(2, 21),
              oddTranNo: payRes[1].substring(22, 41),
              tranTimestamp: payRes[1].substring(42, 55),
              payeeShortName: payRes[1].substring(56, 67),
              payerShortName: payRes[1].substring(68, 97),
              payerBankAccount: payRes[1].substring(98, 107),
              amount: payRes[1].substring(108, 122),
              returnStatus: payRes[1].substring(123, 123),
              returnCode: payRes[1].substring(124, 126),
              returnMessage:
                payRes[1].substring(124, 126) == '000'
                  ? 'Success'
                  : payRes[1].substring(127, 226),
              ref1: payRes[2].substring(2, 101),
              ref2: payRes[2].substring(102, 201),
              ref3: payRes[2].substring(202, 301),
              ref4: payRes[2].substring(302, 401),
            };
            resolve(response);
          });
        }
      );
    });
  },

  async paymentInquiry(tranNo: string) {
    const [baseUrl] = await getConfigFromSsm('cgs/bank-services/kbank', [
      'base-api',
    ]);
    const args = {
      extSysReferenceNo: tranNo,
      extSysShortName: process.env.ExternalSystemShortName,
      laguageEncoding: process.env.Encoding,
      locale: 'th_TH',
    };

    return new Promise<KBANKPaymentInquiryResponse>((resolve, reject) => {
      soap.createClient(
        `${baseUrl}/services/EBXPGPaymentStatusv1?wsdl`,
        (err, kResApi) => {
          if (err) reject(err);
          kResApi.sendSSO(args, (err, kRes) => {
            if (err) reject(err);
            const payRes = kRes.sendSSOResponse;
            const response: KBANKPaymentInquiryResponse = {
              oddTransactionNo: payRes[1].substring(22, 41),
              oddTransactionDatetime: payRes[1].substring(42, 61),
              accountNumber: payRes[1].substring(98, 107),
              amount: payRes[1].substring(108, 122),
              returnCode: payRes[1].substring(124, 126),
              returnMessage: payRes[1].substring(127, 226),
            };
            resolve(response);
          });
        }
      );
    });
  },
};

export default functions;
