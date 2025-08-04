'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.createTable('portfolio_exchange_rate_daily_snapshot', {
      currency: {
        primaryKey: true,
        allowNull: false,
        type: Sequelize.STRING,
      },
      exchange_rate: {
        allowNull: false,
        type: Sequelize.DECIMAL(65, 8),
      },
      date_key: {
        primaryKey: true,
        allowNull: false,
        type: Sequelize.DATEONLY,
      },
      created_at: {
        type: Sequelize.DATE,
      },
    });
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.dropTable('portfolio_exchange_rate_daily_snapshot');
  },
};
