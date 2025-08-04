'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.addIndex(
      'portfolio_cash_daily_snapshot',
      ['trading_account_no'],
      {
        name: 'idx_cash_trading_account_no',
      }
    );
    await queryInterface.addIndex(
      'portfolio_global_equity_daily_snapshot',
      ['trading_account_no'],
      {
        name: 'idx_ge_trading_account_no',
      }
    );
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.removeIndex(
      'portfolio_cash_daily_snapshot',
      'idx_cash_trading_account_no'
    );
    await queryInterface.removeIndex(
      'portfolio_global_equity_daily_snapshot',
      'idx_ge_trading_account_no'
    );
  },
};
