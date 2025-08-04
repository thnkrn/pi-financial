import s3 from '@libs/s3';
import { Injectable, Logger } from '@nestjs/common';
import { isDevelopment } from '@utils/env-helper';
import { createFile } from '@utils/file-storage';
import { createDailyOracleReport } from '@utils/report-helper';
import dayjs, { Dayjs } from 'dayjs';
import { PrismaService } from '../prisma.service';

import { Attachment, MailerClient } from '@libs/ses';
import {
  PaymentGenerated,
  PaymentReceived,
  Plan,
  PurchaseRequest,
  TaxInvoice,
  UserSubscription,
} from '@prisma/client';
import * as process from 'process';
import { VatCalculator } from '../tax/vat-calculator';
const departmentCode = '90000999';

@Injectable()
export class ReportService {
  private readonly mailer = new MailerClient();
  private readonly logger = new Logger(ReportService.name);
  private readonly vatCalculator = new VatCalculator();

  constructor(private readonly prismaService: PrismaService) {}

  /**
   * Fetch data from PurchaseRequest within date period, then send an email with Excel attachment.
   * @param from - First date of last year, paymentDateTime start, e.g. `20230101000000`
   * @param to - End of current month, paymentDateTime end, e.g. `20240630235959`
   */
  async generateOracleMonthlyReport(from: string, to: string, mailTo?: string) {
    this.logger.log('start generate oracle monthly report');

    try {
      const dateFrom = dayjs(from, 'YYYYMMDD');
      const dateTo = dayjs(to, 'YYYYMMDD');
      this.logger.log(
        `manualOracleMonthlyReport from: ${from}, to: ${to}, dateTo: ${dateTo}`,
      );

      const records = await this.getData(from, to);
      this.logger.log(`records: ${records.length}`);

      const reportDate = dateTo.startOf('month');
      this.processReport(records, dateFrom, reportDate, 'monthly', mailTo);
    } catch (e) {
      this.logger.error(e);
    }
  }

  /**
   * Fetch data from PurchaseRequest within date period, then send an email with Excel attachment.
   * @param from - paymentDateTime start e.g. `202406250001`
   * @param to - paymentDateTime end e.g. `202406260000`
   */
  async generateOracleDailyReport(from: string, to: string, mailTo?: string) {
    this.logger.log('start generate oracle daily report');

    try {
      const dateFrom = dayjs(from, 'YYYYMMDD');
      const dateTo = dayjs(to, 'YYYYMMDD');
      this.logger.log(
        `manualOracleDailyReport from: ${from}, to: ${to}, dateTo: ${dateTo}`,
      );

      const records = await this.getData(from, to);
      this.processReport(records, dateFrom, dateTo, 'daily', mailTo);
    } catch (e) {
      this.logger.error(e);
    }
  }

  /**
   * Returns result set of PurchaseRequest within specific period when status is `COMPLETED` or `RECEIVED_PAYMENT`
   * @param dateFrom - paymentDateTime start e.g. `202406250001`
   * @param dateTo - paymentDateTime end e.g. `202406260000`
   * @returns List of PurchaseRequestResponse
   */
  private async getData(
    dateFrom: string,
    dateTo: string,
  ): Promise<PurchaseRequestResponse[]> {
    const records = await this.prismaService.purchaseRequest.findMany({
      where: {
        paymentReceived: {
          paymentDateTime: {
            gte: dateFrom,
            lte: dateTo,
          },
        },
        status: {
          in: ['COMPLETED', 'RECEIVED_PAYMENT'],
        },
      },
      include: {
        plan: {
          select: {
            month: true,
          },
        },
        paymentGenerated: {
          select: {
            transactionNo: true,
            transactionRefCode: true,
          },
        },
        paymentReceived: {
          select: {
            paymentDateTime: true,
            amount: true,
          },
        },
        userSubscription: {
          select: {
            activeDate: true,
            expiredDate: true,
          },
        },
        taxInvoice: {
          select: {
            amountExVat: true,
            vat: true,
          },
        },
      },
    });

    return records as PurchaseRequestResponse[];
  }

