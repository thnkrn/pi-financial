import { middyfy } from '@libs/lambda';
import { getAccessibility, getSecretValue } from '@libs/secrets-manager-client';
import { SecretManagerType } from '@libs/db-utils';

const run = async () => {
  const cmeSecret = await getSecretValue(
    'cme-marketdata',
    getAccessibility(SecretManagerType.Secret)
  );

  const CME_AUTH_HOST = cmeSecret['CME_AUTH_HOST'];
  const CME_AUTH_USERNAME = cmeSecret['CME_AUTH_USERNAME'];
  const CME_AUTH_PASSWORD = cmeSecret['CME_AUTH_PASSWORD'];

  const authUrl = `${CME_AUTH_HOST.replace(/\/$/, '')}/as/token.oauth2`;
  const basicAuth = Buffer.from(
    `${CME_AUTH_USERNAME}:${CME_AUTH_PASSWORD}`
  ).toString('base64');

  const body = new URLSearchParams({
    grant_type: 'client_credentials',
  }).toString();

  const res = await fetch(authUrl, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/x-www-form-urlencoded',
      Authorization: `Basic ${basicAuth}`,
    },
    body,
  });

  return await res.json();
};

export const loginCme = run();
export const main = middyfy(run);
