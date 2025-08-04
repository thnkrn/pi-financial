import { SecretManagerType } from '@libs/db-utils';
import { middyfy } from '@libs/lambda';
import { getObject, storeFileToS3 } from '@libs/s3-utils';
import { getAccessibility, getSecretValue } from '@libs/secrets-manager-client';
import console from 'console';
import { format, subDays } from 'date-fns';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import got from 'got';
import Papa from 'papaparse';
import { Readable } from 'stream';
import { loginCme } from '../login-cme/handler';

dayjs.extend(utc);
dayjs.extend(timezone);

interface Payload {
  token: string;
  bucket: string;
  outputDir: string;
  holidayFileKey: string;
  products: {
    productCode: string;
    productGuidInt: string;
  }[];
  date: string;
}

interface ApiResponse {
  _embedded: {
    instruments: Instrument[];
  };
  _metadata: {
    size: number;
    totalElements: number;
    totalPages: number;
    number: number;
    type: string;
  };
}

interface Instrument {
  globexSymbol: string;
  productGuidInt: string;
  firstTradeDate: string;
  firstNoticeDate: string;
  lastTradeDate: string;
  contractMonth: string;
}

interface Result {
  PRODUCT_CODE: string; // "GC", Product_Code
  PERIOD: string; // "04/2025"
  SYMBOL: string; // "GCJ25"  Symbol (instrument endpoint)
  FIRST_TRADE_DATETIME: string; // "23-05-2023-00:00:00",
  LAST_TRADE_DATETIME: string; // "28-04-2025-03:30:00",
}

interface HolidayPayload {
  Holiday: string;
  Date: string;
}

const ResultFilePrefix = 'MT5_margin_symbol_cme';

const getInstrumentDetail = async (
  url: string,
  token: string,
  productCode: string,
  productGuidInt: string,
  holidays: string[],
  page = 0
): Promise<Result[]> => {
  try {
    const response = await got
      .get(`${url}/refdata/v3/instruments`, {
        searchParams: {
          instrumentType: 'FUT',
          productGuidInt: productGuidInt,
          page: page,
          size: 1000,
        },
        headers: {
          Authorization: `Bearer ${token}`,
        },
        responseType: 'json',
      })
      .json<ApiResponse>();

    console.info(page);
    console.info(response);

    if (response._metadata.totalElements <= 0 || !response._embedded) {
      return [];
    }

    const result: Result[] = [];
    for (const instrument of response._embedded.instruments) {
      if (!filterInstrument(instrument, productCode)) {
        continue;
      }

      result.push(generateResult(productCode, instrument, holidays));
    }

    if (page < response._metadata.totalPages) {
      const nested = await getInstrumentDetail(
        url,
        token,
        productCode,
        productGuidInt,
        holidays,
        page + 1
      );

      return [...result, ...nested];
    }

    return result;
  } catch (error: any) {
    if (error.response?.statusCode === 401) {
      const newLogin = await loginCme;
      return await getInstrumentDetail(
        url,
        newLogin.access_token,
        productCode,
        productGuidInt,
        holidays,
        page
      );
    } else {
      console.error(error);
      throw error;
    }
  }
};

const filterInstrument = (
  instrument: Instrument,
  productCode: string
): boolean => {
  let monthDiff = 12;
  if (['ZC', 'ZS', 'ZM'].includes(productCode)) {
    monthDiff = 24;
  }

  const diff = getMonthsDifference(instrument.contractMonth);

  return diff <= monthDiff;
};

const getMonthsDifference = (dateString: string): number => {
  // Parse the input string
  const year = parseInt(dateString.substring(0, 4));
  const month = parseInt(dateString.substring(4, 6));

  // Validate month range
  if (month < 1 || month > 12) {
    throw new Error('Invalid month. Month must be between 01 and 12');
  }

  const currentDate = new Date();
  const currentYear = currentDate.getFullYear();
  const currentMonth = currentDate.getMonth() + 1; // getMonth() returns 0-11

  return (year - currentYear) * 12 + (month - currentMonth);
};