  /**
   * Returns result set of PurchaseRequest within specific period when status is `COMPLETED` or `RECEIVED_PAYMENT`
   * @param records - List of PurchaseRequestResponse
   * @param dateFrom - start date of distribution header on Excel column
   * @param reportDate - date of report name
   * @param reportType - type of report `monthly` or `daily`
   * @param toRecipients - an email to override the To-Recipients
   */
  private async processReport(
    records: PurchaseRequestResponse[],
    dateFrom: dayjs.Dayjs,
    reportDate: dayjs.Dayjs,
    reportType: 'daily' | 'monthly',
    toRecipients?: string,
  ) {
    let sumExVat = 0;
    let sumVat = 0;
    let sumTotal = 0;
    const sumMap = new Map<string, number>();

    let distributionHeader;
    if (records?.length > 0) {
      distributionHeader = this.generateDistributionMonthlyHeader(
        records,
        dateFrom,
      );
    } else {
      distributionHeader = this.generateDistributionHeader(dateFrom, {
        totalMonths: 24,
      });
    }
    const headers = [
      'Cust ID',
      'Transaction Date',
      'Time',
      'Amount ex VAT',
      'VAT',
      'Amount in VAT',
      'Subscription start date',
      'Subscription end date',
      'QR Ref code',
      'Tax Invoice Number',
      'Department Code',
      ...distributionHeader,
    ];

    const reportRecords = await Promise.all(
      records.map(async (row) => {
        const receivedDate = dayjs(
          row.paymentReceived.paymentDateTime,
          'YYYYMMDDHHmmss',
        );
        const amount = Number(row.paymentReceived.amount);
        const activeDate = row.userSubscription
          ? dayjs(row.userSubscription.activeDate)
          : undefined;
        const expiredDate = row.userSubscription
          ? dayjs(row.userSubscription.expiredDate)
          : undefined;

        let amountExVat = row.taxInvoice?.amountExVat ?? 0;
        let vat = row.taxInvoice?.vat ?? 0;
        if (!row.taxInvoice) {
          const extractedPrice = this.vatCalculator.extractPrice(row.amount);
          amountExVat = extractedPrice.rawPrice;
          vat = extractedPrice.vat;
        }

        const normal = [
          row.customerCode,
          receivedDate.format('DD-MMM-YYYY'),
          receivedDate.format('HH:mm:ss'),
          amountExVat,
          vat,
          amount,
          activeDate?.format('DD-MMM-YYYY') ?? '',
          expiredDate?.format('DD-MMM-YYYY') ?? '',
          row.paymentGenerated.transactionNo,
          row.paymentGenerated.transactionRefCode,
          departmentCode,
        ];

        const pricePerMonth = Number((amountExVat / row.plan.month).toFixed(2));
        const overFlow = pricePerMonth * row.plan.month - amountExVat;
        sumExVat += amountExVat;
        sumVat += vat;
        sumTotal += amount;

        let distributionPrice = [];
        if (activeDate) {
          let currentDate = activeDate;
          const map = new Map<string, number>();
          while (currentDate.isBefore(expiredDate)) {
            const key = currentDate.format('MMM-YY');
            const value = currentDate.isSame(expiredDate, 'month')
              ? pricePerMonth - overFlow
              : pricePerMonth;
            map.set(key, value);
            sumMap.set(key, (sumMap.get(key) ?? 0) + value);
            currentDate = currentDate.add(1, 'month');
          }

          distributionPrice = distributionHeader.map((value) => {
            return map.get(value) ?? '';
          });
        }
        return [...normal, ...distributionPrice];
      }),
    );

    const distributionPrice = distributionHeader.map((value) => {
      return sumMap.get(value) ?? '';
    });
    const summaryRow = [
      [
        'Total',
        '',
        '',
        sumExVat,
        sumVat,
        sumTotal,
        '',
        '',
        '',
        '',
        '',
        ...distributionPrice,
      ],
    ];

    const reportBuffer = await createDailyOracleReport(
      headers,
      reportRecords,
      summaryRow,
    );

    await this.exportOracleFile(
      reportDate.format('YYYYMMDD'),
      reportBuffer,
      reportType,
      toRecipients,
    );
  }

