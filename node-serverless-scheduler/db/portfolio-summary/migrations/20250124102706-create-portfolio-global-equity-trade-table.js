'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.createTable(
      'portfolio_global_equity_trade_daily_snapshot',
      {
        id: {
          type: Sequelize.UUID,
          defaultValue: Sequelize.UUIDV4,
          primaryKey: true,
          allowNull: false,
        },
        custcode: {
          allowNull: false,
          type: Sequelize.STRING(10),
        },
        trading_account_no: {
          allowNull: false,
          type: Sequelize.STRING(20),
        },
        exchange_market_id: {
          type: Sequelize.STRING(10),
        },
        sharecode: {
          allowNull: false,
          type: Sequelize.STRING(50),
        },
        side: {
          type: Sequelize.STRING(20),
        },
        currency: {
          type: Sequelize.STRING(10),
        },
        units: {
          type: Sequelize.DECIMAL(19, 9),
        },
        avg_price: {
          type: Sequelize.DECIMAL(19, 9),
        },
        gross_amount: {
          type: Sequelize.DECIMAL(19, 9),
        },
        commission_before_vat_usd: {
          type: Sequelize.DECIMAL(19, 9),
        },
        vat_amount: {
          type: Sequelize.DECIMAL(19, 9),
        },
        other_fees: {
          type: Sequelize.DECIMAL(19, 9),
        },
        wh_tax: {
          type: Sequelize.DECIMAL(19, 9),
        },
        net_amount: {
          type: Sequelize.DECIMAL(19, 9),
        },
        exchange_rate: {
          type: Sequelize.DECIMAL(19, 9),
        },
        net_amount_thb: {
          type: Sequelize.DECIMAL(19, 9),
        },
        date_key: {
          allowNull: false,
          type: Sequelize.DATEONLY,
        },
        created_at: {
          type: Sequelize.DATE,
        },
      }
    );
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.dropTable(
      'portfolio_global_equity_trade_daily_snapshot'
    );
  },
};
