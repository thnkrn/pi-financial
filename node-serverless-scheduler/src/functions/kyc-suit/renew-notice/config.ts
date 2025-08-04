import * as sql from 'mssql';
import { getConfigFromSsm } from '@libs/ssm-config';

export type Config = Awaited<ReturnType<typeof getConfig>>;

export async function getConfig() {
  const [
    backOfficeDbHost,
    backOfficeDbName,
    backofficeDBUsername,
    backofficeDBPassword,
    notifyPeriodSuit,
    notifyPeriodKyc,
    userServiceHost,
    requestPaginationSize,
  ] = await getConfigFromSsm('notification', [
    `backoffice-db-host`,
    `backoffice-db-name`,
    `backoffice-db-username`,
    `backoffice-db-password`,
    `notify-period-suit`,
    `notify-period-kyc`,
    `user-srv-host`,
    `notify-requests-pagination-size`,
  ]);

  return {
    backOfficeDbHost,
    backOfficeDbName,
    backofficeDBUsername,
    backofficeDBPassword,
    notifyPeriodSuit,
    notifyPeriodKyc,
    userServiceHost,
    requestPaginationSize: +requestPaginationSize,
  };
}

export function configToSqlConfig(config: Config): sql.config {
  return {
    user: config.backofficeDBUsername,
    password: config.backofficeDBPassword,
    server: config.backOfficeDbHost,
    database: config.backOfficeDbName,
    options: {
      encrypt: false,
    },
  };
}
