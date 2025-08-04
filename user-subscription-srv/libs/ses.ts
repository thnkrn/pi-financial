import * as aws from '@aws-sdk/client-ses';
import { SESClient } from '@aws-sdk/client-ses';
import Logger from '@utils/datadog-utils';
import cgsEmailLib from 'email-noti-client';
import nodemailer from 'nodemailer';
import Mail from 'nodemailer/lib/mailer';

export type Attachment = {
  filename: string;
  content: string | Buffer;
  contentType:
    | 'text/plain'
    | 'text/csv'
    | 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    | 'application/pdf'
    | 'image/jpeg';
};

export type EmailBody = {
  sender: string;
  toRecipients: string;
  ccRecipients?: string | null;
  subject: string;
  bodyHtml: string;
  attachments?: Attachment[];
  replyTo?: string;
};

export class MailerClient {
  private readonly stage;
  private readonly author;
  private readonly mail = cgsEmailLib.getInstance();
  private readonly ses;

  constructor(
    stage = process.env.STAGE_NAME,
    author = 'user-subscriptions-srv',
    region = process.env.AWS_REGION,
  ) {
    this.stage = stage;
    this.author = author;

    this.mail.setEnv(this.stage);
    this.mail.setAuthor(this.author);
    this.ses = new SESClient({
      region: region,
    });
  }

  // async sendEmailWithTemplate(
  //   emailFrom: string,
  //   emailTo: string,
  //   emailTemplateId,
  //   emailVariablesMap,
  // ) {
  //   const _emailTo: string | string[] = emailTo.includes(',')
  //     ? emailTo.split(',')
  //     : emailTo;

  //   const sesResp = await this.mail.send(
  //     emailFrom,
  //     _emailTo,
  //     emailTemplateId,
  //     JSON.stringify(emailVariablesMap),
  //   );

  //   if (sesResp && sesResp.success) {
  //     // TODO: to handle case when send invalid from/to/template but library still send success back
  //     Logger.log(`Mail sent ${sesResp}`);
  //   } else {
  //     Logger.log(JSON.stringify(sesResp));
  //     Logger.error('Failure in sending email from SES');
  //   }

  //   return sesResp;
  // }

  // async sendRawEmail(rawMessage) {
  //   const params: SendRawEmailRequest = {
  //     RawMessage: {
  //       Data: rawMessage,
  //     },
  //   };

  //   await this.ses.send(new SendRawEmailCommand(params));
  // }

  async sendEmai({
    sender,
    toRecipients,
    ccRecipients,
    subject,
    bodyHtml,
    replyTo,
  }: EmailBody) {
    await this.sendEmailWithAttachment({
      sender,
      toRecipients,
      ccRecipients,
      subject,
      bodyHtml,
      replyTo,
    });
  }

  async sendEmailWithAttachment({
    sender,
    toRecipients,
    ccRecipients,
    subject,
    bodyHtml,
    attachments,
    replyTo,
  }: EmailBody) {
    const _toRecipients: string | string[] = toRecipients.includes(',')
      ? toRecipients.split(',')
      : toRecipients;

    const _ccRecipients: string | string[] = ccRecipients?.includes(',')
      ? ccRecipients.split(',')
      : ccRecipients ?? '';

    const ses = new aws.SES({
      apiVersion: '2010-12-01',
      region: 'ap-southeast-1',
    });
    const transporter = nodemailer.createTransport({
      SES: { ses, aws },
    });
    const data: Mail.Options = {
      from: sender,
      to: _toRecipients,
      cc: _ccRecipients,
      subject: subject,
      html: bodyHtml,
      attachments,
      replyTo: replyTo ?? process.env.REPLY_EMAIL,
    };
    Logger.log(`Sending email to: ${_toRecipients}, cc: ${_ccRecipients}`);
    await new Promise((resolve, reject) => {
      transporter.sendMail(data, (error, info) => {
        if (error) {
          Logger.error(`Error: sendEmailWithAttachment: ${error}`);
          reject(error);
        } else {
          Logger.log(`Success: sendEmailWithAttachment: ${info}`);
          resolve(info);
        }
      });
    });
  }
}
