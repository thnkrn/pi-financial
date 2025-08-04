import { DbClientBackoffice } from '@libs/backoffice/db-backoffice-client';
import { Attachment, MailerClient } from '@libs/ses';
import { DbClientSmartSales } from '@libs/smartsales/db-smartsales-client';
import { Injectable, Logger } from '@nestjs/common';
import { isDevelopment, isDevelopmentOrStaging } from '@utils/env-helper';
import { HolidayHelper } from '@utils/holiday-helper';
import { readBufferFileFromS3 } from '@utils/s3-helper';
import { getFirstName } from '@utils/smartsales-model-utils';
import { getNewUsersFromSBA, getUniqCustcodes } from '@utils/users-helper';
import dayjs from 'dayjs';
import fs from 'fs';
import path from 'path';
import pug from 'pug';
import { ReportService } from '../report/report.service';
import { TaxService } from '../tax/tax.service';
import { UserSubscriptionService } from '../user_subscription/user_subscription.service';
import { FilteredCustomer } from './model/customer-with-marketing-detail';

@Injectable()
export class TasksService {
  private readonly smartSalesClient = new DbClientSmartSales();
  private readonly backofficeClient = new DbClientBackoffice();
  private readonly holidayHelper = new HolidayHelper();
  private readonly mailer = new MailerClient();
  private readonly logger = new Logger(TasksService.name);
  constructor(
    private readonly userSubscriptionService: UserSubscriptionService,
    private readonly reportService: ReportService,
    private readonly taxService: TaxService,
  ) {}

  async manualOracleDailyReport(
    fromTime: string,
    toTime: string,
    mailTo?: string,
  ) {
    try {
      await this.reportService.generateOracleDailyReport(
        fromTime,
        toTime,
        mailTo,
      );
    } catch (e) {
      this.logger.error(e);
    }
  }

  async manualOracleMonthlyReport(month: string, mailTo?: string) {
    try {
      const monthDate = dayjs(month, 'YYYYMM').tz('Asia/Bangkok');
      const startDate = monthDate.startOf('month');
      const endDate = dayjs().endOf('month').add(-1, 'month');

      await this.reportService.generateOracleMonthlyReport(
        startDate.format('YYYYMMDDHHmmss'),
        endDate.format('YYYYMMDDHHmmss'),
        mailTo,
      );
    } catch (e) {
      this.logger.error(e);
    }
  }

  async manualMonthlyTaxReport(month: string, mailTo?: string) {
    const monthDate = dayjs(month, 'YYYYMM').tz('Asia/Bangkok');
    const startDate = monthDate.startOf('month');
    const endMonth = monthDate.endOf('month');

    await this.taxService.generateMonthlyReport(
      startDate.format('YYYYMMDDHHmmss'),
      endMonth.format('YYYYMMDDHHmmss'),
      mailTo,
    );
  }

  async manualTKSDailyReport(fromTime: string, toTime: string) {
    try {
      await this.taxService.generateTaxFile(fromTime, toTime);
    } catch (e) {
      this.logger.error(e);
    }
  }

  isLastWorkingDay(toTime: string, effectiveDate: string) {
    const _processDate = dayjs(toTime);
    const _effectiveDate = dayjs(effectiveDate);
    const lastWorkingDay =
      _effectiveDate.month() > _processDate.month() ||
      _effectiveDate.year() > _processDate.year();
    return lastWorkingDay;
  }

  async updateSubscriptions(fromTime: string, toTime: string) {
    try {
      this.logger.log(`fromTime: ${fromTime}, toTime: ${toTime}`);
      const effectiveDate = await this.holidayHelper.findNextEffectiveDate(
        toTime,
      );

      const processDate = dayjs(toTime).format('YYYYMMDD');
      const isMonthEndProcess = this.isLastWorkingDay(toTime, effectiveDate);

      this.logger.log(
        `processDate: ${processDate}, effectiveDate: ${effectiveDate}, isMonthEndProcess: ${isMonthEndProcess}`,
      );
      await this.userSubscriptionService.updateAllPurchaseRequestInDate(
        fromTime,
        toTime,
        isMonthEndProcess,
        effectiveDate,
        processDate,
      );
    } catch (e) {
      this.logger.error(e);
    }
  }

