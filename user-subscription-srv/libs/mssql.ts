import sql, { type config } from 'mssql';
import * as tedious from 'tedious';

export const query = async (query: string) => {
  const sqlConfig: config = {
    server: process.env.BACKOFFICE_DB_HOST,
    database: process.env.BACKOFFICE_DB_NAME,
    user: process.env.BACKOFFICE_DB_USER,
    password: process.env.BACKOFFICE_DB_PASSWORD,
    options: { encrypt: false, trustServerCertificate: true },
  };

  const client = await sql.connect(sqlConfig);
  const result = await client.query(query);
  return result;
};

export const sequelizeConfig = async (
  host: string,
  port: string,
  user: string,
  password: string,
  database: string,
  timeout: string,
  models: any,
): Promise<any> => {
  return {
    repositoryMode: true,
    dialect: 'mssql',
    dialectModule: tedious,
    dialectOptions: {
      options: {
        requestTimeout: Number(timeout),
        encrypt: false,
        trustServerCertificate: true,
        cryptoCredentialsDetails: {
          minVersion: 'TLSv1',
        },
      },
    },
    host,
    port: Number(port),
    username: user,
    password,
    database,
    logging: process.env.DEBUG === 'true' ? console.log : false,
    models,
  };
};
