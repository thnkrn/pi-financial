'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.createTable('portfolio_bond_offshore_daily_snapshot', {
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
      market_type: {
        type: Sequelize.STRING,
      },
      asset_name: {
        type: Sequelize.STRING,
      },
      issuer: {
        type: Sequelize.STRING,
      },
      maturity_date: {
        type: Sequelize.DATEONLY,
      },
      initial_date: {
        type: Sequelize.DATEONLY,
      },
      next_call_date: {
        type: Sequelize.DATEONLY,
      },
      coupon_rate: {
        type: Sequelize.DECIMAL(65, 8),
      },
      units: {
        type: Sequelize.DECIMAL(65, 8),
      },
      currency: {
        type: Sequelize.STRING,
      },
      avg_cost: {
        type: Sequelize.DECIMAL(65, 8),
      },
      total_cost: {
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
    });
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.dropTable('portfolio_bond_offshore_daily_snapshot');
  },
};
