'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.createTable(
      'portfolio_global_equity_dividend_daily_snapshot',
      {
        id: {
          type: Sequelize.UUID,
          defaultValue: Sequelize.UUIDV4,
          primaryKey: true,
          allowNull: false,
        },
        custcode: {
          allowNull: false,
          type: Sequelize.STRING(10),
        },
        trading_account_no: {
          allowNull: false,
          type: Sequelize.STRING(20),
        },
        sharecode: {
          allowNull: false,
          type: Sequelize.STRING(50),
        },
        currency: {
          type: Sequelize.STRING(10),
        },
        units: {
          type: Sequelize.DECIMAL(19, 9),
        },
        dividen_per_share: {
          type: Sequelize.DECIMAL(19, 9),
        },
        amount: {
          type: Sequelize.DECIMAL(19, 9),
        },
        tax_amount: {
          type: Sequelize.DECIMAL(19, 9),
        },
        net_amount: {
          type: Sequelize.DECIMAL(19, 9),
        },
        fx_rate: {
          type: Sequelize.DECIMAL(19, 9),
        },
        net_amount_usd: {
          type: Sequelize.DECIMAL(19, 9),
        },
        date_key: {
          allowNull: false,
          type: Sequelize.DATEONLY,
        },
        created_at: {
          type: Sequelize.DATE,
        },
      }
    );
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.dropTable(
      'portfolio_global_equity_dividend_daily_snapshot'
    );
  },
};
