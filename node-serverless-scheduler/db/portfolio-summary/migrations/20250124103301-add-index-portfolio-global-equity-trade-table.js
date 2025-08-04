'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.addIndex(
      'portfolio_global_equity_trade_daily_snapshot',
      ['custcode'],
      {
        name: 'idx_ge_trade_custcode',
      }
    );
    await queryInterface.addIndex(
      'portfolio_global_equity_trade_daily_snapshot',
      ['trading_account_no'],
      {
        name: 'idx_ge_trade_trading_account_no',
      }
    );
    await queryInterface.addIndex(
      'portfolio_global_equity_trade_daily_snapshot',
      ['sharecode'],
      {
        name: 'idx_ge_trade_sharecode_no',
      }
    );
    await queryInterface.addIndex(
      'portfolio_global_equity_trade_daily_snapshot',
      ['date_key', 'trading_account_no', 'sharecode', 'custcode'],
      {
        name: 'idx_ge_trade_daily_snapshot',
      }
    );
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.removeIndex(
      'portfolio_global_equity_trade_daily_snapshot',
      'idx_ge_trade_custcode'
    );
    await queryInterface.removeIndex(
      'portfolio_global_equity_trade_daily_snapshot',
      'idx_ge_trade_trading_account_no'
    );
    await queryInterface.removeIndex(
      'portfolio_global_equity_trade_daily_snapshot',
      'idx_ge_trade_sharecode_no'
    );
    await queryInterface.removeIndex(
      'portfolio_global_equity_trade_daily_snapshot',
      'idx_ge_trade_daily_snapshot'
    );
  },
};
