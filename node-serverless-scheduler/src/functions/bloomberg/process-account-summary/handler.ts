import { parseCsv, streamToPromise } from '@libs/csv-utils';
import { middyfy } from '@libs/lambda';
import { getObject } from '@libs/s3-utils';
import { defaultNull, stringToFloat } from '@libs/string-utils';
import { Instrument, SecId } from '../../../constants/bloomberg';

interface Event {
  body: Payload;
}
interface Payload {
  bucket: string;
  key: string;
}

interface AccountSummary {
  date: Date;
  account: string;
  instrument: string;
  iso: string;
  instrumentName: string;
  qty: number;
  avgPrice: number;
  price: number;
  ccy: string;
  pAndL: number;
  pAndLInEur: number;
  pAndLPercent: number;
  value: number;
  valueInEur: number;
  dailyPAndL: number;
  dailyPAndLInEur: number;
  dailyPAndLPercent: number;
  isin: string;
}

interface SecurityIdentifier {
  key: SecId;
  value: string[];
}

const hasIdentifier = (data: AccountSummary): boolean => {
  return hasIso(data) || hasIsin(data);
};

const hasIso = (data: AccountSummary): boolean => {
  return !!data.iso && data.iso.trim().length > 0;
};

const hasIsin = (data: AccountSummary): boolean => {
  return !!data.isin && data.isin.trim().length > 0;
};

/**
 * Retrieves and parses a CSV file from an S3 bucket.
 * @param event - The event containing the bucket and key of the CSV file.
 * @returns An object with an array of rows from the CSV file.
 */
const run = async (event: Event) => {
  console.info(event);
  const payload: Payload = event.body;
  console.info(payload);

  try {
    const response = await getObject(payload.bucket, payload.key);
    const csvContent = await streamToPromise(response.Body);
    const data = await parseCsv(csvContent);

    const accountSummaries: AccountSummary[] = data
      .map((row) => transformPayload(row))
      .filter(
        (formattedRow: AccountSummary) =>
          !Object.values(Instrument).includes(formattedRow.instrument)
      );
    console.info(accountSummaries);

    const kvpSecId: { [key in SecId]: string[] } = {
      [SecId.BBGLOBAL]: [],
      [SecId.ISIN]: [],
    };
    accountSummaries
      .filter((row: AccountSummary) => hasIdentifier(row))
      .forEach((row: AccountSummary) => {
        const key = hasIso(row) ? SecId.BBGLOBAL : SecId.ISIN;
        const value = hasIso(row) ? row.iso : row.isin;

        kvpSecId[key].push(value);
      });

    const securityIdentifiers: SecurityIdentifier[] = Object.keys(kvpSecId).map(
      (key) => ({
        key: key as SecId,
        value: kvpSecId[key],
      })
    );

    console.info(securityIdentifiers);

    return {
      body: {
        securityIdentifiers,
      },
    };
  } catch (error) {
    console.error('Error process account summary data:', error);
    throw error;
  }
};

/**
 * Transforms the payload event into an AccountSummary object.
 * @param event The event payload.
 * @returns The transformed AccountSummary object.
 */
const transformPayload = (event: string[]): AccountSummary => {
  return {
    date: new Date(event['Date']),
    account: defaultNull(event['Account']),
    instrument: defaultNull(event['Instrument']),
    iso: defaultNull(event['ISO']),
    instrumentName: defaultNull(event['Instrument name']),
    qty: stringToFloat(event['QTY']),
    avgPrice: stringToFloat(event['Avg Price']),
    price: stringToFloat(event['Price']),
    ccy: defaultNull(event['CCY']),
    pAndL: stringToFloat(event['P&L']),
    pAndLInEur: stringToFloat(event['P&L in EUR']),
    pAndLPercent: stringToFloat(event['P&L, %']),
    value: stringToFloat(event['Value']),
    valueInEur: stringToFloat(event['Value in EUR']),
    dailyPAndL: stringToFloat(event['Daily P&L']),
    dailyPAndLInEur: stringToFloat(event['Daily P&L in EUR']),
    dailyPAndLPercent: stringToFloat(event['Daily P&L, %']),
    isin: defaultNull(event['ISIN']),
  };
};

export const main = middyfy(run);
