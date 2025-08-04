import { SecretManagerType } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import { getObject } from '@libs/s3-utils';
import { getAccessibility, getSecretValue } from '@libs/secrets-manager-client';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import SFTPClient from 'ssh2-sftp-client';
import { Readable } from 'stream';

dayjs.extend(utc);
dayjs.extend(timezone);

interface LambdaEvent {
  fileKey: string;
  bucket: string;
  date: string;
  finalFilePrefix: string;
}

export const run = async (event: LambdaEvent) => {
  const data = await getObject(event.bucket, event.fileKey);

  if (!data) {
    throw new Error(`File not found in S3: ${event.bucket}/${event.fileKey}`);
  }

  const secret = await getSecretValue(
    'cme-marketdata',
    getAccessibility(SecretManagerType.Secret)
  );

  const host = secret['TOP_TRADER_SFTP_HOST'];
  const port = secret['TOP_TRADER_SFTP_PORT'];
  const username = secret['TOP_TRADER_SFTP_USERNAME'];
  const password = secret['TOP_TRADER_SFTP_PASSWORD'];

  const sftp = new SFTPClient();

  const date = dayjs(event.date).tz('Asia/Bangkok').format('YYYYMMDD');
  const finalFileName = `${event.finalFilePrefix}_${date}.json`;

  try {
    await sftp.connect({
      host: host,
      port: port,
      username: username,
      password: password,
      readyTimeout: 50000,
      keepaliveInterval: 10000,
    });

    const finalFile = finalFileName;
    if (data.Body instanceof Readable) {
      await sftp.put(data.Body as Readable, finalFile);
    } else {
      const error = `Cannot read file as stream fileName: ${finalFile}`;
      console.info(error);
      throw error;
    }
  } catch (ex) {
    console.error(ex, 'Fail to place file by SFTP to top trader');
    throw ex;
  } finally {
    try {
      await sftp.end();
    } catch (endError) {
      //NOTE: There is error ECONNRESET if connect to SFTP server that run on Window
      //Ref: https://www.npmjs.com/package/ssh2-sftp-client#org1eef7b1
      console.error('Error while closing SFTP connection:', endError);
    }
  }

  return { finalFile: finalFileName };
};

export const main = middyfy(run);
