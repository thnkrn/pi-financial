import { Body, Controller, Headers, Logger, Param, Post } from '@nestjs/common';
import { ApiTags } from '@nestjs/swagger';
import { PaymentGenerated, Product, PurchaseRequest } from '@prisma/client';
import {
  getCustomerCodeFromUserId,
  getInternalUser,
} from '@utils/custcode-helper';
import { isDevelopment } from '@utils/env-helper';
import { HolidayHelper } from '@utils/holiday-helper';
import dayjs from 'dayjs';
import { Consumer } from 'sqs-consumer';
import { CustcodeNotFound, UndefineUserId } from '../exception';
import { PlanService } from '../plan/plan.service';
import { UserSubscriptionService } from '../user_subscription/user_subscription.service';
import { CreatePurchaseRequestResponse } from './dto/create-purchase-request-response.dto';
import { CreatePurchaseRequestInput } from './dto/create-purchase.dto';
import {
  PaymentHistoryListResponse,
  PaymentHistoryResponse,
  PaymentStatus,
} from './dto/payment-history-response.dto';
import { PaymentStatusResponse } from './dto/payment-status-response.dto';
import { PurchaseService } from './purchase.service';
import { PaymentNotification } from './sqs-message/payment-notification-message.dto';

@Controller('/secure/purchase')
@ApiTags('Purchase')
export class PurchaseController {
  private readonly logger = new Logger(PurchaseController.name);
  private readonly holidayHelper = new HolidayHelper();
  private _sqsConsumer!: Consumer;

  constructor(
    private readonly purchaseService: PurchaseService,
    private readonly planService: PlanService,
    private readonly userSubscriptionService: UserSubscriptionService,
  ) {
    if (!isDevelopment()) {
      this._sqsConsumer = Consumer.create({
        queueUrl: process.env.SQS_QUEUE_URL,
        region: process.env.AWS_REGION,
        handleMessage: async (message) => {
          try {
            this.logger.log('[SQS] get message');
            this.logger.debug(message.Body);
            const paymentNotification = JSON.parse(
              message.Body,
            ) as PaymentNotification;
            this.logger.debug(paymentNotification);
            await this.handlePaymentNotification(paymentNotification);
          } catch (e) {
            this.logger.error(e);
            return;
          }
        },
      });

      this._sqsConsumer.on('error', (err) => {
        this.logger.error(err.message);
      });

      this._sqsConsumer.on('processing_error', (err) => {
        this.logger.error(err.message);
      });

      this._sqsConsumer.start();
    }
  }

  private async handlePaymentNotification(message: PaymentNotification) {
    if (
      !message.transactionRefCode.startsWith(process.env.TRANSACTION_PREFIX) ||
      !message.isSuccess
    ) {
      this.logger.log(`Unhandled SQS Message [${message}]`);
      return;
    }
    await this.purchaseService.receivedPaymentMessage(message);
  }

  @Post('mt5/status/:id')
  async getPaymentStatus(
    @Param('id') id: string,
  ): Promise<PaymentStatusResponse> {
    const purchaseRequest = await this.purchaseService.getPurchaseRequest(id);
    return {
      status: purchaseRequest.status,
    };
  }

  @Post('pushPaymentNotification')
  async pushPaymentNotification(
    @Body() data: PaymentNotification,
  ): Promise<void> {
    await this.handlePaymentNotification(data);
  }

  @Post('mt5/request')
  async createNewRequest(
    @Headers('user-id') userId: string,
    @Body() params: CreatePurchaseRequestInput,
  ): Promise<CreatePurchaseRequestResponse> {
    if (!userId) {
      this.logger.error(`UndefineUserId ${userId}`);
      throw new UndefineUserId();
    }
    try {
      const result = await getInternalUser(userId);
      const customerCode =
        await this.userSubscriptionService.findPreferCustomerCodeFromList(
          result.data.customerCodes.map((it) => it.code),
        );

      if (customerCode == null) {
        this.logger.error(`CustcodeNotFound ${userId}`);
        throw new CustcodeNotFound();
      }

      const purchasingPlan = await this.planService.getPlan(params.planId);
      const pendingRequest = await this.purchaseService.getPendingRequest(
        customerCode,
        purchasingPlan.id,
      );

      let purchaseRequest: PurchaseRequest;
      let paymentGenerated: PaymentGenerated;
      if (!pendingRequest) {
        purchaseRequest =
          await this.purchaseService.generateNewMT5PurchaseRequest(
            customerCode,
            purchasingPlan.id,
            purchasingPlan.price,
          );
        paymentGenerated = await this.purchaseService.generateNewQrPayment(
          purchaseRequest,
          Product.mt5,
        );
      } else {
        purchaseRequest = pendingRequest;
        paymentGenerated = pendingRequest.paymentGenerated;
      }

      const effectiveDate = await this.holidayHelper.findNextEffectiveDate(
        dayjs().format('YYYY-MM-DD HH:mm:ss'),
      );
      const estimateResult =
        await this.userSubscriptionService.estimateSubscription(
          customerCode,
          purchasingPlan.id,
          effectiveDate,
        );

      return {
        id: purchaseRequest.id,
        referenceCode: purchaseRequest.referenceCode,
        amount: purchaseRequest.amount.toFixed(2),
        qrExpiredTime: dayjs(paymentGenerated.qrExpirationTime).format(
          'YYYYMMDDHHmmss',
        ),
        qrValue: paymentGenerated.qrValue,
        serviceStartAt: estimateResult.startDate.format('MMMM, YYYY'),
        serviceEndAt: estimateResult.endDate.format('MMMM, YYYY'),
      };
    } catch (e) {
      throw e;
    }
  }

  private checkPaymentStatus(status: string): PaymentStatus {
    if (status === 'COMPLETED') {
      return PaymentStatus.Completed;
    } else if (status === 'RECEIVED_PAYMENT') {
      return PaymentStatus.Processing;
    } else {
      return PaymentStatus.Default;
    }
  }

  @Post('mt5/history')
  async getPaymentHistory(
    @Headers('user-id') userId: string,
  ): Promise<PaymentHistoryListResponse> {
    if (!userId) {
      this.logger.error(`UndefineUserId = ${userId}`);
      throw new UndefineUserId();
    }
    const customerCode = await getCustomerCodeFromUserId(userId);
    const response: PaymentHistoryResponse[] = [];
    try {
      const result = await this.purchaseService.getMT5PaymentHistory(
        customerCode,
      );
      const items = result.map((item, index) => {
        const newItem: PaymentHistoryResponse = {
          transactionNo: index + 1,
          paymentDate: dayjs(
            item.paymentReceived.paymentDateTime,
            'YYYYMMDDHHmmss',
          ).format('YYYY-MM-DD'),
          clientCustcode: customerCode,
          mt5Period: `${item.plan.month} months`,
          validUntil: item.userSubscription?.expiredDate
            ? dayjs(item.userSubscription.expiredDate).format('YYYY-MM-DD')
            : null,
          amount: +item.amount.toFixed(2),
          status: this.checkPaymentStatus(item.status),
        };
        return newItem;
      });
      return { items: items };
    } catch (e) {
      this.logger.error(e);
    }
    return { items: response };
  }
}
