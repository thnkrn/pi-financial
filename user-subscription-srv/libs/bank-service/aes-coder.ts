import { Str } from '@supercharge/strings';
import * as crypto from 'crypto';

const IV_LENGTH = 16; // For AES, this is always 16

export class AESCoder {
  private readonly aesKey: string;
  constructor(aesKey: string = process.env.AES_SECRET_KEY) {
    this.aesKey = aesKey;
  }
  encrypt(val: string, IV: string = Str.random(IV_LENGTH)) {
    console.debug(`encrypt val = ${val}`);
    const cipher = crypto.createCipheriv('aes-256-cbc', this.aesKey, IV);
    let encrypted = cipher.update(val, 'utf8', 'base64');
    encrypted += cipher.final('base64');
    return `${IV}:${encrypted}`;
  }

  decrypt(encrypted: string) {
    console.debug(`decrypt val = ${encrypted}`);
    const decipher = crypto.createDecipheriv(
      'aes-256-cbc',
      this.aesKey,
      encrypted.split(':')[0],
    );
    const decrypted = decipher.update(
      encrypted.split(':')[1],
      'base64',
      'utf8',
    );
    return decrypted + decipher.final('utf8');
  }
}
