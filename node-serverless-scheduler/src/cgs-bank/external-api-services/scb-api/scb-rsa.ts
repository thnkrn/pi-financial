import forge from 'node-forge';
import NodeRSA from 'node-rsa';

export const encrypt = (data, publicKey: string) => {
  if (publicKey.length == 0) return '';
  const pki = forge.pki;
  const keyData =
    '-----BEGIN RSA PUBLIC KEY-----\n' +
    publicKey +
    '\n-----END RSA PUBLIC KEY-----';
  const pub = pki.publicKeyFromPem(keyData);
  const enc = pub.encrypt(data);
  return forge.util.encode64(enc);
};

export const decrypt = (data, publicKey: string) => {
  if (publicKey.length == 0) return '';
  const key = new NodeRSA();
  const keyData =
    '-----BEGIN PUBLIC KEY-----\n' + publicKey + '\n-----END PUBLIC KEY-----';
  key.importKey(keyData, 'public');
  return key.decryptPublic(data, 'utf8');
};
