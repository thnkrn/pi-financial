import { Sequelize } from 'sequelize';
import mysql from 'mysql2';
import { getAccessibility, getSecretValue } from '@libs/secrets-manager-client';
import { SecretManagerType } from '@libs/db-utils';

const dbName = 'cgs_bank_db';
const dbUser = 'cgs_bank_srv';
const dbPort = '3306';

const mySqlDbConnection = new Sequelize(dbName, dbUser, '', {
  port: Number(dbPort),
  dialect: 'mysql',
  dialectModule: mysql,
  hooks: {
    beforeConnect: async (config) => {
      const databaseSecret = await getSecretValue(
        'cgs/bank-services/database',
        getAccessibility(SecretManagerType.Public)
      );
      config.database = databaseSecret['MYSQL_DB_NAME'];
      config.username = databaseSecret['MYSQL_DB_USER'];
      config.host = databaseSecret['MYSQL_DB_HOST'];
      config.password = databaseSecret['MYSQL_DB_PASSWORD'];
    },
  },
});

export default mySqlDbConnection;
