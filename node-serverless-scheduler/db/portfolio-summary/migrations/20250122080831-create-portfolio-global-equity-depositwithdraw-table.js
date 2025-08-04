'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.createTable(
      'portfolio_global_equity_depositwithdraw_daily_snapshot',
      {
        id: {
          type: Sequelize.UUID,
          defaultValue: Sequelize.UUIDV4,
          primaryKey: true,
          allowNull: false,
        },
        type: {
          allowNull: false,
          type: Sequelize.STRING(100),
        },
        custcode: {
          allowNull: false,
          type: Sequelize.STRING(10),
        },
        trading_account_no: {
          allowNull: false,
          type: Sequelize.STRING(20),
        },
        currency: {
          type: Sequelize.STRING(10),
        },
        fx_rate: {
          type: Sequelize.DECIMAL(19, 9),
        },
        amount_usd: {
          type: Sequelize.DECIMAL(19, 9),
        },
        amount_thb: {
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
      'portfolio_global_equity_depositwithdraw_daily_snapshot'
    );
  },
};
