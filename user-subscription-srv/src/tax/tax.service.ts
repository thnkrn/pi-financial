import s3 from '@libs/s3';
import { Attachment, MailerClient } from '@libs/ses';
import { DbClientSmartSales } from '@libs/smartsales/db-smartsales-client';
import { Injectable, Logger } from '@nestjs/common';
import { PurchaseRequestStatus } from '@prisma/client';
import { getBankNameFromCode } from '@utils/bank-code-helper';
import { isDevelopment } from '@utils/env-helper';
import { createFile } from '@utils/file-storage';
import { createTaxInvoiceMonthlyReport } from '@utils/report-helper';
import { TKSFileRecord, toTKSFileFormat } from '@utils/tks-file-generator';
import dayjs from 'dayjs';
import process from 'process';
import { PrismaService } from '../prisma.service';
import { CreateTaxInvoiceRequest } from './dto/create-tax-invoice-request.dto';
import { VatCalculator } from './vat-calculator';

@Injectable()
export class TaxService {
  private readonly smartSalesClient = new DbClientSmartSales();
  private readonly vatCalculator = new VatCalculator();
  private readonly logger = new Logger(TaxService.name);
  private readonly mailer = new MailerClient();
  constructor(private readonly prismaService: PrismaService) {}

  private async createTaxInvoice(request: CreateTaxInvoiceRequest) {
    const taxInvoiceNo = `PiT-${request.taxInvoiceNo}`;
    let invoice = await this.prismaService.taxInvoice.findFirst({
      where: {
        taxInvoiceNo: taxInvoiceNo,
      },
    });
    if (!invoice) {
      invoice = await this.prismaService.taxInvoice.create({
        data: {
          taxInvoiceNo: taxInvoiceNo,
          amount: request.amount,
          amountExVat: request.amountExVat,
          vat: request.vat,
          customerName: request.customerName,
          customerId: request.customerId,
        },
      });

      await this.prismaService.purchaseRequest.update({
        where: {
          id: request.purchaseRequestId,
        },
        data: {
          taxInvoiceId: invoice.id,
        },
      });
    }
    return invoice;
  }

  async generateTaxFile(from: string, to: string): Promise<void> {
    const reportDate = dayjs(to).format('YYYYMMDD');
    this.logger.log(
      `manualTKSDailyReport from: ${from}, to: ${to}, reportDate: ${reportDate}`,
    );
    try {
      const successRecords = await this.prismaService.purchaseRequest.findMany({
        where: {
          paymentReceived: {
            paymentDateTime: {
              gte: from,
              lte: to,
            },
          },
          status: {
            equals: PurchaseRequestStatus.COMPLETED,
          },
        },
        include: {
          paymentReceived: {
            select: {
              amount: true,
              paymentDateTime: true,
              customerBankCode: true,
            },
          },
        },
      });

      await this.smartSalesClient.connect();
      const reportRecords = await Promise.all(
        successRecords.map(async (record): Promise<TKSFileRecord> => {
          const accounts = await this.smartSalesClient.getAccounts(
            record.customerCode,
          );
          const customerRecord = await this.smartSalesClient.getCustomerDetail(
            record.customerCode,
          );
          const marketing = await this.smartSalesClient.getMarketingDetail(
            record.customerCode,
          );
          const extractedPrice = this.vatCalculator.extractPrice(
            Number(record.paymentReceived.amount),
          );
          const newInvoice = await this.createTaxInvoice({
            purchaseRequestId: record.id,
            amount: record.amount,
            amountExVat: extractedPrice.rawPrice,
            customerId: customerRecord.cardId,
            customerName: [customerRecord.name, customerRecord.lastname].join(
              ' ',
            ),
            taxInvoiceNo: record.referenceCode,
            vat: extractedPrice.vat,
          });
          return {
            // customerCode: record.customerCode,
            customerCode:
              accounts?.length > 0 && accounts[0]
                ? accounts[0]
                : record.customerCode,
            address: customerRecord.address[0], // sAddress1
            address2: customerRecord.address[1], // sAddress2
            address3: customerRecord.address[2], // sAddress3
            postCode: customerRecord.zipCode, // sFirstZipCode
            idCardNo: customerRecord.cardId, // sCardId
            lastname: customerRecord.lastname,
            location: 'สำนักงานใหญ่',
            marketingName: marketing.marketingName,
            marketingTeam: marketing.marketingTeamName,
            name: customerRecord.name,
            bankCode: getBankNameFromCode(
              record.paymentReceived.customerBankCode,
            ),
            paymentAmount: record.paymentReceived.amount.toFixed(2),
            processDate: dayjs(record.paymentReceived.paymentDateTime).format(
              'YYYYMMDD',
            ),
            rawAmount: extractedPrice.rawPrice.toFixed(2),
            taxAmount: extractedPrice.vat.toFixed(2),
            title: customerRecord.title,
            taxInvoiceNumber: newInvoice.taxInvoiceNo,
          };
        }),
      );
      const buffer = reportRecords
        .map((value) => toTKSFileFormat(value))
        .join('\n');

      const fileName = `PiT-MT5-${reportDate}.txt`;
      if (isDevelopment()) {
        await createFile(`${process.env.FILE_OUTPUT_PATH}`, fileName, buffer);
      } else {
        const s3Client = s3();
        const bucket = `${process.env.USER_SUBSCRIPTION_BUCKET_NAME}`;
        const key = `${reportDate}/${fileName}`;
        await s3Client.parallelUpload(bucket, key, Buffer.from(buffer, 'utf8'));

        const attachment: Attachment = {
          filename: fileName,
          content: buffer,
          contentType: 'text/plain',
        };
        const subject = `TKS Daily File [${reportDate}]`;
        const body = `<!doctype html><html lang='en'><body><p>TKS Daily File [${reportDate}]</p>Please find out the report in attached file.<p>Best Regards,<br/>Tech Team</p></body></html>`;
        this.mailer.sendEmailWithAttachment({
          sender: process.env.TKS_FROM_EMAIL,
          toRecipients: process.env.TKS_DESTINATION_EMAIL,
          subject,
          bodyHtml: body,
          attachments: [attachment],
        });
      }
    } catch (e) {
      this.logger.error(e);
    }
  }

