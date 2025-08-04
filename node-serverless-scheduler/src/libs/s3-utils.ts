import {
  CopyObjectCommand,
  DeleteObjectCommand,
  GetObjectCommand,
  ListObjectsCommand,
  PutObjectCommand,
  S3Client,
} from '@aws-sdk/client-s3';
import { getSignedUrl } from '@aws-sdk/s3-request-presigner';
import { Readable } from 'stream';

export async function storeFileToS3(
  bucketName: string,
  data: Buffer,
  fileName: string
) {
  const client = new S3Client({
    region: 'ap-southeast-1',
    maxAttempts: 2,
  });
  const putObjectcommand = new PutObjectCommand({
    Bucket: bucketName,
    Key: fileName,
    Body: data,
  });
  try {
    await client.send(putObjectcommand);
  } catch (e) {
    console.error('Failed to upload file to S3:', e);
    throw e;
  }
  return null;
}

export async function getPreSignedUrl(bucketName: string, fileKey: string) {
  const client = new S3Client({ region: 'ap-southeast-1' });
  const getObjectcommand = new GetObjectCommand({
    Bucket: bucketName,
    Key: fileKey,
  });

  try {
    const preSignedUrl = await getSignedUrl(client, getObjectcommand, {
      expiresIn: Number(process.env.EXPIRES_IN || 3600),
    });
    return preSignedUrl;
  } catch (e) {
    console.error('Failed to generate preSigned Url', e);
    throw e;
  }
}

export async function listObject(bucketName: string, prefix: string) {
  const client = new S3Client({ region: 'ap-southeast-1' });
  const command = new ListObjectsCommand({
    Bucket: bucketName,
    Prefix: prefix,
  });
  return await client.send(command);
}

export async function getFileBufferFromS3(
  bucketName: string,
  filename: string
): Promise<Buffer | null> {
  const client = new S3Client({ region: 'ap-southeast-1' });
  try {
    const getObjectCommand = new GetObjectCommand({
      Bucket: bucketName,
      Key: filename,
    });

    // Use the S3 client to get the file as a readable stream
    const { Body } = await client.send(getObjectCommand);

    if (Body instanceof Readable) {
      // Convert the readable stream to a buffer
      const chunks: Uint8Array[] = [];
      for await (const chunk of Body) {
        chunks.push(chunk);
      }
      return Buffer.concat(chunks);
    }

    return null;
  } catch (error) {
    console.error('Error retrieving file from S3:', error);
    return null;
  }
}

export async function moveS3File(
  sourceBucket: string,
  sourceKey: string,
  destinationBucket: string,
  destinationKey: string
) {
  const client = new S3Client({ region: 'ap-southeast-1' });
  // Copy the object from the source bucket to the destination bucket
  try {
    const copyObjectParams = {
      Bucket: destinationBucket,
      CopySource: `/${sourceBucket}/${sourceKey}`,
      Key: destinationKey,
    };

    await client.send(new CopyObjectCommand(copyObjectParams));

    // If the copy is successful, delete the original object
    const deleteObjectParams = {
      Bucket: sourceBucket,
      Key: sourceKey,
    };

    await client.send(new DeleteObjectCommand(deleteObjectParams));

    console.log(
      `File moved successfully from ${sourceBucket}/${sourceKey} to ${destinationBucket}/${destinationKey}`
    );
  } catch (error) {
    console.error('Error moving file:', error);
  }
}

/**
 * Retrieves an object from an S3 bucket.
 * @param bucketName - The name of the bucket.
 * @param fileKey - The key of the file in the bucket.
 * @returns The data of the retrieved object.
 * @throws If there is an error retrieving the object.
 */
export async function getObject(bucketName: string, fileKey: string) {
  const client = new S3Client({ region: 'ap-southeast-1' });
  const getObjectcommand = new GetObjectCommand({
    Bucket: bucketName,
    Key: fileKey,
  });

  try {
    const data = await client.send(getObjectcommand);
    return data;
  } catch (e) {
    console.error('Failed to read file from S3', e);
    throw e;
  }
}
