import { GetObjectCommand, S3Client } from '@aws-sdk/client-s3';
import { Upload } from '@aws-sdk/lib-storage';
import { getSignedUrl } from '@aws-sdk/s3-request-presigner';
import Logger from '@utils/datadog-utils';
import { getParameter } from './ssm';

const createClient = async () => {
  const accessKeyId = await getParameter(
    '/_aws/users/service-access/access-key-id',
  );
  const secretAccessKey = await getParameter(
    '/_aws/users/service-access/secret-access-key',
    true,
  );
  const options = {
    credentials: {
      accessKeyId,
      secretAccessKey,
    },
    region: process.env.AWS_REGION,
    signatureVersion: 'v4',
  };
  return new S3Client(options);
};

const parallelUpload = async (bucket: string, key: string, body: Buffer) => {
  Logger.log('S3 parallel upload started......');
  try {
    const options = {
      region: process.env.AWS_REGION,
      signatureVersion: 'v4',
    };
    const client = new S3Client(options);
    const target = { Bucket: bucket, Key: key, Body: body };
    const parallelUploads3 = new Upload({
      client,
      // tags: [...], // optional tags
      // queueSize: 4, // optional concurrency configuration
      // partSize: 5MB, // optional size of each part
      // leavePartsOnError: false, // optional manually handle dropped parts
      params: target,
    });
    parallelUploads3.on('httpUploadProgress', (progress) => {
      Logger.log(JSON.stringify(progress));
    });

    await parallelUploads3.done();
  } catch (e) {
    Logger.error(`S3 Error: ${e}`);
  }
};

const generateDownloadUrl = async (
  bucket: string,
  key: string,
  expiresIn = 3600,
): Promise<string> => {
  const client = await createClient();
  const getObjectParams = { Bucket: bucket, Key: key };
  const command = new GetObjectCommand(getObjectParams);
  return getSignedUrl(client, command, { expiresIn });
};

export default () => ({
  parallelUpload,
  generateObjectUrl: generateDownloadUrl,
});