  private async exportOracleFile(
    date: string,
    reportBuffer: Buffer,
    type: string,
    mailTo?: string,
  ) {
    const fileName = mailTo
      ? `${date}_mt5_oracle_${type}_manual.xlsx`
      : `${date}_mt5_oracle_${type}.xlsx`;

    if (isDevelopment()) {
      await createFile('out', fileName, reportBuffer);
    } else {
      const s3Client = s3();
      const bucket = `${process.env.USER_SUBSCRIPTION_BUCKET_NAME}`;
      const key = `${date}/${fileName}`;
      await s3Client.parallelUpload(bucket, key, reportBuffer);

      const attachment: Attachment = {
        filename: fileName,
        content: reportBuffer,
        contentType:
          'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      };
      const subject = `[MT5-${type}] Oracle Report`;
      const body = `<!doctype html><html lang='en'><body><p>[MT5-${type}] Oracle Report (${date})</p>Please find out the report in attached file.<p>Best Regards,<br/>Tech Team</p></body></html>`;
      this.mailer.sendEmailWithAttachment({
        sender: process.env.ORACLE_REPORT_EMAIL_FROM,
        toRecipients: mailTo ?? process.env.ORACLE_REPORT_EMAIL_TO,
        ccRecipients: mailTo ? null : process.env.ORACLE_REPORT_EMAIL_CC,
        subject: mailTo ? `${subject} - MANUAL` : subject,
        bodyHtml: body,
        attachments: [attachment],
      });
    }
  }

  private calculateEffectiveDate(records: PurchaseRequestResponse[]) {
    const minActiveDate = records
      .filter((i) => i.userSubscription)
      .map((i) => dayjs(i.userSubscription?.activeDate))
      .sort((a, b) => (a.isBefore(b) ? -1 : 1))
      .at(0);

    const maxExpiredDate = records
      .filter((i) => i.userSubscription)
      .map((i) => dayjs(i.userSubscription?.expiredDate))
      .sort((a, b) => (b.isBefore(a) ? -1 : 1))
      .at(0);

    return {
      minActiveDate: dayjs(minActiveDate),
      maxExpiredDate: dayjs(maxExpiredDate),
    };
  }

  private generateDistributionMonthlyHeader(
    records: PurchaseRequestResponse[],
    fromDate: dayjs.Dayjs,
  ): string[] {
    const effectiveDates = this.calculateEffectiveDate(records);
    const startDistributionDate = effectiveDates.minActiveDate.set('month', 0);
    const endDistributionDate = effectiveDates.maxExpiredDate.set('month', 12);

    const totalMonths = endDistributionDate.diff(
      startDistributionDate,
      'month',
    );

    return this.generateDistributionHeader(fromDate, {
      totalMonths: totalMonths,
      startDate: startDistributionDate,
    });
  }

  private generateDistributionHeader(
    fromDate: Dayjs,
    { totalMonths, startDate }: { totalMonths: number; startDate?: Dayjs },
  ): string[] {
    const startDistributionDate = startDate ?? fromDate.set('month', 0);

    const distributionHeader: string[] = [];
    for (let i = 0; i <= totalMonths; i++) {
      distributionHeader.push(
        startDistributionDate.add(i, 'month').format('MMM-YY'),
      );
    }

    return distributionHeader;
  }
}

type PurchaseRequestResponse = PurchaseRequest & {
  plan: Plan;
  paymentGenerated: PaymentGenerated;
  paymentReceived: PaymentReceived;
  taxInvoice: TaxInvoice;
  userSubscription: UserSubscription;
};
