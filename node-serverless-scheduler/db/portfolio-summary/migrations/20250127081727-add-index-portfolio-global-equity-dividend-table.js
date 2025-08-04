'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.addIndex(
      'portfolio_global_equity_dividend_daily_snapshot',
      ['custcode'],
      {
        name: 'idx_ge_dividend_custcode',
      }
    );
    await queryInterface.addIndex(
      'portfolio_global_equity_dividend_daily_snapshot',
      ['trading_account_no'],
      {
        name: 'idx_ge_dividend_trading_account_no',
      }
    );
    await queryInterface.addIndex(
      'portfolio_global_equity_dividend_daily_snapshot',
      ['sharecode'],
      {
        name: 'idx_ge_dividend_sharecode',
      }
    );
    await queryInterface.addIndex(
      'portfolio_global_equity_dividend_daily_snapshot',
      ['date_key', 'trading_account_no', 'sharecode', 'custcode'],
      {
        name: 'idx_ge_dividend_daily_snapshot',
      }
    );
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.removeIndex(
      'portfolio_global_equity_dividend_daily_snapshot',
      'idx_ge_dividend_custcode'
    );
    await queryInterface.removeIndex(
      'portfolio_global_equity_dividend_daily_snapshot',
      'idx_ge_dividend_trading_account_no'
    );
    await queryInterface.removeIndex(
      'portfolio_global_equity_dividend_daily_snapshot',
      'idx_ge_dividend_sharecode'
    );
    await queryInterface.removeIndex(
      'portfolio_global_equity_dividend_daily_snapshot',
      'idx_ge_dividend_daily_snapshot'
    );
  },
};
