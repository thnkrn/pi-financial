import {
  GetParameterCommand,
  GetParameterCommandInput,
  SSMClient,
} from '@aws-sdk/client-ssm';
import Logger from '@utils/datadog-utils';

const options = { region: process.env.AWS_REGION };
const client = new SSMClient(options);

export const getParameter = async (
  key: string,
  withDecryption?: boolean,
): Promise<string | undefined> => {
  const params: GetParameterCommandInput = {
    Name: key,
    WithDecryption: withDecryption || false,
  };
  try {
    const command = new GetParameterCommand(params);
    const response = await client.send(command);
    if (response?.$metadata?.httpStatusCode !== 200) {
      Logger.error(`ErrorCode=${response?.$metadata?.httpStatusCode || 500}`);
      return undefined;
    }
    return response.Parameter?.Value;
  } catch (error) {
    const errorMessage = error?.Code || 'Failure in getParameter from ssm';
    Logger.error(
      `ErrorCode=${
        error?.$metadata?.httpStatusCode || 500
      }, Message=${errorMessage}`,
    );
    throw new Error(errorMessage);
  }
};
