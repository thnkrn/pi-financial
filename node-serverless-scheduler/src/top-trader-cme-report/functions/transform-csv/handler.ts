import { middyfy } from '@libs/lambda';
import console from 'console';
import { getObject } from '@libs/s3-utils';
import { Readable } from 'stream';
import Papa from 'papaparse';

interface Payload {
  bucket: string;
  key: string;
}

async function streamToString(stream: Readable): Promise<string> {
  return await new Promise((resolve, reject) => {
    const chunks: Buffer[] = [];
    stream.on('data', (chunk) => chunks.push(Buffer.from(chunk)));
    stream.on('error', (err) => reject(err));
    stream.on('end', () => resolve(Buffer.concat(chunks).toString('utf-8')));
  });
}

const transformCsv = async (bucketName: string, fileKey: string) => {
  console.info(bucketName, fileKey);
  const data = await getObject(bucketName, fileKey);
  if (!data.Body) {
    throw new Error(`File content is null: ${fileKey}`);
  }

  if (data.Body instanceof Readable) {
    const raw = await streamToString(data.Body);

    const result = Papa.parse(raw, {
      header: true,
      skipEmptyLines: true,
    });
    return result.data;
  }
};

const run = async (event) => {
  console.info('event', event);
  const payload = event.body as Payload;

  if (!payload.bucket || !payload.key) {
    throw new Error("Missing 'bucket' or 'key' in input");
  }

  return await transformCsv(payload.bucket, payload.key);
};

export const main = middyfy(run);
