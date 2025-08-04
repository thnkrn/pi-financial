'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.createTable('portfolio_tfex_daily_snapshot', {
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
      sharecode: {
        type: Sequelize.STRING,
      },
      multiplier: {
        type: Sequelize.DECIMAL(65, 8),
      },
      unit: {
        type: Sequelize.DECIMAL(65, 8),
      },
      avg_price: {
        type: Sequelize.DECIMAL(65, 8),
      },
      market_price: {
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
    await queryInterface.dropTable('portfolio_tfex_daily_snapshot');
  },
};
