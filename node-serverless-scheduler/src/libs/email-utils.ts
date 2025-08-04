import * as aws from '@aws-sdk/client-ses';
import * as console from 'console';
import * as Nodemailer from 'nodemailer';

export type Attachment = {
  filename: string;
  content?: string | Buffer;
  contentType?: 'text/plain';
  path?: string;
  cid?: string;
};

export const sendEmailToSES = async (
  recipient: string,
  subject: string,
  attachments: Attachment[],
  from = 'biztran-dev+noreply@pi.financial',
  html = ''
) => {
  const ses = new aws.SES({
    apiVersion: '2010-12-01',
    region: 'ap-southeast-1',
  });
  const transporter = Nodemailer.createTransport({
    SES: { ses, aws },
  });
  console.info(`Sending email subject: ${subject} to recipient ${recipient}`);
  await new Promise((resolve, reject) => {
    transporter.sendMail(
      {
        from: from,
        to: `${recipient}`,
        subject: subject,
        text: '',
        attachments: attachments,
        html: html,
        attachDataUrls: true,
      },
      (e, info) => {
        if (e) {
          console.error(`Failed to send email errors: ${e}`);
          reject(e);
        } else {
          console.info(`Sent email successfully`);
          resolve(info);
        }
      }
    );
  });
};

export function convertFilesToAttachments(
  filenames: string[],
  contents: Buffer[]
): Attachment[] {
  return filenames.map((filename, index) => {
    return {
      filename: filename,
      content: contents[index],
      contentType: 'text/plain',
    };
  });
}
