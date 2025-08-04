import s3 from '@libs/s3';
import { Attachment, MailerClient } from '@libs/ses';
import { SFTPClient } from '@libs/sftp';
import {
  DbClientSmartSales,
  getProductFromAccountCode,
} from '@libs/smartsales/db-smartsales-client';
import { ProductType } from '@libs/smartsales/model/product-type';
import { Injectable, Logger } from '@nestjs/common';
import {
  PurchaseRequest,
  PurchaseRequestStatus,
  UserSubscription,
} from '@prisma/client';
import { UserInfo } from '@types';
import { isDevelopment } from '@utils/env-helper';
import { createFile } from '@utils/file-storage';
import {
  getAccountsFromSBA,
  getActiveAccountsFromSBA,
  getInactiveAccountsFromSBA,
  getPayableAccountsFromSBA,
} from '@utils/users-helper';
import dayjs, { Dayjs } from 'dayjs';
import _ from 'lodash';
import { PlanService } from '../plan/plan.service';
import { PrismaService } from '../prisma.service';
import { ExportMT5Data } from '../tasks/model/export-mt5-data';
import { renderDate } from './date-helper';
import { MT5PlanStatus, MT5Status } from './dto/check-status-response.dto';
import { CreateUserSubscriptionDto } from './dto/create-user_subscription.dto';
import { PayableStatusResponse } from './dto/payable-status-response.dto';
import { SyncUserSubscriptionDto } from './dto/sync-user_subscription.dto';

@Injectable()
export class UserSubscriptionService {
  private readonly smartSalesClient = new DbClientSmartSales();
  private readonly sftpClient = new SFTPClient();
  private readonly logger = new Logger(UserSubscriptionService.name);
  private readonly mailer = new MailerClient();

  constructor(
    private readonly planService: PlanService,
    private readonly prismaService: PrismaService,
  ) {}

  async estimateSubscription(
    customerCode: string,
    planId: number,
    effectiveDate: string,
  ) {
    try {
      const plan = await this.planService.getPlan(planId);
      const record = await this.findLatestSubscription(customerCode);

      let startDate: Dayjs;
      if (!record) {
        startDate = dayjs();
      } else {
        const _effectiveDate = dayjs(effectiveDate);
        const latestExpiredDate = dayjs(record.expiredDate).add(1, 'day');
        // If effective date is after latest expired date, use effectiveDate.startOf('month') even effectiveDate is nearly end of Month
        startDate = latestExpiredDate.isAfter(_effectiveDate)
          ? latestExpiredDate
          : _effectiveDate.startOf('month');
      }

      const endDate = startDate
        .add(plan.month - 1, 'month')
        .endOf('month')
        .subtract(1, 'day');

      this.logger.log(
        'estimatedResult',
        startDate.format('YYYY-MM-DD HH:mm:ss'),
        endDate.format('YYYY-MM-DD HH:mm:ss'),
      );

      return {
        startDate,
        endDate,
      };
    } catch (e) {
      this.logger.log(e);
      throw 'Cannot estimate subscription';
    }
  }

  async checkCustomerHasEquityAccount(customerCode: string) {
    try {
      await this.smartSalesClient.connect();
      const accounts = await this.smartSalesClient.getAccounts(customerCode);
      return accounts
        .map((item) => getProductFromAccountCode(item))
        .includes(ProductType.EQUITY);
    } catch (e) {
      this.logger.log(e);
      throw 'Cannot check equity account';
    }
  }

  private async findUserActivationRange(customerCode, inDate: Date) {
    const items = await this.prismaService.userSubscription.findMany({
      where: {
        customerCode: {
          equals: customerCode,
        },
      },
      orderBy: {
        expiredDate: 'desc',
      },
    });

    if (items.length === 0) {
      return { isExpired: true, minActiveDate: null, maxExpiredDate: null };
    }

    let minDate = items[0].activeDate;
    const maxDate = items[0].expiredDate;

    if (items.some((i) => i.expiredDate >= inDate)) {
      minDate = _.chain(items)
        .filter((i) => i.expiredDate >= inDate)
        .map((i) => i.activeDate)
        .min()
        .value();
    }

    return {
      isExpired: dayjs(maxDate).isBefore(dayjs().startOf('day')),
      minActiveDate: minDate,
      maxExpiredDate: maxDate,
    };
  }

