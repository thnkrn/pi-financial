'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.addIndex(
      'portfolio_global_equity_depositwithdraw_daily_snapshot',
      ['custcode'],
      {
        name: 'idx_ge_depositwithdraw_custcode',
      }
    );
    await queryInterface.addIndex(
      'portfolio_global_equity_depositwithdraw_daily_snapshot',
      ['type'],
      {
        name: 'idx_ge_depositwithdraw_type',
      }
    );
    await queryInterface.addIndex(
      'portfolio_global_equity_depositwithdraw_daily_snapshot',
      ['trading_account_no'],
      {
        name: 'idx_ge_depositwithdraw_trading_account_no',
      }
    );
    await queryInterface.addIndex(
      'portfolio_global_equity_depositwithdraw_daily_snapshot',
      ['date_key', 'trading_account_no', 'type', 'custcode'],
      {
        name: 'idx_ge_depositwithdraw_daily_snapshot',
      }
    );
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.removeIndex(
      'portfolio_global_equity_depositwithdraw_daily_snapshot',
      'idx_ge_depositwithdraw_custcode'
    );
    await queryInterface.removeIndex(
      'portfolio_global_equity_depositwithdraw_daily_snapshot',
      'idx_ge_depositwithdraw_type'
    );
    await queryInterface.removeIndex(
      'portfolio_global_equity_depositwithdraw_daily_snapshot',
      'idx_ge_depositwithdraw_trading_account_no'
    );
    await queryInterface.removeIndex(
      'portfolio_global_equity_depositwithdraw_daily_snapshot',
      'idx_ge_depositwithdraw_daily_snapshot'
    );
  },
};
