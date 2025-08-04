import * as crypto from 'crypto';
import { getConfigFromSsm } from './ssm-config';

const algorithm = 'aes-256-cbc';

const getConfig = async () => {
  const [iv, key] = await getConfigFromSsm('report', [
    'init-vector',
    'encryption-key',
  ]);

  return { iv, key };
};
export const encrypt = async (text: string) => {
  const config = await getConfig();
  const cipher = crypto.createCipheriv(
    algorithm,
    Buffer.from(config.key, 'hex'),
    Buffer.from(config.iv, 'hex')
  );
  let encryptedData = cipher.update(text, 'utf-8', 'base64');
  encryptedData += cipher.final('base64');
  return encryptedData;
};

export const decrypt = async (text: string) => {
  const config = await getConfig();
  const decipher = crypto.createDecipheriv(
    algorithm,
    Buffer.from(config.key, 'hex'),
    Buffer.from(config.iv, 'hex')
  );
  let decryptedData = decipher.update(text, 'base64', 'utf-8');
  decryptedData += decipher.final('utf8');
  return decryptedData;
};
