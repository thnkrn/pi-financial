import { formatDateTimeUTC } from '@libs/date-utils';
import console from 'console';
import got from 'got';

export enum TransferType {
  Debit,
  Credit,
  Qr,
}

export enum TransferVia {
  Finnet,
  CgsBank,
}

export type TransferHistoryRequest = {
  createdAtFrom?: Date;
  createdAtTo?: Date;
  transactionNo?: string;
  bankCode?: string;
  success?: boolean;
  transferVia?: TransferVia;
  transferType?: TransferType;
};

export type TransferHistory = {
  bankReferenceNo?: string;
  transactionNo?: string;
  transferType?: TransferType;
  transferVia?: TransferVia;
  transferAmount?: string;
  customerBankCode?: string;
  customerBankAccountNo?: string;
  customerBankAccountName?: string;
  product?: string;
  success?: boolean;
  completedAt?: Date;
};

export type TransferHistoryResponse = {
  data: TransferHistory[];
};

const timeoutConfig = { request: Number(process.env.API_TIMEOUT || 100000) };

export async function getTransferHistory(
  paymentServiceHost: string,
  request: TransferHistoryRequest
) {
  const url = new URL('internal/transfer-history', paymentServiceHost);
  Object.keys(request).forEach((key) => {
    if (request[key] instanceof Date) {
      url.searchParams.append(key, formatDateTimeUTC(request[key]));
    } else if (request[key] !== undefined) {
      url.searchParams.append(key, request[key].toString());
    }
  });

  return got
    .get(url, {
      timeout: timeoutConfig,
      hooks: {
        beforeRequest: [
          (request) => console.info(`HTTP Requesting ${request.url}`),
        ],
      },
    })
    .json<TransferHistoryResponse>()
    .catch((e) => {
      console.error(`Failed to get transfer history. Exception: ${e}`);
      throw e;
    });
}
