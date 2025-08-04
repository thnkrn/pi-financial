'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.createTable('structure_note_cash_movement', {
      custcode: {
        allowNull: false,
        type: Sequelize.STRING,
      },
      trading_account_no: {
        type: Sequelize.STRING,
      },
      exchange_market_id: {
        type: Sequelize.STRING,
      },
      customer_type: {
        type: Sequelize.STRING,
      },
      customer_sub_type: {
        type: Sequelize.STRING,
      },
      account_type: {
        type: Sequelize.STRING,
      },
      account_type_code: {
        type: Sequelize.STRING,
      },

      sub_account: {
        type: Sequelize.STRING,
      },
      transaction_date: {
        type: Sequelize.DATEONLY,
      },
      settlement_date: {
        type: Sequelize.DATEONLY,
      },
      transaction_type: {
        type: Sequelize.STRING,
      },
      currency: {
        type: Sequelize.STRING,
      },
      amount: {
        type: Sequelize.DECIMAL(65, 8),
      },
      note: {
        type: Sequelize.STRING,
      },
      description: {
        type: Sequelize.STRING,
      },

      date_key: {
        allowNull: false,
        type: Sequelize.DATEONLY,
      },
      created_at: {
        type: Sequelize.DATE,
      },
    });
    await queryInterface.addIndex('structure_note_cash_movement', ['custcode']);
    await queryInterface.addIndex('structure_note_cash_movement', ['date_key']);
    await queryInterface.addIndex('structure_note_cash_movement', [
      'date_key',
      'custcode',
    ]);
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.removeIndex('structure_note_cash_movement', [
      'custcode',
    ]);
    await queryInterface.removeIndex('structure_note_cash_movement', [
      'date_key',
    ]);
    await queryInterface.removeIndex('structure_note_cash_movement', [
      'date_key',
      'custcode',
    ]);
    await queryInterface.dropTable('structure_note_cash_movement');
  },
};
