'use strict';

const { DataTypes } = require('sequelize');
module.exports = {
  up: async (queryInterface, Sequelize) => {
    return queryInterface.createTable('kkp_qr_generated_result', {
      id: {
        allowNull: false,
        autoIncrement: true,
        primaryKey: true,
        type: Sequelize.INTEGER,
      },

      channel_code: { type: Sequelize.STRING },
      service_name: { type: Sequelize.STRING },
      system_code: { type: Sequelize.STRING },
      transaction_date_time: { type: Sequelize.STRING },
      transaction_id: { type: Sequelize.STRING },
      amount: { type: Sequelize.INTEGER },
      transaction_no: { type: Sequelize.STRING },
      transaction_ref_code: { type: Sequelize.STRING },
      customer_code: { type: Sequelize.STRING },
      product: { type: Sequelize.STRING },
      bill_payment_biller_id: { type: Sequelize.STRING },
      bill_payment_reference1: { type: Sequelize.STRING },
      bill_payment_reference2: { type: Sequelize.STRING },
      bill_payment_reference3: { type: Sequelize.STRING },
      bill_payment_suffix: { type: Sequelize.STRING },
      bill_payment_tax_id: { type: Sequelize.STRING },
      credit_transfer_bank_account: { type: Sequelize.STRING },
      credit_transfer_e_wallet_id: { type: Sequelize.STRING },
      credit_transfer_mobile_number: { type: Sequelize.STRING },
      credit_transfer_tax_id: { type: Sequelize.STRING },
      transaction_amount: { type: Sequelize.STRING },

      format: { type: Sequelize.STRING },
      qr_value: { type: Sequelize.STRING },

      response_code: { type: Sequelize.STRING },
      response_message: { type: Sequelize.STRING },

      // Error
      code: { type: Sequelize.STRING },
      description: { type: Sequelize.STRING },
      message: { type: Sequelize.STRING },

      created_at: {
        allowNull: false,
        type: Sequelize.DATE,
      },
      updated_at: {
        allowNull: false,
        type: Sequelize.DATE,
      },
    });
  },

  down: async (queryInterface, Sequelize) => {
    await queryInterface.dropTable('kkp_qr_generated_result');
  },
};
