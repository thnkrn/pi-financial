'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.addColumn('portfolio_summary_daily_snapshot', 'y_0', {
      type: Sequelize.DECIMAL(65, 8),
    });
    await queryInterface.addColumn('portfolio_summary_daily_snapshot', 'm_0', {
      type: Sequelize.DECIMAL(65, 8),
    });
    await queryInterface.addColumn(
      'portfolio_summary_daily_snapshot',
      'as_of_date',
      {
        type: Sequelize.DATEONLY,
      }
    );
    await queryInterface.addColumn(
      'portfolio_summary_daily_snapshot',
      'exchange_rate_as_of',
      {
        type: Sequelize.DATEONLY,
      }
    );
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.removeColumn(
      'portfolio_summary_daily_snapshot',
      'y_0'
    );
    await queryInterface.removeColumn(
      'portfolio_summary_daily_snapshot',
      'm_0'
    );
    await queryInterface.removeColumn(
      'portfolio_summary_daily_snapshot',
      'as_of_date'
    );
    await queryInterface.removeColumn(
      'portfolio_summary_daily_snapshot',
      'exchange_rate_as_of'
    );
  },
};
