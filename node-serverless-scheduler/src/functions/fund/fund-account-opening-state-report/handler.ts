import type { ValidatedEventAPIGatewayProxyEvent } from '@libs/api-gateway';
import schema from './schema';
import { formatJSONResponse } from '@libs/api-gateway';
import { middyfy } from '@libs/lambda';
import got from 'got';
import { stringify as csvStringify } from 'csv-stringify/sync';
import * as Nodemailer from 'nodemailer';
import * as aws from '@aws-sdk/client-ses';
import { getConfigFromSsm } from '@libs/ssm-config';

// TODO change values
const reportRecipient = 'nutchanon.an@pi.financial';
const reportSubject = 'Fund account opening report';
const reportBody = '';

type ResponseItem = {
  requestReceivedTime: string;
  status: string;
  custCode: string;
};

type Attachment = {
  filename: string;
  content: string | Buffer;
  contentType: 'text/plain';
};

async function getConfig() {
  const [fundServiceHost] = await getConfigFromSsm('fund', ['fund-srv-host']);

  return {
    fundServiceHost,
  };
}

const handle = async (
  date: string | null | undefined,
  ndid: boolean
): Promise<ResponseItem[]> => {
  const dateStr = (date || new Date().toISOString()).slice(0, 10);
  const config = await getConfig();

  const result = await getFundAccountOpeningStates(
    config.fundServiceHost,
    dateStr,
    ndid
  );
  const list = openingStatesToMailAttachments(result, {
    fileNamePrefix: `${dateStr}-`,
  });
  await sendEmailToSES(reportRecipient, list);

  return result;
};

const schedule = async () => {
  await handle(null, false);
};
void schedule();

const _run: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  const result = await handle(event.body.requestDate, false);

  return formatJSONResponse({
    message: result,
    event,
  });
};

const run = middyfy(_run);

const getFundAccountOpeningStates = async (
  fundSrvHost: string,
  requestReceivedDate: string,
  ndid: boolean
): Promise<ResponseItem[]> => {
  try {
    const result = await got
      .get(`${fundSrvHost}/fund-accounts/open-state`, {
        searchParams: { requestReceivedDate, ndid },
      })
      .json<ResponseItem[]>();
    console.info(
      'Received fund account opening state.\n' + JSON.stringify(result)
    );

    return result;
  } catch (e) {
    console.error(
      'Failed to get fund account opening state\n' +
        JSON.stringify(e.response.body)
    );
    throw e;
  }
};

// states: None, Initial, Final, Received, CustomerCreated, AccountCreated, DocsUploadFailed
const ACCOUNT_OPENING_SUCCESSFUL_STATES = ['Final'];
const openingStatesToMailAttachments = (
  results: ResponseItem[],
  { fileNamePrefix = '' } = {}
): Attachment[] => {
  const successList: string[][] = [];
  const failureList: string[][] = [];

  results.forEach((data) => {
    if (ACCOUNT_OPENING_SUCCESSFUL_STATES.includes(data.status)) {
      successList.push([data.custCode, data.status]);
    } else {
      failureList.push([data.custCode, data.status]);
    }
  });

  return [
    {
      filename: `${fileNamePrefix}success.csv`,
      content: csvStringify(successList),
      contentType: 'text/plain',
    },
    {
      filename: `${fileNamePrefix}failed.csv`,
      content: csvStringify(failureList),
      contentType: 'text/plain',
    },
  ];
};

const sendEmailToSES = async (recipient: string, attachments: Attachment[]) => {
  const ses = new aws.SES({
    apiVersion: '2010-12-01',
    region: 'us-east-1',
  });
  const transporter = Nodemailer.createTransport({
    SES: { ses, aws },
  });
  await new Promise((resolve, reject) => {
    transporter.sendMail(
      {
        from: 'biztran-dev+noreply@pi.financial',
        to: `${recipient}`,
        subject: reportSubject,
        text: reportBody,
        attachments,
      },
      (err, info) => {
        if (err) {
          reject(err);
        } else {
          resolve(info);
        }
      }
    );
  });
};

export { schedule, run };
