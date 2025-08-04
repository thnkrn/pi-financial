import * as csv from 'csv';
import csvParser from 'csv-parser';
import { EventEmitter, Readable } from 'stream';
import { formatDate } from './date-utils';

export function convertDataToBuffer(data: unknown[][] | unknown[]) {
  return new Promise<Buffer>((resolve, reject) => {
    csv.stringify(
      data,
      { delimiter: ',', bom: true },
      (e: Error, csvString: string) => {
        if (e) {
          console.error('Error while converting data to buffer:', e);
          reject(e);
        }
        console.log('Data has been converted successfully.');
        resolve(Buffer.from(csvString));
      }
    );
  });
}

export const reportHeaders = (
  reportID: string,
  reportName: string,
  purpose: string
) => [
  ['Report ID', reportID],
  ['Report Name', reportName],
  ['Purpose', purpose],
  ['Timestamp', formatDate(new Date(), 'DD/MM/YYYY HH:mm:ss')],
];

export const generateBlankColumns = () => {
  return ['', '', '', '', '', '', '', '', '', ''];
};

/**
 * Parses a CSV content and returns the parsed data as an array of arrays.
 *
 * @param csvContent The CSV content to parse.
 * @returns A Promise that resolves to the parsed data as an array of arrays.
 */
export const parseCsv = (csvContent: string): Promise<string[][]> =>
  new Promise((resolve, reject) => {
    const parsedData: string[][] = [];
    const parser = csvParser();

    parser.on('data', (data: string[]) => {
      parsedData.push(data);
    });

    parser.on('end', () => {
      resolve(parsedData);
    });

    parser.on('error', (err: Error) => {
      reject(err);
    });

    const readableStream = Readable.from([csvContent]);
    readableStream.pipe(parser);
  });

/**
 * Converts a readable stream to a promise that resolves with the stream's data as a string.
 *
 * @param {Readable} stream - The readable stream to convert.
 * @returns {Promise<string>} A promise that resolves with the stream's data as a string.
 */
export const streamToPromise = (stream: Readable): Promise<string> =>
  new Promise((resolve, reject) => {
    const chunks: Buffer[] = [];
    const eventEmitter: EventEmitter = stream as EventEmitter;

    eventEmitter.on('data', (chunk: Buffer) => chunks.push(chunk));
    eventEmitter.on('end', () =>
      resolve(Buffer.concat(chunks).toString('utf-8'))
    );
    eventEmitter.on('error', reject);
  });
