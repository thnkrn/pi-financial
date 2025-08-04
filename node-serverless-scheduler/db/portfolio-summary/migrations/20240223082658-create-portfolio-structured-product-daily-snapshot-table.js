'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.createTable(
      'portfolio_structured_product_daily_snapshot',
      {
        custcode: {
          allowNull: false,
          type: Sequelize.STRING,
        },
        trading_account_no: {
          type: Sequelize.STRING,
        },
        exchange_market_id: {
          type: Sequelize.STRING,
        },
        customer_type: {
          type: Sequelize.STRING,
        },
        customer_sub_type: {
          type: Sequelize.STRING,
        },
        account_type: {
          type: Sequelize.STRING,
        },
        account_type_code: {
          type: Sequelize.STRING,
        },
        product_type: {
          type: Sequelize.STRING,
        },
        issuer: {
          type: Sequelize.STRING,
        },
        note: {
          type: Sequelize.STRING,
        },
        underlying: {
          type: Sequelize.STRING,
        },
        trade_date: {
          type: Sequelize.DATEONLY,
        },
        maturity_date: {
          type: Sequelize.DATEONLY,
        },
        tenor: {
          type: Sequelize.INTEGER,
        },
        capital_protection: {
          type: Sequelize.STRING,
        },
        yield: {
          type: Sequelize.DECIMAL(65, 8),
        },
        currency: {
          type: Sequelize.STRING,
        },
        exchange_rate: {
          type: Sequelize.DECIMAL(65, 8),
        },
        notional_value: {
          type: Sequelize.DECIMAL(65, 8),
        },
        market_value: {
          type: Sequelize.DECIMAL(65, 8),
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
      'portfolio_structured_product_daily_snapshot'
    );
  },
};
