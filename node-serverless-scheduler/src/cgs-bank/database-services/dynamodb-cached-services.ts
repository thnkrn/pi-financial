import { DynamoDBClient } from '@aws-sdk/client-dynamodb';
import {
  DynamoDBDocumentClient,
  GetCommand,
  PutCommand,
} from '@aws-sdk/lib-dynamodb';
import dayjs from 'dayjs';
import { KKPAPIKeyObject } from '@cgs-bank/models/KKPAPIKeyObject';
import { nowUTC } from '@cgs-bank/utils/timestamp';

const options = { region: process.env.AWS_REGION };
const ddbClient = new DynamoDBClient(options);
const marshallOptions = {
  // Whether to automatically convert empty strings, blobs, and sets to `null`.
  convertEmptyValues: true, // false, by default.
  // Whether to remove undefined values while marshalling.
  removeUndefinedValues: true, // false, by default.
  // Whether to convert typeof object to map attribute.
  convertClassInstanceToMap: true, // false, by default.
};
const unmarshallOptions = {
  // Whether to return numbers as a string instead of converting them to native JavaScript numbers.
  wrapNumbers: true, // false, by default.
};
const translateConfig = { marshallOptions, unmarshallOptions };
const ddbDocClient = DynamoDBDocumentClient.from(ddbClient, translateConfig);

const cacheKey = process.env.KKP_API_CACHE_KEY;

export const getKKPApiToken = async (): Promise<string> => {
  const object: KKPAPIKeyObject = {
    key: cacheKey,
  };
  const params = {
    TableName: process.env.AWS_DYNAMODB_API_CACHE_TABLE_NAME,
    Key: object,
  };
  try {
    console.log('Get Object Prams');
    console.log(params);
    const result = await ddbDocClient.send(new GetCommand(params));
    const item = result.Item as KKPAPIKeyObject;
    if (dayjs(item.expiredAt).valueOf() < nowUTC().valueOf()) {
      console.error('Token is expired');
      throw new Error('Token is expired');
    }

    return item.apiKeyValue;
  } catch (e) {
    console.error('Get item from dynamodb error', e);
    throw new Error(e.message);
  }
};

export const storeKKPApiToken = async (value: string): Promise<void> => {
  console.log('Store KKP ApiToken');

  const now = nowUTC();
  const endOfDay = now.endOf('day');

  const item: KKPAPIKeyObject = {
    key: cacheKey,
    apiKeyValue: value,
    createdAt: now.toISOString(),
    expiredAt: endOfDay.toISOString(),
  };

  const params = {
    TableName: process.env.AWS_DYNAMODB_API_CACHE_TABLE_NAME,
    Item: item,
  };

  console.log('params');
  console.log(params);
  try {
    await ddbClient.send(new PutCommand(params));
  } catch (e) {
    console.error('Put item to dynamodb error', e);
    throw new Error(e.message);
  }
};
