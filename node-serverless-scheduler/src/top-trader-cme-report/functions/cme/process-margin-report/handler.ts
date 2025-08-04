import { middyfy } from '@libs/lambda';
import { getFileBufferFromS3, storeFileToS3 } from '@libs/s3-utils';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import * as Papa from 'papaparse';

// Extend dayjs with timezone support
dayjs.extend(utc);
dayjs.extend(timezone);

interface Payload {
  bucket: string;
  reportPrefix: string;
  date: string;
  span1ReportName: string;
  span2ReportName: string;
  folder: string;
}

interface Span1Record {
  'Asset Class': string;
  'Product Name': string;
  'Product Code': string;
  'Start Period': string;
  'End Period': string;
  Maintenance: string;
  Currency: string;
}

interface Span2Record {
  'ASSET CLASS': string;
  PRODUCT: string;
  'PRODUCT CODE': string;
  PERIOD: string;
  'MAINTENANCE LONG': string;
  'MAINTENANCE SHORT': string;
}

interface FinalRecord {
  asset_class: string;
  product_name: string;
  product_code: string;
  period: string;
  margin: string;
  currency: string;
}

const parseCSVFromBuffer = (buffer: Buffer): unknown[] => {
  try {
    const csvString = buffer.toString('utf8');
    const parsed = Papa.parse(csvString, {
      header: true,
      skipEmptyLines: true,
      dynamicTyping: false, // Keep as strings to handle data properly
      transformHeader: (header: string) => header.trim(), // Remove whitespace from headers
    });

    if (parsed.errors.length > 0) {
      console.warn('CSV parsing warnings:', parsed.errors);
    }

    return parsed.data;
  } catch (error) {
    console.error('Error parsing CSV:', error);
    throw error;
  }
};

const getHigherMaintenanceValue = (
  maintenanceLong: string,
  maintenanceShort: string
): string => {
  // Parse values, handling potential non-numeric data
  const longValue = parseFloat(maintenanceLong?.replace(/[,$]/g, '') || '0');
  const shortValue = parseFloat(maintenanceShort?.replace(/[,$]/g, '') || '0');

  // Return the higher value as string, preserving original format if possible
  if (longValue >= shortValue) {
    return maintenanceLong || '';
  }

  return maintenanceShort || '';
};

function parseMonthYear(periodStr: string): { month: number; year: number } {
  const parts = periodStr.split('/');

  if (parts.length === 2) {
    // Format: mm/yyyy
    return {
      month: parseInt(parts[0], 10),
      year: parseInt(parts[1], 10),
    };
  } else if (parts.length === 3) {
    // Format: mm/dd/yyyy - ignore the day part
    return {
      month: parseInt(parts[0], 10),
      year: parseInt(parts[2], 10),
    };
  } else {
    throw new Error(`Invalid period format: ${periodStr}`);
  }
}

function formatPeriod(month: number, year: number): string {
  const monthStr = month.toString().padStart(2, '0');
  return `${monthStr}/${year}`;
}

function generateMonthlyPeriods(
  startPeriod: string,
  endPeriod: string
): string[] {
  const start = parseMonthYear(startPeriod);
  const end = parseMonthYear(endPeriod);

  const periods: string[] = [];
  let currentMonth = start.month;
  let currentYear = start.year;

  while (
    currentYear < end.year ||
    (currentYear === end.year && currentMonth <= end.month)
  ) {
    periods.push(formatPeriod(currentMonth, currentYear));

    currentMonth++;
    if (currentMonth > 12) {
      currentMonth = 1;
      currentYear++;
    }
  }

  return periods;
}

const processFinalReport = (
  span1Data: Span1Record[],
  span2Data: Span2Record[]
): FinalRecord[] => {
  const finalRecord: FinalRecord[] = [];

  for (const span2Record of span2Data) {
    if (!span2Record['PRODUCT CODE']) continue;

    const highestMaintenance = getHigherMaintenanceValue(
      span2Record['MAINTENANCE LONG'],
      span2Record['MAINTENANCE SHORT']
    );

    const match = highestMaintenance.match(/([\d,.]+)\s*([A-Z]+)/);

    let margin = '';
    let currency = '';

    if (match) {
      margin = match[1].replace(/,/g, '');
      currency = match[2];
    }

    finalRecord.push({
      asset_class: span2Record['ASSET CLASS'] || '',
      product_name: span2Record['PRODUCT'] || '',
      product_code: span2Record['PRODUCT CODE'].trim(),
      period: span2Record['PERIOD'] || '',
      margin,
      currency,
    });
  }

  for (const span1Record of span1Data) {
    if (!span1Record['Product Code']) continue;

    const startPeriod = span1Record['Start Period'].trim();
    const endPeriod = span1Record['End Period'].trim();

    // Generate all monthly periods between start and end (inclusive)
    const periods = generateMonthlyPeriods(startPeriod, endPeriod);

    // Create a record for each period
    for (const period of periods) {
      finalRecord.push({
        asset_class: span1Record['Asset Class'],
        product_name: span1Record['Product Name'],
        product_code: span1Record['Product Code'].trim(),
        period: period,
        margin: span1Record['Maintenance'] || '',
        currency: span1Record['Currency'] || '',
      });
    }
  }

  return finalRecord;
};

const convertToCsv = (data: FinalRecord[]): string => {
  const csv = Papa.unparse(data, {
    header: true,
    columns: [
      'asset_class',
      'product_name',
      'product_code',
      'period',
      'margin',
      'currency',
    ],
  });
  return csv;
};

const run = async (event) => {
  console.info('Event:', event);

  try {
    const payload = event as Payload;
    console.info('Payload:', payload);

    // Validate payload
    if (!payload.bucket || !payload.date) {
      throw new Error('Missing required fields: bucket or date');
    }

    if (!payload.span1ReportName || !payload.span2ReportName) {
      throw new Error('Cannot find span report name');
    }

    // Format date
    const date = dayjs(payload.date).tz('Asia/Bangkok').format('YYYY-MM-DD');
    console.info('Formatted Date:', date);

    // Extract S3 keys from previous results
    const span1S3Key = `${payload.folder}/${date}/${payload.span1ReportName}`;
    const span2S3Key = `${payload.folder}/${date}/${payload.span2ReportName}`;

    // Read both CSV files from S3
    const [span1Buffer, span2Buffer] = await Promise.all([
      getFileBufferFromS3(payload.bucket, span1S3Key),
      getFileBufferFromS3(payload.bucket, span2S3Key),
    ]);

    if (!span1Buffer || !span2Buffer) {
      throw new Error('Failed to read buffer from CSV file');
    }

    // Parse CSV data
    const span1Data = parseCSVFromBuffer(span1Buffer) as Span1Record[];
    const span2Data = parseCSVFromBuffer(span2Buffer) as Span2Record[];

    // Combine the reports
    const finalReport = processFinalReport(span1Data, span2Data);

    // Convert to CSV
    const csvData = convertToCsv(finalReport);
    const csvBuffer = Buffer.from(csvData, 'utf8');

    // Create combined report name
    const reportName = `${payload.reportPrefix}.csv`;
    const s3Key = `${payload.folder}/${date}/${reportName}`;

    // Upload combined report to S3
    await storeFileToS3(payload.bucket, csvBuffer, s3Key);

    return {
      body: {
        reportName,
        status: 'succeeded',
      },
    };
  } catch (error) {
    console.error('Error combining margin reports:', error);

    throw new Error(`Error combining margin reports: ${error.message}`);
  }
};

export const main = middyfy(run);
