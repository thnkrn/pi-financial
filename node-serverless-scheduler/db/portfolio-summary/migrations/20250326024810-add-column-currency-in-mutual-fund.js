'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.addColumn(
      'portfolio_mutual_fund_daily_snapshot',
      'currency',
      {
        allowNull: true,
        type: Sequelize.STRING,
      }
    );
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.removeColumn(
      'portfolio_mutual_fund_daily_snapshot',
      'currency'
    );
  },
};
