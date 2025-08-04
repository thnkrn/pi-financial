import { middyfy } from '@libs/lambda';
import { storeFileToS3 } from '@libs/s3-utils';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import { Readable } from 'stream';
import * as unzipper from 'unzipper';
import * as XLSX from 'xlsx';

dayjs.extend(utc);
dayjs.extend(timezone);

interface Payload {
  reportPrefix: string;
  reportURL: string;
  bucket: string;
  date: string;
  folder: string;
}

const downloadZipFromUrl = async (url: string): Promise<Buffer> => {
  try {
    const response = await fetch(url);

    if (!response.ok) {
      throw new Error(
        `Failed to download ZIP: ${response.status} ${response.statusText}`
      );
    }

    const arrayBuffer = await response.arrayBuffer();
    const buffer = Buffer.from(arrayBuffer);
    return buffer;
  } catch (error) {
    console.error('Error downloading ZIP:', error);
    throw error;
  }
};

const extractXlsxFromZip = async (zipBuffer: Buffer): Promise<Buffer> => {
  try {
    return new Promise((resolve, reject) => {
      const readable = Readable.from(zipBuffer);
      let xlsxFound = false;

      readable
        .pipe(unzipper.Parse())
        .on('entry', (entry) => {
          const fileName = entry.path;
          const type = entry.type;

          if (type === 'File' && fileName.toLowerCase().endsWith('.xlsx')) {
            xlsxFound = true;

            const chunks: Uint8Array[] = [];

            entry.on('data', (chunk: Buffer) => {
              chunks.push(new Uint8Array(chunk));
            });

            entry.on('end', () => {
              // Calculate total length
              const totalLength = chunks.reduce(
                (acc, chunk) => acc + chunk.length,
                0
              );

              // Create combined buffer
              const combined = new Uint8Array(totalLength);
              let offset = 0;

              for (const chunk of chunks) {
                combined.set(chunk, offset);
                offset += chunk.length;
              }

              const xlsxBuffer = Buffer.from(combined);
              resolve(xlsxBuffer);
            });

            entry.on('error', (error) => {
              reject(error);
            });
          } else {
            entry.autodrain();
          }
        })
        .on('error', (error) => {
          reject(error);
        })
        .on('close', () => {
          if (!xlsxFound) {
            reject(new Error('No XLSX file found in the ZIP archive'));
          }
        });
    });
  } catch (error) {
    console.error('Error extracting XLSX from ZIP:', error);
    throw error;
  }
};

const convertXlsxToCsv = (xlsxBuffer: Buffer): string => {
  try {
    // Read the XLSX file from buffer
    const workbook = XLSX.read(xlsxBuffer, { type: 'buffer' });

    // Get the first worksheet
    const firstSheetName = workbook.SheetNames[0];
    const worksheet = workbook.Sheets[firstSheetName];

    // Convert to CSV
    const csvData = XLSX.utils.sheet_to_csv(worksheet);

    return csvData;
  } catch (error) {
    console.error('Error converting XLSX to CSV:', error);
    throw error;
  }
};

const run = async (event) => {
  console.info('Event:', event);
  try {
    const payload = event.body as Payload;
    console.info('Payload:', payload);

    const date = dayjs(payload.date).tz('Asia/Bangkok').format('YYYY-MM-DD');
    console.info('Formatted Date:', date);

    const reportName = `${payload.reportPrefix}.csv`;
    const s3Key = `${payload.folder}/${date}/${reportName}`;

    const zipBuffer = await downloadZipFromUrl(payload.reportURL);
    const xlsxBuffer = await extractXlsxFromZip(zipBuffer);
    const csvData = convertXlsxToCsv(xlsxBuffer);
    const csvBuffer = Buffer.from(csvData, 'utf8');

    await storeFileToS3(payload.bucket, csvBuffer, s3Key);

    return {
      body: {
        date,
        reportName,
        status: 'succeeded',
      },
    };
  } catch (error) {
    console.error('Error in lambda execution:', error);

    throw new Error(`Failed to download CME Margin Span 2: ${error.message}`);
  }
};

export const main = middyfy(run);
