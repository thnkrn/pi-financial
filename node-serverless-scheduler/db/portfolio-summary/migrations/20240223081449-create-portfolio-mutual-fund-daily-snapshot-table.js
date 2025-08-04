'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.createTable('portfolio_mutual_fund_daily_snapshot', {
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
      fund_category: {
        type: Sequelize.STRING,
      },
      amccode: {
        type: Sequelize.STRING,
      },
      fund_name: {
        type: Sequelize.STRING,
      },
      nav_date: {
        type: Sequelize.DATEONLY,
      },
      unit: {
        type: Sequelize.DECIMAL(65, 8),
      },
      avg_nav_cost: {
        type: Sequelize.DECIMAL(65, 8),
      },
      market_nav: {
        type: Sequelize.DECIMAL(65, 8),
      },
      total_cost: {
        type: Sequelize.DECIMAL(65, 8),
      },
      market_value: {
        type: Sequelize.DECIMAL(65, 8),
      },
      gain_loss: {
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
    await queryInterface.dropTable('portfolio_mutual_fund_daily_snapshot');
  },
};
