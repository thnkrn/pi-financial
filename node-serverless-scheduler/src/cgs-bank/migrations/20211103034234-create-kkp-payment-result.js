'use strict';

module.exports = {
  up: async (queryInterface, Sequelize) => {
    return queryInterface.createTable('kkp_payment_result', {
      id: {
        allowNull: false,
        autoIncrement: true,
        primaryKey: true,
        type: Sequelize.INTEGER,
      },

      // DDPaymentRequest
      account_no: { type: Sequelize.STRING },
      amount: { type: Sequelize.INTEGER },
      destination_bank_code: { type: Sequelize.STRING },
      transaction_no: { type: Sequelize.STRING },
      transaction_ref_code: { type: Sequelize.STRING },

      // InternalReference
      customer_code: { type: Sequelize.STRING },
      product: { type: Sequelize.STRING },

      // KKP Response HEADER
      channel_code: { type: Sequelize.STRING },
      service_name: { type: Sequelize.STRING },
      system_code: { type: Sequelize.STRING },
      transaction_date_time: { type: Sequelize.STRING },
      transaction_id: { type: Sequelize.STRING },

      response_code: { type: Sequelize.STRING },
      response_message: { type: Sequelize.STRING },

      effective_date: { type: Sequelize.STRING },
      rtp_reference_no: { type: Sequelize.STRING },
      transfer_amount: { type: Sequelize.INTEGER },

      receiving_account_no: { type: Sequelize.STRING },
      receiving_bank_code: { type: Sequelize.STRING },
      txn_reference_no: { type: Sequelize.STRING },

      fee_amount: { type: Sequelize.INTEGER },

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
    await queryInterface.dropTable('kkp_payment_result');
  },
};