  /**
   * Update custcode incase of incorrect custcode was entered
   * only when status is RECEIVED_PAYMENT or COMPLETED
   * @param {string} currentCustCode Incorrect custcode
   * @param {string} newCustcode Correct custcode
   * @param {string} effectiveDate Active date (next working day)
   * @param {boolean?} force Force update when status is COMPLETED
   */
  async manualUpdateCustCode(
    currentCustCode: string,
    newCustcode: string,
    effectiveDate: string,
    force = false,
  ): Promise<string> {
    const pugPayload = {} as ManualUpdateCustCodePayload;
    pugPayload.currentCustCode = currentCustCode;
    pugPayload.newCustCode = newCustcode;

    let updateStatus = '';
    this.logger.log(
      `UpdateCustCode: Updating Custcode ${currentCustCode} to ${newCustcode}, effectiveDate: ${effectiveDate}, force: ${force}`,
    );

    try {
      const result = await this.userSubscriptionService.manualUpdateCustCode(
        currentCustCode,
        newCustcode,
        effectiveDate,
        force,
      );

      if (result.warning) {
        this.logger.warn(`UpdateCustCode: ${result.warning}`);
        updateStatus = 'failed';
        pugPayload.bodyMessage = `Unable to update payment of Custcode ${currentCustCode} to ${newCustcode}. (Warn: ${result.warning})`;
        return result.warning;
      } else {
        const pr = result.purchaseRequest;
        this.logger.log('UpdateCustCode: success');
        updateStatus = 'successfully.';
        pugPayload.bodyMessage = 'Update new Custcode successfully.';

        pugPayload.referenceCode = pr.referenceCode;
        pugPayload.amount = pr.amount.toString();
        pugPayload.status = pr.status;
        pugPayload.updatedAt = dayjs(pr.updated_at).format(
          'YYYY-MM-DD HH:MM:SS',
        );

        if (force && result.accounts != null) {
          pugPayload.accounts = result.accounts.map((i) => i.account);

          if (result.subscription)
            pugPayload.userSubscription = JSON.stringify(result.subscription);
          if (result.purchaseRequest)
            pugPayload.purchaseRequest = JSON.stringify(result.purchaseRequest);
        }
      }
    } catch (e) {
      updateStatus = 'failed';
      pugPayload.bodyMessage = `Failed to update Custcode ${currentCustCode} to ${newCustcode}. (Error: ${e})`;
      this.logger.error(`UpdateCustCode: ${e}`);
      return e.message;
    } finally {
      this.logger.log('UpdateCustCode: Notify result.');

      const emailTemplate = this.getEmailTemplate('update-custcode-report');
      const htmlOutput = emailTemplate(pugPayload);
      await this.mailer.sendEmai({
        sender: process.env.MANUAL_UPDATE_CUSTCODE_FROM,
        toRecipients: process.env.MANUAL_UPDATE_CUSTCODE_TO,
        subject: `[MT5] Manual update custcode ${updateStatus}`,
        bodyHtml: htmlOutput,
      });
    }
  }

