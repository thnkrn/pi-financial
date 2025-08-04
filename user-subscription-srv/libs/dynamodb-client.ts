import { DynamoDBClient } from '@aws-sdk/client-dynamodb';
import {
  DynamoDBDocumentClient,
  GetCommand,
  ScanCommand,
  ScanCommandOutput,
} from '@aws-sdk/lib-dynamodb';
import Logger from '@utils/datadog-utils';
import { captureAWSv3Client } from 'aws-xray-sdk-core';

const { AWS_REGION, DEBUG } = process.env;

let options = { region: AWS_REGION };
if (process.env.IS_OFFLINE) {
  options = Object.assign(options, {
    endpoint: process.env.DYNAMODB_OFFLINE_ENDPOINT,
  });
}
const ddbClient = process.env.USE_AWS_XRAY
  ? captureAWSv3Client(new DynamoDBClient(options))
  : new DynamoDBClient(options);
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
  wrapNumbers: false, // false, by default.
};
const translateConfig = { marshallOptions, unmarshallOptions };
const ddbDocClient = DynamoDBDocumentClient.from(ddbClient, translateConfig);

export const scan = async (
  tableName: string,
  filterExpression: string = null,
  expressionAttributeValues = null,
  expressionAttributeNames = null,
  indexName = null,
  limit = null,
  exclusiveStartKey = null,
): Promise<ScanCommandOutput> => {
  const params = {
    TableName: tableName,
  };
  if (filterExpression !== null) {
    Object.assign(params, { FilterExpression: filterExpression });
  }
  if (expressionAttributeValues !== null) {
    Object.assign(params, {
      ExpressionAttributeValues: expressionAttributeValues,
    });
  }
  if (expressionAttributeNames !== null) {
    Object.assign(params, {
      ExpressionAttributeNames: expressionAttributeNames,
    });
  }
  if (indexName !== null) {
    Object.assign(params, { IndexName: indexName });
  }
  if (limit !== null) {
    Object.assign(params, { Limit: limit });
  }
  if (exclusiveStartKey !== null) {
    Object.assign(params, { ExclusiveStartKey: exclusiveStartKey });
  }
  try {
    const result = await ddbClient.send(new ScanCommand(params));
    if (DEBUG) {
      Logger.log(
        `Scan from dynamodb success: { params: ${JSON.stringify(
          params,
        )}, result: ${JSON.stringify(result)}}`,
      );
    }
    return result;
  } catch (error) {
    Logger.error(
      `Scan from dynamodb error: ${error.message}, { params: ${JSON.stringify(
        params,
      )} }`,
    );
    throw error;
  }
};
export const getItem = async (tableName: string, key: any): Promise<any> => {
  const params = {
    TableName: tableName,
    Key: key,
  };
  try {
    const result = await ddbDocClient.send(new GetCommand(params));
    if (DEBUG) {
      Logger.log(
        `Get item from dynamodb success: { params: ${JSON.stringify(
          params,
        )}, result: ${JSON.stringify(result)}`,
      );
    }
    return result.Item;
  } catch (error) {
    Logger.error(
      `Get item from dynamodb error:', error.message, { params: ${JSON.stringify(
        params,
      )}}`,
    );
    throw error;
  }
};
