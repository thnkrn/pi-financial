'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.createTable('portfolio_summary_daily_snapshot', {
      custcode: {
        allowNull: false,
        type: Sequelize.STRING,
      },
      mktid: {
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
      y_1: {
        type: Sequelize.DECIMAL(65, 8),
      },
      y_2: {
        type: Sequelize.DECIMAL(65, 8),
      },
      y_3: {
        type: Sequelize.DECIMAL(65, 8),
      },
      m_1: {
        type: Sequelize.DECIMAL(65, 8),
      },
      m_2: {
        type: Sequelize.DECIMAL(65, 8),
      },
      m_3: {
        type: Sequelize.DECIMAL(65, 8),
      },
      m_4: {
        type: Sequelize.DECIMAL(65, 8),
      },
      m_5: {
        type: Sequelize.DECIMAL(65, 8),
      },
      m_6: {
        type: Sequelize.DECIMAL(65, 8),
      },
      m_7: {
        type: Sequelize.DECIMAL(65, 8),
      },
      m_8: {
        type: Sequelize.DECIMAL(65, 8),
      },
      m_9: {
        type: Sequelize.DECIMAL(65, 8),
      },
      m_10: {
        type: Sequelize.DECIMAL(65, 8),
      },
      m_11: {
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
    await queryInterface.dropTable('portfolio_summary_daily_snapshot');
  },
};
