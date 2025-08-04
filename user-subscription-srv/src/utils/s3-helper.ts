import { GetObjectCommand, S3Client, S3ClientConfig } from '@aws-sdk/client-s3';
import { Readable } from 'stream';
import * as process from 'process';

const s3Config: S3ClientConfig = {
  region: 'ap-southeast-1',
};
const s3Client = new S3Client(s3Config);

function streamToPromise(stream: Readable): Promise<Buffer> {
  return new Promise((resolve, reject) => {
    const chunks: Buffer[] = [];
    stream.on('data', (chunk: Buffer) => chunks.push(chunk));
    stream.on('end', () => resolve(Buffer.concat(chunks)));
    stream.on('error', (error) => reject(error));
  });
}

export async function readStringFileFromS3(fileKey: string): Promise<string> {
  const params = {
    Bucket: `${process.env.USER_SUBSCRIPTION_BUCKET_NAME}`,
    Key: fileKey,
  };

  try {
    const data = await s3Client.send(new GetObjectCommand(params));
    return streamToPromise(data.Body as Readable).then((buffer) =>
      buffer.toString('utf-8'),
    );
  } catch (err) {
    throw new Error(`Error reading file ${fileKey} from S3: ${err.message}`);
  }
}

export async function readBufferFileFromS3(fileKey: string): Promise<Buffer> {
  const params = {
    Bucket: `${process.env.USER_SUBSCRIPTION_BUCKET_NAME}`,
    Key: fileKey,
  };

  try {
    const data = await s3Client.send(new GetObjectCommand(params));
    return streamToPromise(data.Body as Readable);
  } catch (err) {
    throw new Error(`Error reading file ${fileKey} from S3: ${err.message}`);
  }
}