  async generateMonthlyReport(
    from: string,
    to: string,
    mailTo?: string,
  ): Promise<void> {
    this.logger.log(`manualMonthlyTaxReport from: ${from}, to: ${to}`);

    const fromDate = dayjs(from);
    const records = await this.prismaService.purchaseRequest.findMany({
      where: {
        status: { in: ['COMPLETED', 'RECEIVED_PAYMENT'] },
        paymentReceived: {
          paymentDateTime: {
            gte: from,
            lt: to,
          },
        },
      },
      include: {
        paymentReceived: {
          select: {
            paymentDateTime: true,
            transactionRefCode: true,
            customerName: true,
            amount: true,
          },
        },
        taxInvoice: {
          select: {
            taxInvoiceNo: true,
            amount: true,
            amountExVat: true,
            vat: true,
            customerId: true,
            customerName: true,
          },
        },
      },
    });

    const data = records.map((item, index) => {
      const year = item.paymentReceived.paymentDateTime.substring(0, 4);
      const month = item.paymentReceived.paymentDateTime.substring(4, 6);
      const day = item.paymentReceived.paymentDateTime.substring(6, 8);
      const formattedDate = `${year}-${month}-${day}`;

      let amountExVat = item.taxInvoice?.amountExVat;
      let vat = item.taxInvoice?.vat;
      if (!item.taxInvoice) {
        const extractedPrice = this.vatCalculator.extractPrice(
          item.paymentReceived.amount,
        );
        amountExVat = extractedPrice.rawPrice;
        vat = extractedPrice.vat;
      }

      return [
        `${index + 1}`,
        formattedDate,
        item.taxInvoice?.taxInvoiceNo ??
          item.paymentReceived?.transactionRefCode,
        item.taxInvoice?.customerName ?? item.paymentReceived?.customerName,
        item.taxInvoice?.customerId,
        'X',
        '',
        amountExVat,
        vat,
        item.taxInvoice?.amount ?? item.paymentReceived?.amount,
      ];
    });

    const reportBuffer = createTaxInvoiceMonthlyReport(data);
    const fileName = `${fromDate.format('YYYYMMDD')}_tax_invoice_report.xlsx`;

    if (isDevelopment()) {
      await createFile('out', fileName, reportBuffer);
    } else {
      const s3Client = s3();
      const bucket = `${process.env.USER_SUBSCRIPTION_BUCKET_NAME}`;
      const key = `${
        process.env.STAGE_NAME === 'prod' ? '' : `${process.env.STAGE_NAME}`
      }/${fromDate.format('YYYYMMDD')}/${fileName}`;
      await s3Client.parallelUpload(bucket, key, reportBuffer);

      const attachment: Attachment = {
        filename: fileName,
        content: reportBuffer,
        contentType:
          'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      };
      const type = 'tax_monthly';
      const subject = `[MT5-${type}] Oracle Report`;
      const body = `<!doctype html><html lang='en'><body><p>[MT5-${type}] Oracle Report (${from})</p>Please find out the report in attached file.<p>Best Regards,<br/>Tech Team</p></body></html>`;
      this.mailer.sendEmailWithAttachment({
        sender: process.env.ORACLE_REPORT_EMAIL_FROM,
        toRecipients: mailTo ?? process.env.ORACLE_REPORT_EMAIL_TO,
        ccRecipients: process.env.ORACLE_REPORT_EMAIL_CC,
        subject,
        bodyHtml: body,
        attachments: [attachment],
      });
    }
  }
}