  async findCurrentMT5Status(customerCode: string): Promise<MT5Status> {
    const todayDate = dayjs();
    try {
      const recordCount = await this.prismaService.userSubscription.count({
        where: {
          customerCode: {
            equals: customerCode,
          },
        },
      });
      const result = await this.findUserActivationRange(
        customerCode,
        todayDate.toDate(),
      );
      let planStatus;
      if (recordCount == 0) {
        return null;
      } else if (result.isExpired) {
        planStatus = MT5PlanStatus.expired;
      } else {
        const pendingPurchase = await this.prismaService.purchaseRequest.count({
          where: {
            customerCode: {
              equals: customerCode,
            },
            status: {
              equals: 'RECEIVED_PAYMENT',
            },
          },
        });
        planStatus =
          pendingPurchase > 0
            ? MT5PlanStatus.processing
            : recordCount > 1
            ? MT5PlanStatus.active
            : MT5PlanStatus.trial;
      }
      const planExpiredDate = renderDate(result.maxExpiredDate);
      const lastSubscription = renderDate(result.minActiveDate);

      return {
        planStatus,
        planExpiredDate,
        lastSubscription,
      };
    } catch (e) {
      this.logger.log(e);
      throw 'Cannot find current mt5 status';
    }
  }

  /**
   * Finds the preferred customer code from a list of customer codes based on the maximum expired date.
   * @param list - List of customer codes.
   * @returns Customer code or null.
   */
  async findPreferCustomerCodeFromList(list: string[]) {
    const result = await this.prismaService.userSubscription.groupBy({
      by: ['customerCode'],
      where: {
        customerCode: {
          in: list,
        },
      },
      _max: {
        expiredDate: true,
      },
      orderBy: [
        {
          _max: {
            expiredDate: 'desc',
          },
        },
      ],
      take: 1,
    });

    return result?.[0]?.customerCode ?? null;
  }

  async create(
    createUserSubscriptionDto: CreateUserSubscriptionDto,
    effectiveDate: string,
  ) {
    try {
      const customerCode = createUserSubscriptionDto.customerCode;
      const estimatedResult = await this.estimateSubscription(
        customerCode,
        createUserSubscriptionDto.planId,
        effectiveDate,
      );
      return this.prismaService.userSubscription.create({
        data: {
          planId: createUserSubscriptionDto.planId,
          purchaseRequestId: createUserSubscriptionDto.purchaseRequestId,
          customerCode: customerCode,
          activeDate: estimatedResult.startDate.toDate(),
          expiredDate: estimatedResult.endDate.toDate(),
        },
      });
    } catch (e) {
      this.logger.log(e);
      throw 'Cannot creat new user subscription';
    }
  }

  private async saveUsersFromFreeview(newUsersDto: SyncUserSubscriptionDto[]) {
    try {
      return await this.prismaService.userSubscription.createMany({
        data: newUsersDto,
      });
    } catch (e) {
      this.logger.log(e);
      throw new Error('Cannot creat sync user subscription');
    }
  }

