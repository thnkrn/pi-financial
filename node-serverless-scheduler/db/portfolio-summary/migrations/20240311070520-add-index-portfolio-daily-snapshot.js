'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  up: async (queryInterface) => {
    await queryInterface.addIndex('portfolio_bond_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.addIndex('portfolio_bond_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.addIndex('portfolio_bond_daily_snapshot', [
      'date_key',
      'custcode',
    ]);

    await queryInterface.addIndex('portfolio_cash_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.addIndex('portfolio_cash_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.addIndex('portfolio_cash_daily_snapshot', [
      'date_key',
      'custcode',
    ]);

    await queryInterface.addIndex('portfolio_global_equity_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.addIndex('portfolio_global_equity_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.addIndex('portfolio_global_equity_daily_snapshot', [
      'date_key',
      'custcode',
    ]);

    await queryInterface.addIndex(
      'portfolio_global_equity_otc_daily_snapshot',
      ['custcode']
    );
    await queryInterface.addIndex(
      'portfolio_global_equity_otc_daily_snapshot',
      ['date_key']
    );
    await queryInterface.addIndex(
      'portfolio_global_equity_otc_daily_snapshot',
      ['date_key', 'custcode']
    );

    await queryInterface.addIndex('portfolio_mutual_fund_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.addIndex('portfolio_mutual_fund_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.addIndex('portfolio_mutual_fund_daily_snapshot', [
      'date_key',
      'custcode',
    ]);

    await queryInterface.addIndex(
      'portfolio_structured_product_daily_snapshot',
      ['custcode']
    );
    await queryInterface.addIndex(
      'portfolio_structured_product_daily_snapshot',
      ['date_key']
    );
    await queryInterface.addIndex(
      'portfolio_structured_product_daily_snapshot',
      ['date_key', 'custcode']
    );

    await queryInterface.addIndex('portfolio_summary_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.addIndex('portfolio_summary_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.addIndex('portfolio_summary_daily_snapshot', [
      'date_key',
      'custcode',
      'mktid',
    ]);

    await queryInterface.addIndex('portfolio_tfex_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.addIndex('portfolio_tfex_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.addIndex('portfolio_tfex_daily_snapshot', [
      'date_key',
      'custcode',
    ]);

    await queryInterface.addIndex('portfolio_tfex_summary_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.addIndex('portfolio_tfex_summary_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.addIndex('portfolio_tfex_summary_daily_snapshot', [
      'date_key',
      'custcode',
    ]);

    await queryInterface.addIndex('portfolio_thai_equity_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.addIndex('portfolio_thai_equity_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.addIndex('portfolio_thai_equity_daily_snapshot', [
      'date_key',
      'custcode',
    ]);
  },
  down: async (queryInterface) => {
    await queryInterface.removeIndex('portfolio_bond_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.removeIndex('portfolio_bond_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.removeIndex('portfolio_bond_daily_snapshot', [
      'date_key',
      'custcode',
    ]);

    await queryInterface.removeIndex('portfolio_cash_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.removeIndex('portfolio_cash_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.removeIndex('portfolio_cash_daily_snapshot', [
      'date_key',
      'custcode',
    ]);

    await queryInterface.removeIndex('portfolio_global_equity_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.removeIndex('portfolio_global_equity_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.removeIndex('portfolio_global_equity_daily_snapshot', [
      'date_key',
      'custcode',
    ]);

    await queryInterface.removeIndex(
      'portfolio_global_equity_otc_daily_snapshot',
      ['custcode']
    );
    await queryInterface.removeIndex(
      'portfolio_global_equity_otc_daily_snapshot',
      ['date_key']
    );
    await queryInterface.removeIndex(
      'portfolio_global_equity_otc_daily_snapshot',
      ['date_key', 'custcode']
    );

    await queryInterface.removeIndex('portfolio_mutual_fund_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.removeIndex('portfolio_mutual_fund_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.removeIndex('portfolio_mutual_fund_daily_snapshot', [
      'date_key',
      'custcode',
    ]);

    await queryInterface.removeIndex(
      'portfolio_structured_product_daily_snapshot',
      ['custcode']
    );
    await queryInterface.removeIndex(
      'portfolio_structured_product_daily_snapshot',
      ['date_key']
    );
    await queryInterface.removeIndex(
      'portfolio_structured_product_daily_snapshot',
      ['date_key', 'custcode']
    );

    await queryInterface.removeIndex('portfolio_summary_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.removeIndex('portfolio_summary_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.removeIndex('portfolio_summary_daily_snapshot', [
      'date_key',
      'custcode',
      'mktid',
    ]);

    await queryInterface.removeIndex('portfolio_tfex_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.removeIndex('portfolio_tfex_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.removeIndex('portfolio_tfex_daily_snapshot', [
      'date_key',
      'custcode',
    ]);

    await queryInterface.removeIndex('portfolio_tfex_summary_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.removeIndex('portfolio_tfex_summary_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.removeIndex('portfolio_tfex_summary_daily_snapshot', [
      'date_key',
      'custcode',
    ]);

    await queryInterface.removeIndex('portfolio_thai_equity_daily_snapshot', [
      'custcode',
    ]);
    await queryInterface.removeIndex('portfolio_thai_equity_daily_snapshot', [
      'date_key',
    ]);
    await queryInterface.removeIndex('portfolio_thai_equity_daily_snapshot', [
      'date_key',
      'custcode',
    ]);
  },
};
