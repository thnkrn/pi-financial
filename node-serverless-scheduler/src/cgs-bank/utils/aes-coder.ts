import crypto from 'crypto';
// For AES, this is always 16
import Str from '@supercharge/strings';

const ENCRYPTION_KEY = process.env.AES_SECRET_KEY; // Must be 256 bits (32 characters)';
const IV_LENGTH = 16;
export const encrypt = (
  val,
  key = ENCRYPTION_KEY,
  IV = Str.random(IV_LENGTH)
) => {
  const cipher = crypto.createCipheriv('aes-256-cbc', key, IV);
  let encrypted = cipher.update(val, 'utf8', 'base64');
  encrypted += cipher.final('base64');
  return `${IV}:${encrypted}`;
};

export const decrypt = (encrypted, key = ENCRYPTION_KEY) => {
  const decipher = crypto.createDecipheriv(
    'aes-256-cbc',
    key,
    encrypted.split(':')[0]
  );
  const decrypted = decipher.update(encrypted.split(':')[1], 'base64', 'utf8');
  return decrypted + decipher.final('utf8');
};