  async updateAllPurchaseRequestInDate(
    from: string,
    to: string,
    findExpiredUser: boolean,
    effectiveDate: string,
    processDate: string,
  ): Promise<void> {
    this.logger.log('updateAllPurchaseRequestInDate');
    try {
      // 1. Get all payment received during the period
      const records = await this.prismaService.purchaseRequest.findMany({
        where: {
          status: 'RECEIVED_PAYMENT',
          paymentReceived: {
            paymentDateTime: {
              gte: from,
              lte: to,
            },
          },
        },
      });

      this.logger.log(`records: ${JSON.stringify(records)}`);

      let exportFileRecords: ExportMT5Data[] = [];
      await this.smartSalesClient.connect();

      // 2. For each payment records
      // 3. get inactive accounts from SBA
      // 4. Create new record in table user_subscription with new ExpiredDate
      // 5. Update purchase request status to COMPLETED
      // 6. Append inactive accounts to exportFileRecords to be activated on SBA
      for (const record of records) {
        this.logger.log(`process record: ${record.id}`);
        const inactiveAccounts = await getInactiveAccountsFromSBA(
          record.customerCode,
        );
        this.logger.log(
          `inactiveAccounts: ${JSON.stringify(inactiveAccounts)}`,
        );

        if (!isDevelopment()) {
          await this.create(
            {
              purchaseRequestId: record.id,
              planId: record.planId,
              customerCode: record.customerCode,
            },
            effectiveDate,
          );
          this.logger.log(
            `create userSubscription: ${record.id}, ${record.planId}, ${record.customerCode}`,
          );

          await this.prismaService.purchaseRequest.update({
            where: {
              id: record.id,
            },
            data: {
              status: 'COMPLETED',
            },
          });
          this.logger.log(`update purchaseRequest: ${record.id} COMPLETED`);
        }

        if (inactiveAccounts.length > 0) {
          this.logger.log(
            `${record.customerCode} expired, will be renew: ${inactiveAccounts
              .map((i) => i.account)
              .join(',')}`,
          );

          exportFileRecords = exportFileRecords.concat(
            inactiveAccounts
              .filter((i) => i.account)
              .map((i) => {
                return {
                  accountNo: i.account,
                  isActive: true,
                };
              }),
          );
        } else {
          this.logger.log(`${record.customerCode}: No expired account`);
        }
      }

      const expiredUserExport: ExportMT5Data[] = [];
      // 7. When the service run on last working of each month, it will set findExpiredUser = true
      this.logger.log(`findExpiredUser: ${findExpiredUser}`);
      if (findExpiredUser) {
        const expiredDate = dayjs(from).endOf('month').format('YYYY-MM-DD');

        const users = await this.findExpiredUserInDate(expiredDate);
        await Promise.all(
          users.map(async (user) => {
            const accounts = await getActiveAccountsFromSBA(user.customerCode);

            accounts.forEach((account) => {
              if (account.account) {
                expiredUserExport.push({
                  accountNo: account.account,
                  isActive: false,
                });
              }
            });
          }),
        );
      }

      this.logger.log(`Total activate: ${exportFileRecords.length}`);
      this.logger.log(`Total disactivate:'${expiredUserExport.length}`);
      const tmpRawData = exportFileRecords
        .concat(expiredUserExport)
        .map((item) =>
          [
            item.accountNo,
            item.isActive ? 'Y' : 'N',
            // isActive = 'N' => Date=T, isActive = 'Y' => Date=T+1
            item.isActive ? effectiveDate : processDate,
          ].join('|'),
        );
      this.logger.log(`rawData: ${JSON.stringify(tmpRawData)}`);
      const rawData = tmpRawData.join('\n');

      if (isDevelopment()) {
        await createFile(
          `${process.env.FILE_OUTPUT_PATH}`,
          process.env.SFTP_FILENAME,
          rawData,
        );
      } else {
        this.logger.log('Uploading file');
        await this.sftpClient.uploadSFTPFile(
          rawData,
          `${process.env.SFTP_WORKSPACE}/${process.env.SFTP_FILENAME}`,
        );
        this.logger.log('Finished upload');
        const reportDate = dayjs(to).format('YYYYMMDD');
        await this.uploadFileToS3(reportDate, rawData);
        await this.sendEmail(reportDate, rawData);
      }
    } catch (e) {
      this.logger.error(`updateAllPurchaseRequestInDate: ${e}`);
      throw 'Update all purchase request fail';
    }
  }

  async getThaiEquityAccounts(cuscode: string) {
    const accounts = await this.smartSalesClient.getAccounts(cuscode);
    const equityAccounts = accounts.filter((account) => {
      const type = getProductFromAccountCode(account);
      return type === ProductType.EQUITY;
    });
    return [...new Set(equityAccounts)];
  }

  async uploadFileToS3(reportDate: string, rawData: string) {
    try {
      const s3Client = s3();
      const bucket = `${process.env.USER_SUBSCRIPTION_BUCKET_NAME}`;
      const key = `${reportDate}/${process.env.SFTP_FILENAME}`;
      await s3Client.parallelUpload(bucket, key, Buffer.from(rawData, 'utf8'));
      this.logger.log(`uploadS3 success: ${bucket}/${key}`);
    } catch (error) {
      this.logger.error('uploadS3 fail:', error);
    }
  }

