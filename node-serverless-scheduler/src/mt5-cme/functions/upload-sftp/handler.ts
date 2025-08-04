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

interface Payload {
  date: string;
  timezone: string;
  bucket: string;
  clientAccountFilePrefix: string;
  commissionGroupFilePrefix: string;
  tierFilePrefix: string;
  traderFilePrefix: string;
  traderGroupFilePrefix: string;
  transactionFeeFilePrefix: string;
  marginSymbolFilePrefix: string;
}

const uploadFileToSftp = async (
  fileName: string,
  bucket: string,
  fileKey: string,
  sftpConfig: unknown
) => {
  const sftp = new SFTPClient();

  try {
    const data = await getObject(bucket, fileKey);
    if (!data?.Body || !(data.Body instanceof Readable)) {
      throw new Error(`File not found or invalid: ${bucket}/${fileKey}`);
    }

    await sftp.connect(sftpConfig);
    await sftp.put(data.Body, fileName);

    console.info(`Successfully uploaded: ${fileName}`);
  } catch (error) {
    console.error(`Failed to upload ${fileName}:`, error);
    throw error;
  } finally {
    try {
      await sftp.end();
    } catch (endError) {
      // NOTE: Ignore connection close errors (common on Windows SFTP servers), https://www.npmjs.com/package/ssh2-sftp-client#org1eef7b1
      console.warn('SFTP connection close warning:', endError.message);
    }
  }
};

export const run = async (event: { body: Payload }) => {
  console.info('Event:', event);

  const payload = event.body;
  const bucket = payload.bucket;

  const formattedDate = dayjs(payload.date)
    .tz(payload.timezone)
    .format('YYYYMMDD');

  const fileNames = [
    `${payload.clientAccountFilePrefix}_${formattedDate}.txt`,
    `${payload.commissionGroupFilePrefix}_${formattedDate}.txt`,
    `${payload.tierFilePrefix}_${formattedDate}.txt`,
    `${payload.traderFilePrefix}_${formattedDate}.txt`,
    `${payload.traderGroupFilePrefix}_${formattedDate}.txt`,
    `${payload.transactionFeeFilePrefix}_${formattedDate}.txt`,
    `${payload.marginSymbolFilePrefix}_${formattedDate}.json`,
  ];

  const secret = await getSecretValue(
    'cme-marketdata',
    getAccessibility(SecretManagerType.Secret)
  );

  const uploadPromises = fileNames.map((fileName) => {
    const isJsonFile = fileName.endsWith('.json');

    const sftpConfig = {
      host: secret['TOP_TRADER_SFTP_HOST'],
      port: secret['TOP_TRADER_SFTP_PORT'],
      username: isJsonFile
        ? secret['TOP_TRADER_SFTP_JSON_USERNAME']
        : secret['TOP_TRADER_SFTP_USERNAME'],
      password: isJsonFile
        ? secret['TOP_TRADER_SFTP_JSON_PASSWORD']
        : secret['TOP_TRADER_SFTP_PASSWORD'],
      readyTimeout: 50000,
      keepaliveInterval: 10000,
    };

    const fileKey = `${formattedDate}/${fileName}`;

    return uploadFileToSftp(fileName, bucket, fileKey, sftpConfig);
  });

  try {
    await Promise.all(uploadPromises);
    console.info(`Successfully uploaded ${fileNames.length} files`);
    return { status: 'success', filesUploaded: fileNames.length };
  } catch (error) {
    console.error('Upload failed:', error);
    throw new Error(`Upload failed: ${error.message}`);
  }
};

export const main = middyfy(run);
