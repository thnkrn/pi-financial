import {
  GetParametersCommand,
  Parameter,
  SSMClient,
} from '@aws-sdk/client-ssm';

export async function getConfigFromSsm(
  prefix: string,
  parameters: string[]
): Promise<string[]> {
  const env = process.env.ENVIRONMENT;
  const parameterPrefix = `/${env}/pi/functions/${prefix}`;
  const ssmClient = new SSMClient({ region: 'ap-southeast-1' });
  const parameterNames = parameters.map(
    (parameter) => `${parameterPrefix}/${parameter}`
  );
  const getParamsResult = await ssmClient.send(
    new GetParametersCommand({
      Names: parameterNames,
      WithDecryption: true,
    })
  );

  if (getParamsResult.InvalidParameters?.length) {
    throw new Error(
      'Failed to get values from parameter store\n' +
        JSON.stringify(getParamsResult.InvalidParameters)
    );
  }

  const results = reorderResult(parameterNames, getParamsResult.Parameters);

  return results.map((result) => result.Value);
}

function reorderResult(
  parameterList: string[],
  resultList: Parameter[]
): Parameter[] {
  const lookup: { [key: string]: Parameter } = {};
  resultList.forEach((result) => (lookup[result.Name] = result));

  return parameterList.map((parameter) => lookup[parameter]);
}