  async sendEmail(reportDate: string, rawData: string) {
    this.logger.log(`Sending email to ${process.env.MT5PORTAL_TO}`);
    if (!process.env.MT5PORTAL_TO) return;

    try {
      const attachment: Attachment = {
        filename: process.env.SFTP_FILENAME,
        content: Buffer.from(rawData, 'utf8'),
        contentType: 'text/csv',
      };

      const subject = `MT-5 Daily export [${reportDate}]`;
      const body = `<!doctype html><html lang='en'><body><p>This report will be sent to Freeview to on/off MT5 users.</p></body></html>`;
      this.mailer.sendEmailWithAttachment({
        sender: process.env.TKS_FROM_EMAIL ?? '',
        toRecipients: process.env.MT5PORTAL_TO ?? '',
        subject,
        bodyHtml: body,
        attachments: [attachment],
      });
    } catch (error) {
      this.logger.error('sendEmail fail:', error);
    }
  }

  /**
   * Finds expired user subscriptions for a given target date to be expired on SBA
   * @param targetDate - The target date to search for expired user subscriptions.
   */
  async findExpiredUserInDate(targetDate: string) {
    return this.prismaService.userSubscription.groupBy({
      by: ['customerCode'],
      _max: {
        expiredDate: true,
      },
      having: {
        expiredDate: {
          _max: {
            equals: new Date(targetDate),
          },
        },
      },
    });
  }

  async findAllByCustomerCode(customerCode: string) {
    return this.prismaService.userSubscription.findMany({
      where: {
        customerCode: {
          equals: customerCode,
        },
      },
    });
  }

  async findAllByCustomerCodes(customerCodes: string[]) {
    return this.prismaService.userSubscription.findMany({
      where: {
        customerCode: {
          in: customerCodes,
        },
      },
    });
  }

  async getPayableStatus(
    custcodeList: string[],
  ): Promise<PayableStatusResponse> {
    const payableDate = new Date('9999-12-31');
    let latestEndDate = null;
    for (const custcode of custcodeList) {
      const accounts = await getPayableAccountsFromSBA(custcode);

      const latestEndDateAccount =
        accounts.length > 0
          ? accounts.reduce((latest, current) => {
              return current.enddate > latest.enddate ? current : latest;
            }, accounts[0])
          : null;

      if (
        latestEndDateAccount &&
        latestEndDateAccount.enddate?.getTime() === payableDate.getTime()
      ) {
        return { payable: true, lastEndDate: latestEndDateAccount.enddate };
      } else if (latestEndDateAccount) {
        latestEndDate =
          latestEndDate === null ||
          latestEndDateAccount.enddate > latestEndDate.enddate
            ? latestEndDateAccount.enddate
            : latestEndDate;
      }
    }

    return { payable: false, lastEndDate: latestEndDate };
  }

  async findActiveSubscriptions(customerCode: string) {
    try {
      const todayDate = dayjs().toDate();
      return this.prismaService.userSubscription.findFirst({
        where: {
          customerCode: {
            equals: customerCode,
          },
          activeDate: {
            lte: todayDate,
          },
          expiredDate: {
            gte: todayDate,
          },
        },
      });
    } catch (e) {
      this.logger.log(e);
      throw 'Cannot find active subscriptions';
    }
  }

  async getProcessingPurchaseRequest(customerCode: string) {
    try {
      return this.prismaService.purchaseRequest.findFirst({
        where: {
          customerCode: {
            equals: customerCode,
          },
          status: {
            equals: PurchaseRequestStatus.RECEIVED_PAYMENT,
          },
        },
        select: {
          referenceCode: true,
        },
      });
    } catch (e) {
      this.logger.log(e);
      throw 'Cannot get processing purchase request';
    }
  }

  async findNearlyExpiredUsers(targetExpiredDate: Date) {
    try {
      const latestSubscriptions =
        await this.prismaService.userSubscription.findMany({
          distinct: ['customerCode'],
          orderBy: {
            expiredDate: 'desc',
          },
          select: {
            customerCode: true,
            expiredDate: true,
          },
        });

      return latestSubscriptions.filter(
        (item) => targetExpiredDate > item.expiredDate,
      );
    } catch (e) {
      this.logger.log(e);
      throw new Error('Cannot find nearly expired users');
    }
  }

  async findNearlyExpiredSubscription(targetExpiredDate: string) {
    try {
      const subscriptions: any[] = await this.prismaService.$queryRaw`
        SELECT customer_code AS customerCode, MAX(expired_Date) AS expiredDate
        FROM user_subscription
        GROUP BY customer_code
        HAVING MAX(expired_Date) = ${targetExpiredDate};
      `;
      return subscriptions.map(
        (subscription) => subscription.customerCode as string,
      );
    } catch (e) {
      this.logger.log(e);
      throw new Error('Cannot find nearly expired subscription');
    }
  }

