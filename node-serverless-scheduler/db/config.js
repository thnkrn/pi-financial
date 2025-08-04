require('dotenv').config();

module.exports = {
  username: process.env.DB_USERNAME || 'root',
  password: process.env.DB_PASSWORD || 'root',
  database: process.env.DB_NAME || 'report_db',
  host: process.env.DB_HOST || 'localhost',
  port: 30001,
  dialect: 'mysql',
  dialectOptions: {
    connectTimeout: 60000,
  },
  seederStorage: 'sequelize',
  seederStorageTableName: 'sequelizeseeds',
  logging: true,
};