const generateResult = (
  productCode: string,
  instrument: Instrument,
  holidays: string[]
): Result => {
  const year = instrument.contractMonth.substring(0, 4);
  const month = instrument.contractMonth.substring(4, 6);
  const firstTradeDate = convertDateFormat(instrument.firstTradeDate);
  const lastTradeDate = findLastTradeDate(
    new Date(instrument.firstNoticeDate ?? instrument.lastTradeDate),
    holidays,
    3
  );

  return {
    FIRST_TRADE_DATETIME: `${firstTradeDate}-00:00:00`,
    LAST_TRADE_DATETIME: `${format(lastTradeDate, 'dd-MM-yyyy')}-17:00:00`,
    PERIOD: `${month}/${year}`,
    PRODUCT_CODE: productCode,
    SYMBOL: instrument.globexSymbol,
  };
};

function findLastTradeDate(
  date: Date,
  holidays: string[],
  workingDays: number
): Date {
  const dateStr = format(date, 'dd/MM/yyyy');
  const day = date.getDay();

  // Valida holiday and weekend
  if (holidays.includes(dateStr) || day === 0 || day === 6) {
    return findLastTradeDate(subDays(date, 1), holidays, workingDays);
  }

  workingDays -= 1;
  if (workingDays != 0) {
    return findLastTradeDate(subDays(date, 1), holidays, workingDays);
  }

  return date;
}

function convertDateFormat(input: string): string {
  const [year, month, day] = input.split('-');
  return `${day}-${month}-${year}`;
}

async function generateFileAndStoreS3(
  bucket: string,
  dir: string,
  data: Result[],
  date: string
) {
  const buffer = Buffer.from(JSON.stringify(data, null, 2), 'utf-8');
  const fileDate = dayjs(date).tz('Asia/Bangkok').format('YYYYMMDD');
  const fileName = `${ResultFilePrefix}_${fileDate}.json`;
  const fileKey = `${dir}/${fileName}`;

  await storeFileToS3(bucket, buffer, fileKey);

  return {
    bucket: bucket,
    fileKey: fileKey,
  };
}

async function getHolidays(
  bucketName: string,
  fileKey: string
): Promise<string[]> {
  const data = await getObject(bucketName, fileKey);
  if (!data.Body) {
    throw new Error(`File content is null: ${fileKey}`);
  }

  if (data.Body instanceof Readable) {
    const raw = await streamToString(data.Body);

    const result = Papa.parse<HolidayPayload>(raw, {
      header: true,
      skipEmptyLines: true,
    });

    return result.data.map((payload) => payload.Date);
  }

  return [];
}

async function streamToString(stream: Readable): Promise<string> {
  return await new Promise((resolve, reject) => {
    const chunks: Buffer[] = [];
    stream.on('data', (chunk) => chunks.push(Buffer.from(chunk)));
    stream.on('error', (err) => reject(err));
    stream.on('end', () => resolve(Buffer.concat(chunks).toString('utf-8')));
  });
}

const run = async (event) => {
  console.info(event);
  const payload = event as Payload;
  if (!payload.products) {
    throw new Error("Missing 'productGuidIntList' in input");
  }

  const cmeSecret = await getSecretValue(
    'cme-marketdata',
    getAccessibility(SecretManagerType.Secret)
  );
  const CME_REF_API_HOST = cmeSecret['CME_REF_API_HOST'];
  if (!CME_REF_API_HOST)
    throw new Error('CME_REF_API_HOST missing from secret');

  const holidays: string[] = await getHolidays(
    payload.bucket,
    payload.holidayFileKey
  );
  let results: Result[] = [];
  for (const product of payload.products) {
    const response = await getInstrumentDetail(
      CME_REF_API_HOST,
      payload.token,
      product.productCode,
      product.productGuidInt,
      holidays
    );

    results = [...results, ...response];
  }

  return await generateFileAndStoreS3(
    payload.bucket,
    payload.outputDir,
    results,
    payload.date
  );
};

export const execute = middyfy(run);