  private async findLatestSubscription(customerCode: string) {
    try {
      return this.prismaService.userSubscription.findFirst({
        where: {
          customerCode: customerCode,
        },
        orderBy: {
          expiredDate: 'desc',
        },
      });
    } catch (e) {
      this.logger.log(e);
      throw 'Cannot find latest subscription';
    }
  }

  async findLatestSubscriptionByCreatedAt(customerCode: string) {
    return this.prismaService.userSubscription.findFirst({
      where: {
        customerCode: customerCode,
      },
      orderBy: {
        createdAt: 'desc',
      },
      include: {
        purchaseRequest: {
          select: {
            referenceCode: true,
            amount: true,
            status: true,
            updated_at: true,
          },
        },
      },
    });
  }

  async insertNewCustomers(
    customerCodes: string[],
    activeDate: Date,
    expiredDate: Date,
  ) {
    const users = customerCodes.map((code) => {
      return {
        customerCode: code.trim(),
        activeDate,
        expiredDate,
      } as SyncUserSubscriptionDto;
    });
    return await this.saveUsersFromFreeview(users);
  }

  async manualUpdateCustCode(
    currentCustCode: string,
    newCustcode: string,
    effectiveDate: string,
    force = false,
  ): Promise<ManualUpdateCustCodeResponse> {
    // Validate if target custcode is MT5 User
    // const currentAccounts = await getAccountsFromSBA(currentCustCode, null);
    // if (!currentAccounts?.length)
    //   return {
    //     warning: `Current Custcode '${currentCustCode}' not found on SBA DB`,
    //   };
    const newAccounts = await getAccountsFromSBA(newCustcode, null);
    if (!newAccounts?.length)
      return {
        warning: `New Custcode '${newCustcode}' wasn't found on SBA DB`,
      };

    // Find latest payment record
    const subscription = await this.findLatestSubscriptionByCreatedAt(
      currentCustCode,
    );

    if (!subscription)
      return {
        warning: `No subscription found for CustCode: '${currentCustCode}'`,
      };
    // Validate PurchaseRequest.status must equals to RECEIVED_PAYMENT
    if (!subscription.planId) return { warning: 'planId is null' };
    const status = subscription?.purchaseRequest?.status?.toString() ?? 'N/A';
    const isForceUpdate =
      force && status === PurchaseRequestStatus.COMPLETED.toString();
    if (
      !isForceUpdate &&
      status !== PurchaseRequestStatus.RECEIVED_PAYMENT.toString()
    )
      return {
        warning: `Cannot update custcode because subscription status is ${status}`,
      };

    const estimatedPeriod = await this.estimateSubscription(
      newCustcode,
      subscription.planId,
      effectiveDate,
    );
    this.logger.log(
      `EstimatedPeriod: ${estimatedPeriod.startDate.format(
        'YYYY-MM-DD',
      )} to ${estimatedPeriod.endDate.format('YYYY-MM-DD')}`,
    );

    // Update subscription
    this.logger.log(
      `Updating subscription.id=${subscription.id} set CustomerCode=${newCustcode}`,
    );
    await this.prismaService.userSubscription.update({
      where: {
        id: subscription.id,
      },
      data: {
        customerCode: newCustcode,
        activeDate: estimatedPeriod.startDate.toDate(),
        expiredDate: estimatedPeriod.endDate.toDate(),
      },
    });
    // Update purchaseRequest
    this.logger.log(
      `Updating purchaseRequest.id=${subscription.purchaseRequestId} set CustomerCode=${newCustcode}`,
    );
    await this.prismaService.purchaseRequest.update({
      where: {
        id: subscription.purchaseRequestId,
      },
      data: {
        customerCode: newCustcode,
      },
    });

    return {
      subscription,
      purchaseRequest:
        subscription?.purchaseRequest as unknown as PurchaseRequest,
      accounts: isForceUpdate ? newAccounts : [],
    };
  }
}

type ManualUpdateCustCodeResponse = {
  warning?: string;
  purchaseRequest?: PurchaseRequest;
  accounts?: UserInfo[];
  subscription?: UserSubscription;
};
