require('dotenv').config();

module.exports = {
  username: process.env.DB_USERNAME || 'root',
  password: process.env.DB_PASSWORD || 'P@assword',
  database: process.env.DB_NAME || 'portfolio_summary_db',
  host: process.env.DB_HOST || '127.0.0.1',
  port: 3306,
  dialect: 'mysql',
  dialectOptions: {
    connectTimeout: 60000,
  },
  seederStorage: 'sequelize',
  seederStorageTableName: 'sequelizeseeds',
  logging: true,
};
