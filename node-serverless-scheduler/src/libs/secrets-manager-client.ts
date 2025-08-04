import {
  GetSecretValueCommand,
  SecretsManagerClient,
} from '@aws-sdk/client-secrets-manager';
import { SecretManagerType } from './db-utils';

export const getAccessibility = (
  secretManagerType: SecretManagerType
): string => {
  switch (secretManagerType) {
    case SecretManagerType.Public:
      return '/public';
    case SecretManagerType.Secret:
      return '/secret';
    default:
      return '';
  }
};

export const getSecretValue = async (key: string, accessibility: string) => {
  const env = process.env.ENVIRONMENT;
  const secretName = `pi/tech/${env}${accessibility}/function/${key}`;

  const client = new SecretsManagerClient();
  const response = await client.send(
    new GetSecretValueCommand({
      SecretId: secretName,
    })
  );
  const keyValueResponse = parseToKeyValueLookup(response.SecretString);

  return keyValueResponse;
};

interface KeyValueLookup {
  [key: string]: string;
}

function parseToKeyValueLookup(input: string): KeyValueLookup {
  // Parse the input string into a JSON object
  const jsonObject = JSON.parse(input);

  const keyValueLookup: KeyValueLookup = {};

  // Iterate over the properties of the JSON object and extract key-value pairs
  for (const key in jsonObject) {
    if (Object.prototype.hasOwnProperty.call(jsonObject, key)) {
      keyValueLookup[key] = Buffer.from(jsonObject[key], 'base64').toString(
        'utf-8'
      );
    }
  }

  return keyValueLookup;
}
