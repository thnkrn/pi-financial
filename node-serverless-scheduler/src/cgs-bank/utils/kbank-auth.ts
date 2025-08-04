import crypto from 'crypto';

export const encryptSha256 = (text) =>
  crypto.createHash('sha256').update(text).digest('hex').toUpperCase();

export function generateAuthParams(
  passPhrase: string,
  externalSystem: string,
  payeeShortName: string,
  externalReference: string
): string {
  return encryptSha256(
    `${passPhrase}${externalSystem}${payeeShortName}${externalReference}`
  );
}
