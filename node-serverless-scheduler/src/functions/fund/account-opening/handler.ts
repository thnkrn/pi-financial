import * as sql from 'mssql';
import type { ValidatedEventAPIGatewayProxyEvent } from '@libs/api-gateway';
import schema from './schema';
import { formatJSONResponse } from '@libs/api-gateway';
import { middyfy } from '@libs/lambda';
import got from 'got';
import { SSMClient, GetParametersCommand } from '@aws-sdk/client-ssm';

const schedule = async () => {
  const date = new Date();
  date.setDate(date.getDate() - 1);
  const targetDate = date.toISOString().split('T')[0];
  const config = await getConfig();
  const customerCodes = await getCustomerCodes(
    targetDate,
    config.backOfficeDbHost,
    config.backofficeDBUsername,
    config.backofficeDBPassword
  );
  await sendOpenFundAccountRequest(customerCodes, config.fundServiceHost);
};

const _run: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  const config = await getConfig();
  const customerCodes = await getCustomerCodes(
    event.body.openingDate,
    config.backOfficeDbHost,
    config.backofficeDBUsername,
    config.backofficeDBPassword
  );
  const result = await sendOpenFundAccountRequest(
    customerCodes,
    config.fundServiceHost
  );
  return formatJSONResponse({
    message: result,
    event,
  });
};

const run = middyfy(_run);

const getCustomerCodes = async (
  openingDate: string,
  dbHost: string,
  dbUsername: string,
  dbPassword: string
) => {
  console.info(
    `Getting customer codes from backoffice with opening date ${openingDate}`
  );
  const sqlConfig: sql.config = {
    user: dbUsername,
    password: dbPassword,
    server: dbHost,
    database: 'Backoffice',
    options: {
      trustServerCertificate: true,
    },
  };
  let pool: sql.ConnectionPool;
  try {
    pool = await sql.connect(sqlConfig);
    const queryResult = await sql.query<string>(
      `SELECT custcode FROM tcas WHERE xchgmkt = 4 AND opendate = '${openingDate}'`
    );
    const customerCodes = queryResult.output.recordset.map(
      (r) => r['custcode']
    );
    console.info('Customer codes received\n' + JSON.stringify(customerCodes));
    return customerCodes;
  } catch (e) {
    console.error('Failed to get customer codes\n', +JSON.stringify(e));
    throw e;
  } finally {
    if (pool) {
      pool.close();
    }
  }
};

const sendOpenFundAccountRequest = async (
  customerCodes: string[],
  fundSrvHost: string
) => {
  try {
    const result = await got
      .post(`${fundSrvHost}/fund-accounts`, {
        json: customerCodes.map((code) => ({
          customerCode: code,
          ndid: false,
        })),
      })
      .json<{ customerCode: string; ticketId: string }[]>();
    console.info(
      'Open fund accounts request is sent.\n' + JSON.stringify(result)
    );
    return result;
  } catch (e) {
    console.error(
      'Failed to send open fund accounts request\n' +
        JSON.stringify(e.response.body)
    );
    throw e;
  }
};

const getConfig = async () => {
  const env = process.env.ENVIRONMENT;
  const parameterPrefix = `/${env}/pi/functions/fund`;
  const ssmClient = new SSMClient({ region: 'ap-southeast-1' });
  const parameterNames = [
    `${parameterPrefix}/fund-srv-host`,
    `${parameterPrefix}/backoffice-db-host`,
    `${parameterPrefix}/backoffice-db-username`,
    `${parameterPrefix}/backoffice-db-password`,
  ];
  const getParamsResult = await ssmClient.send(
    new GetParametersCommand({
      Names: parameterNames,
      WithDecryption: true,
    })
  );
  if (
    getParamsResult.InvalidParameters &&
    getParamsResult.InvalidParameters.length
  ) {
    throw new Error(
      'Failed to get values from parameter store\n' +
        JSON.stringify(getParamsResult.InvalidParameters)
    );
  }

  return {
    fundServiceHost: getParamsResult.Parameters[0].Value,
    backOfficeDbHost: getParamsResult.Parameters[1].Value,
    backofficeDBUsername: getParamsResult.Parameters[2].Value,
    backofficeDBPassword: getParamsResult.Parameters[3].Value,
  };
};

export { schedule, run };
