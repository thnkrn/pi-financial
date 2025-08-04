import { middyfy } from '@libs/lambda';
import { storeFileToS3 } from '@libs/s3-utils';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import { transactionFeeFileConfig } from '../../models/TransactionFeeFileConfig';
import { FixedLengthGenerator } from '../../utils/FixedLengthGenerator';

dayjs.extend(utc);
dayjs.extend(timezone);

interface Payload {
  date: string;
  timezone: string;
  filePrefix: string;
  fileDir: string;
  backofficeBucket: string;
  cmeBucket: string;
  configs: Record<string, string>[];
}

const run = async (event) => {
  console.info('Event:', JSON.stringify(event));

  try {
    const payload = event.body as Payload;
    const {
      configs,
      filePrefix,
      fileDir,
      backofficeBucket,
      cmeBucket,
      timezone,
      date,
    } = payload;

    const fullRecords = configs.map((cfg) => ({
      ...cfg,
      custodian_account: 'CLIENT',
      trade_category: 'F',
      filler: '',
    }));

    const generator = new FixedLengthGenerator(transactionFeeFileConfig);
    const fileContent = generator.generate(fullRecords);

    const formattedDate = dayjs(date).tz(timezone).format('YYYYMMDD');
    const fileName = `${filePrefix}_${formattedDate}.txt`;
    const fileKey = `${formattedDate}/${fileName}`;
    const boFileKey = `${fileDir}/${fileKey}`;

    await storeFileToS3(
      backofficeBucket,
      Buffer.from(fileContent, 'utf8'),
      boFileKey
    );
    await storeFileToS3(cmeBucket, Buffer.from(fileContent, 'utf8'), fileKey);

    console.info('File saved to S3:', fileKey);

    return {
      body: {
        fileKey: boFileKey,
        recordCount: fullRecords.length,
        success: true,
      },
    };
  } catch (error) {
    console.error('Error details:', error);
    throw new Error(`Failed to process: ${error.message}`);
  }
};

export const main = middyfy(run);
