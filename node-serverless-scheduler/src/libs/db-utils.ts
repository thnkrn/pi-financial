import sql from 'mssql';
import { Sequelize } from 'sequelize';
import serverlessMysql from 'serverless-mysql';
import { getAccessibility, getSecretValue } from './secrets-manager-client';
import { getConfigFromSsm } from './ssm-config';

export enum DBConfigType {
  ParameterStore = 1,
  SecretManager,
  Local,
}

export enum SecretManagerType {
  Public = 1,
  Secret,
  Empty,
}

export interface SequelizePooslConfig {
  max: number;
  min: number;
  idle: number;
  evict: number;
}

async function getConfig({
  parameterName,
  dbHost,
  dbPassword,
  dbUsername,
}: {
  parameterName: string;
  dbHost: string;
  dbPassword: string;
  dbUsername: string;
}) {
  const [databaseHost, databasePassword, databaseUsername] =
    await getConfigFromSsm(parameterName, [dbHost, dbPassword, dbUsername]);

  return {
    databaseHost,
    databasePassword,
    databaseUsername,
  };
}

async function getDBConfig(
  parameterName: string,
  dbHost: string,
  dbPassword: string,
  dbUsername: string,
  dbConfigType: DBConfigType,
  secretManagerType: SecretManagerType
) {
  switch (dbConfigType) {
    case DBConfigType.Local:
      return {
        databaseHost: '127.0.0.1',
        databasePassword: 'P@assword',
        databaseUsername: 'root',
      };
    case DBConfigType.SecretManager: {
      const secret = await getSecretValue(
        parameterName,
        getAccessibility(secretManagerType)
      );
      return {
        databaseHost: secret[dbHost],
        databasePassword: secret[dbPassword],
        databaseUsername: secret[dbUsername],
      };
    }
    default:
      return await getConfig({
        parameterName: parameterName,
        dbHost: dbHost,
        dbPassword: dbPassword,
        dbUsername: dbUsername,
      });
  }
}

export const getSequelize = async ({
  parameterName,
  dbHost,
  dbPassword,
  dbUsername,
  dbName,
  dbConfigType = DBConfigType.ParameterStore,
  secretManagerType = SecretManagerType.Empty,
  pool,
}: {
  parameterName: string;
  dbHost: string;
  dbPassword: string;
  dbUsername: string;
  dbName: string;
  dbConfigType?: DBConfigType;
  secretManagerType?: SecretManagerType;
  pool?: SequelizePooslConfig;
}) => {
  const config = await getDBConfig(
    parameterName,
    dbHost,
    dbPassword,
    dbUsername,
    dbConfigType,
    secretManagerType
  );
  return new Sequelize({
    username: config.databaseUsername,
    password: config.databasePassword,
    database: dbName,
    host: config.databaseHost,
    port: 3306,
    dialect: 'mysql',
    dialectModule: require('mysql2'),
    pool: {
      acquire: 180000,
      idle: 10000,
    },
    dialectOptions: {
      connectTimeout: 180000,
    },
    ...(pool && { pool }),
  });
};

export const getmySqlClient = async ({
  parameterName,
  dbHost,
  dbPassword,
  dbUsername,
  dbName,
  dbConfigType = DBConfigType.ParameterStore,
  secretManagerType = SecretManagerType.Empty,
}: {
  parameterName: string;
  dbHost: string;
  dbPassword: string;
  dbUsername: string;
  dbName: string;
  dbConfigType?: DBConfigType;
  secretManagerType?: SecretManagerType;
}) => {
  const config = await getDBConfig(
    parameterName,
    dbHost,
    dbPassword,
    dbUsername,
    dbConfigType,
    secretManagerType
  );

  return serverlessMysql({
    config: {
      user: config.databaseUsername,
      password: config.databasePassword,
      host: config.databaseHost,
      port: 3306,
      database: dbName,
    },
  });
};

export const getMssqlConnection = async ({
  parameterName,
  dbHost,
  dbPassword,
  dbUsername,
  dbName,
  dbConfigType = DBConfigType.SecretManager,
  secretManagerType = SecretManagerType.Secret,
  dbPort = 1433,
}: {
  parameterName: string;
  dbHost: string;
  dbPassword: string;
  dbUsername: string;
  dbName: string;
  dbConfigType?: DBConfigType;
  secretManagerType?: SecretManagerType;
  dbPort?: number;
}) => {
  const config = await getDBConfig(
    parameterName,
    dbHost,
    dbPassword,
    dbUsername,
    dbConfigType,
    secretManagerType
  );

  return await sql.connect({
    user: config.databaseUsername,
    password: config.databasePassword,
    server: config.databaseHost,
    database: dbName,
    port: dbPort,
    options: {
      encrypt: false,
      trustServerCertificate: true,
    },
  });
};
