import { BankServiceClient } from '@libs/bank-service/bank-service-client';
import {
  GenerateQrPropose,
  GenerateQrRequest,
} from '@libs/bank-service/model/generate-qr-request';
import { Injectable, Logger } from '@nestjs/common';
import {
  Product,
  PurchaseRequest,
  PurchaseRequestStatus,
} from '@prisma/client';
import { nowBangkok } from '@utils/timestamp';
import { PrismaService } from '../prisma.service';
import { PaymentNotification } from './sqs-message/payment-notification-message.dto';

@Injectable()
export class PurchaseService {
  private readonly logger = new Logger(PurchaseService.name);
  private readonly bankClient = new BankServiceClient();
  constructor(private readonly prismaService: PrismaService) {}

  async getPurchaseRequest(id: string) {
    try {
      return this.prismaService.purchaseRequest.findUnique({
        where: {
          id: Number(id),
        },
        select: {
          status: true,
        },
      });
    } catch (e) {
      this.logger.log(e);
      throw 'Cannot get purchase request';
    }
  }

  async getPendingRequest(customerCode: string, planId: number) {
    try {
      return this.prismaService.purchaseRequest.findFirst({
        where: {
          customerCode: customerCode,
          planId: planId,
          status: 'GENERATED_PAYMENT',
          paymentGenerated: {
            qrExpirationTime: {
              gt: nowBangkok().toDate(),
            },
          },
        },
        include: {
          paymentGenerated: true,
        },
      });
    } catch (e) {
      this.logger.log(e);
      throw 'Cannot get pending request';
    }
  }
  async generateNewMT5PurchaseRequest(
    customerCode: string,
    planId: number,
    amount: number,
  ) {
    try {
      const prefixTransactionNo = `${
        process.env.TRANSACTION_PREFIX
      }-${nowBangkok().format('YYYYMMDD')}`;
      const newId = await this.prismaService.purchaseRequest
        .count({
          where: {
            referenceCode: {
              startsWith: prefixTransactionNo,
            },
          },
        })
        .then((result) => `${result + 1}`.padStart(6, '0'));
      const newRefCode = `${prefixTransactionNo}-${newId}`;
      return this.prismaService.purchaseRequest.create({
        data: {
          product: 'mt5',
          referenceCode: newRefCode,
          customerCode: customerCode,
          planId: planId,
          amount: amount,
        },
      });
    } catch (e) {
      this.logger.log(e);
      throw 'Cannot create new MT5 request';
    }
  }

  async generateNewQrPayment(record: PurchaseRequest, product: Product) {
    try {
      const now = nowBangkok();
      const qrPaymentExpiredTime = 15;

      const apiToken = await this.bankClient.getToken();
      const request: GenerateQrRequest = {
        transactionNo: `MT5QR${now.format('YYYYMMDDHHmmss')}`,
        expiredTimeInMinute: qrPaymentExpiredTime,
        amount: record.amount,
        transactionRefCode: record.referenceCode.split('-').join(''),
        internalRef: {
          customerCode: record.customerCode,
          product: product,
        },
        propose: GenerateQrPropose.PAYMENT,
      };
      this.logger.log(`generateQrPayment Request ${JSON.stringify(request)}`);
      const response = await this.bankClient.generateQrPayment(
        apiToken.body.token,
        request,
      );
      this.logger.log(`generateQrPayment Response ${JSON.stringify(response)}`);
      const paymentGenerated = await this.prismaService.paymentGenerated.create(
        {
          data: {
            transactionNo: request.transactionNo,
            transactionRefCode: request.transactionRefCode,
            amount: request.amount,
            qrValue: response.QRValue,
            qrExpirationTime: now.add(qrPaymentExpiredTime, 'minute').toDate(),
          },
        },
      );

      await this.prismaService.purchaseRequest.update({
        where: {
          id: record.id,
        },
        data: {
          paymentGeneratedId: paymentGenerated.id,
          status: 'GENERATED_PAYMENT',
        },
      });

      return paymentGenerated;
    } catch (e) {
      this.logger.log(e);
      throw 'Cannot generate new QR';
    }
  }

  async receivedPaymentMessage(message: PaymentNotification) {
    try {
      const revertTransactionRefCode = [
        message.transactionRefCode.slice(0, 3),
        message.transactionRefCode.slice(3, 11),
        message.transactionRefCode.slice(11),
      ].join('-');
      const purchaseReqRecord =
        await this.prismaService.purchaseRequest.findFirst({
          where: { referenceCode: revertTransactionRefCode },
        });

      if (!purchaseReqRecord) {
        throw `Cannot find purchaseRequest [${revertTransactionRefCode}]`;
      }

      if (message.amount != purchaseReqRecord.amount) {
        await this.prismaService.purchaseRequest.update({
          where: { id: purchaseReqRecord.id },
          data: {
            status: PurchaseRequestStatus.REJECTED,
          },
        });
        throw 'Unequal amount';
      }

      const paymentReceived = await this.prismaService.paymentReceived.create({
        data: {
          transactionNo: message.transactionNo,
          transactionRefCode: message.transactionRefCode,

          customerName: message.payerName,
          customerBankAccount: message.payerAccountNo,
          customerBankCode: message.payerBankCode,

          amount: message.amount,

          paymentType: 'EYP',
          paymentDateTime: message.paymentDateTime,
        },
      });

      await this.prismaService.purchaseRequest.update({
        where: { id: purchaseReqRecord.id },
        data: {
          status: PurchaseRequestStatus.RECEIVED_PAYMENT,
          paymentReceivedId: paymentReceived.id,
        },
      });
    } catch (e) {
      this.logger.log(e);
      throw 'Reject payment received';
    }
  }

  async getMT5PaymentHistory(customerCode: string) {
    try {
      return this.prismaService.purchaseRequest.findMany({
        where: {
          product: 'mt5',
          customerCode: {
            equals: customerCode,
          },
          status: { in: ['COMPLETED', 'RECEIVED_PAYMENT'] },
        },
        orderBy: {
          updated_at: 'asc',
        },
        include: {
          plan: true,
          paymentReceived: true,
          userSubscription: true,
        },
      });
    } catch (e) {
      this.logger.log(e);
      throw 'Cannot get MT5 history';
    }
  }
}