  /**
   * Schedules a process to remind users of their MT5 account expiry date.
   * @param date The date to use as a reference for finding customers to remind. e.g. 20231130
   * @param mktId
   * @returns A Promise that resolves when the process is complete.
   */
  async remindExpiryUser(date: string, mktId?: string) {
    try {
      const targetDate = dayjs(date).format('YYYY-MM-DD');
      const endOfMonthDay = dayjs(
        await this.holidayHelper.findEndOfMonthEffectiveDate(targetDate),
      ).format('DD-MM-YYYY');
      const nextWorkingDay = dayjs(
        await this.holidayHelper.findStartOfMonthEffectiveDate(targetDate),
      ).format('DD-MM-YYYY');
      this.logger.log(
        `input=${date}, targetDate=${targetDate}, endOfMonthDay=${endOfMonthDay}, nextWorkingDay=${nextWorkingDay}`,
      );

      let icWithCustomers = await this.getCustomersToBeReminded(targetDate);
      icWithCustomers = mktId
        ? icWithCustomers.filter((i) => i.mktId === mktId)
        : icWithCustomers;
      this.logger.log(`icWithCustomers: ${icWithCustomers.length}`);

      const marketingEmailTemplate = this.getEmailTemplate(
        'remind-expiry-users-to-marketing',
      );
      const marketingEmailSubject =
        'แจ้งเตือนบัญชีลูกค้า MT5 ที่จะต้องชำระค่าบริการก่อนการใช้งานในเดือนถัดไป';
      const customerEmailTemplate = this.getEmailTemplate(
        'remind-expiry-users-to-customer',
      );
      const customerEmailSubject =
        'แจ้งเตือนการสิ้นสุดการเป็นสมาชิกบริการ MT5 ในเดือนถัดไป';
      const attachments: Attachment[] = [];
      if (!isDevelopment()) {
        const imgFileBuffer = await readBufferFileFromS3(
          'email-template/mt5-cancellation.jpg',
        );
        const pdfFileBuffer = await readBufferFileFromS3(
          'email-template/mt5-manual.pdf',
        );
        attachments.push({
          filename: 'การยกเลิกบัญชี MT5.jpg',
          content: imgFileBuffer,
          contentType: 'image/jpeg',
        });
        attachments.push({
          filename: 'คู่มือการชำระค่าบริการ MT5 ผ่านหน้าเว็บไซต์.pdf',
          content: pdfFileBuffer,
          contentType: 'application/pdf',
        });
      }

      const emailRegex = /\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b/g;
      for (const icWithCustomer of icWithCustomers) {
        try {
          const marketingHtmlOutput = marketingEmailTemplate({
            filteredCustomer: icWithCustomer,
            endOfMonthDay: endOfMonthDay,
            nextWorkingDay: nextWorkingDay,
          });

          const matches = icWithCustomer.mktEmail.match(emailRegex) || [];
          const trimmedEmail = matches[0] || '';

          this.logger.log(
            `sending email to ic: ${icWithCustomer.mktName} email: ${trimmedEmail} with ${icWithCustomer.customers.length} customers`,
          );
          await this.sendEmail(
            trimmedEmail,
            marketingEmailSubject,
            marketingHtmlOutput,
            attachments,
            process.env.REMIND_USER_EMAIL_CC,
          );
        } catch (e) {
          this.logger.error(
            `Failed to send email to IC: ${icWithCustomer.mktId}`,
          );
        }

        for (const customer of icWithCustomer.customers) {
          try {
            const customerHtmlOutput = customerEmailTemplate({
              filteredCustomer: customer,
            });

            const matches = customer.email.match(emailRegex) || [];
            if (!matches.length) {
              console.warn(`Incorrect email format: '${customer.email}'`);
            } else {
              const trimmedCustomerEmail = matches[0] || '';
              this.logger.log(`sending email to customer: ${customer.email}`);
              await this.sendEmail(
                trimmedCustomerEmail,
                customerEmailSubject,
                customerHtmlOutput,
                attachments,
              );
            }
          } catch (e) {
            this.logger.error(
              `Failed to send email to customer: ${customer.custcode}`,
            );
          }
        }
      }
      this.logger.log(
        `Finished sending emails to ${icWithCustomers.length} recipients`,
      );
    } catch (e) {
      this.logger.error(`Error scheduleProcessRemindUser: ${e}`);
    }
  }

  private async sendEmail(
    email: string,
    subject: string,
    htmlOutput: string,
    attachments: Attachment[],
    ccRecipients?: string,
  ): Promise<any> {
    await this.mailer.sendEmailWithAttachment({
      sender: process.env.REMIND_USER_EMAIL_FROM,
      toRecipients: isDevelopmentOrStaging()
        ? process.env.REMIND_USER_EMAIL_CC
        : email,
      ccRecipients,
      subject: subject,
      bodyHtml: htmlOutput,
      attachments,
    });
  }

