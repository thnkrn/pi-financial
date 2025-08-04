'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    const transaction = await queryInterface.sequelize.transaction();
    try {
      await queryInterface.addIndex(
        'portfolio_global_equity_dividend_daily_snapshot',
        ['date_key', 'custcode'],
        {
          name: 'idx_ge_dividend_daily_snapshot_datekey_custcode',
        }
      );
      await queryInterface.createTable(
        'portfolio_mutual_fund_dividend_daily_transaction',
        {
          id: {
            type: Sequelize.UUID,
            defaultValue: Sequelize.UUIDV4,
            primaryKey: true,
            allowNull: false,
          },
          date_key: {
            allowNull: false,
            type: Sequelize.DATEONLY,
          },
          custcode: {
            allowNull: false,
            type: Sequelize.STRING(10),
          },
          trading_account_no: {
            allowNull: false,
            type: Sequelize.STRING(20),
          },
          fund_code: {
            allowNull: false,
            type:
              Sequelize.STRING(50) +
              " CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_bin'",
          },

          amccode: {
            type: Sequelize.STRING(50),
          },
          payment_date: {
            type: Sequelize.DATEONLY,
            allowNull: false,
          },
          book_closed_date: {
            type: Sequelize.DATEONLY,
            allowNull: false,
          },
          unit: {
            type: Sequelize.DECIMAL(19, 9),
            allowNull: false,
          },
          dividend_rate: {
            type: Sequelize.DECIMAL(19, 9),
            allowNull: false,
          },
          dividend_amount: {
            type: Sequelize.DECIMAL(19, 9),
            allowNull: false,
          },
          witholding_tax: {
            type: Sequelize.DECIMAL(19, 9),
          },
          dividend_amount_net: {
            type: Sequelize.DECIMAL(19, 9),
          },
          payment_type_description: {
            type: Sequelize.STRING(50),
          },
          bank_name: {
            type: Sequelize.STRING(50),
          },
          bank_account: {
            type: Sequelize.STRING(20),
          },
          created_at: {
            type: Sequelize.DATE,
            allowNull: false,
          },
        }
      );
      await queryInterface.addIndex(
        'portfolio_mutual_fund_dividend_daily_transaction',
        [
          'date_key',
          'trading_account_no',
          'fund_code',
          'payment_date',
          'bank_account',
        ],
        {
          name: 'idx_mf_dividend_daily_tx_unique',
          unique: true,
        }
      );

      await queryInterface.addIndex(
        'portfolio_mutual_fund_dividend_daily_transaction',
        ['custcode', 'date_key'],
        {
          name: 'idx_mf_dividend_daily_tx_custcode_datekey',
        }
      );
      await transaction.commit();
    } catch (error) {
      await transaction.rollback();
      throw error;
    }
  },

  async down(queryInterface, Sequelize) {
    const transaction = await queryInterface.sequelize.transaction();
    try {
      await queryInterface.removeIndex(
        'portfolio_global_equity_dividend_daily_snapshot',
        'idx_ge_dividend_daily_snapshot_datekey_custcode'
      );
      await queryInterface.dropTable(
        'portfolio_mutual_fund_dividend_daily_transaction'
      );
      await transaction.commit();
    } catch (error) {
      await transaction.rollback();
      throw error;
    }
  },
};