  /**
   * Sync MT5 user from Freeview (BackOfficeN DB) into user_subscription table
   * @param {string} date Date to be fetched from BackOfficeN (format yyyy-MM-dd)
   */
  async manualSyncMt5UserFromFreeView(date: string) {
    try {
      this.logger.log(`BACKOFFICE_DB_HOST: ${process.env.BACKOFFICE_DB_HOST}`);
      this.logger.log(`date: ${date}`);

      // Get freeview users from BackofficeN DB
      const freeviewUsers = await getNewUsersFromSBA(date);
      this.logger.log(`freeviewUsers: ${freeviewUsers.length}`);
      if (freeviewUsers.length === 0) return true;

      // Find existing users in our system
      const custcodes = getUniqCustcodes(freeviewUsers);
      const existingUsers =
        await this.userSubscriptionService.findAllByCustomerCodes(custcodes);

      // Extract a new users
      const newUsers = freeviewUsers.filter((row) => {
        const existingUser = existingUsers.find(
          (user) => user.customerCode === row.custcode?.trim(),
        );
        return !existingUser;
      });
      this.logger.log(
        `existUsers: ${existingUsers.length}, newUsers: ${newUsers.length}`,
      );
      if (newUsers.length === 0) return true;

      // New users will be expired at the end of the next month (trial period)
      const activeDate = dayjs().toDate();
      const expiredDate = dayjs()
        .add(1, 'month')
        .endOf('month')
        .tz('Asia/Bangkok')
        .startOf('day')
        .toDate();
      this.logger.log(
        `activeDate: ${activeDate.toLocaleDateString()}, expiredDate: ${expiredDate.toLocaleDateString()}`,
      );

      const newCustcodes = newUsers.map((i) => i.custcode);
      const results = await this.userSubscriptionService.insertNewCustomers(
        newCustcodes,
        activeDate,
        expiredDate,
      );

      this.logger.log(`Insert results: ${JSON.stringify(results)}`);
      return results.count === newCustcodes.length;
    } catch (error) {
      this.logger.error(error);
      throw error;
    }
  }

  private getEmailTemplate(name: string) {
    const pugtemplatePath = path.join(
      process.cwd(),
      `/ses-email-template/${name}.pug`,
    );
    const pugTemplate = fs.readFileSync(pugtemplatePath, 'utf8');
    return pug.compile(pugTemplate);
  }

  private async getCustomersToBeReminded(
    targetDate: string,
  ): Promise<FilteredCustomer[]> {
    const custcodes =
      await this.userSubscriptionService.findNearlyExpiredSubscription(
        targetDate,
      );

    if (custcodes.length === 0) {
      this.logger.log('No customers to be reminded');
      return [];
    }

    await this.smartSalesClient.connect();
    const customers = await this.smartSalesClient.getCustomersDetail(custcodes);
    if (customers.length === 0) {
      this.logger.warn(
        `Customers not found on SmartSales: ${custcodes.join(',')}`,
      );
      return [];
    }

    const mktIds = customers.map((customer) => customer.mktId);
    await this.backofficeClient.connect();
    const mktDetail = await this.backofficeClient.getMktEmail(mktIds);
    if (mktDetail.length === 0) {
      this.logger.warn(`MktDetail not found on SBA: ${mktIds.join(',')}`);
      return [];
    }

    const emailByMktId = mktDetail.reduce((acc, detail) => {
      acc[detail.userid] = detail.emailaddress;
      return acc;
    }, {});

    const filteredCustomers: FilteredCustomer[] = Object.values(
      customers.reduce((acc, obj) => {
        const { mktId, mktName, teamID, eName, ...rest } = obj;
        if (!acc[mktId]) {
          acc[mktId] = {
            mktId,
            mktName: getFirstName(mktName),
            teamID,
            eName,
            mktEmail: emailByMktId[mktId],
            customers: [],
          };
        }
        acc[mktId].customers.push({ ...rest });
        return acc;
      }, {}),
    );
    return filteredCustomers;
  }
}
