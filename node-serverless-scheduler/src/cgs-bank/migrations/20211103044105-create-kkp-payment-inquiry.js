'use strict';

module.exports = {
  up: async (queryInterface, Sequelize) => {
    return queryInterface.createTable('kkp_payment_inquiry', {
      id: {
        allowNull: false,
        autoIncrement: true,
        primaryKey: true,
        type: Sequelize.INTEGER,
      },

      // DDPaymentInquiry
      amount: { type: Sequelize.INTEGER },
      external_ref_code: { type: Sequelize.STRING },
      external_ref_time: { type: Sequelize.STRING },
      transaction_no: { type: Sequelize.STRING },

      // KKP Response Header
      channel_code: { type: Sequelize.STRING },
      service_name: { type: Sequelize.STRING },
      system_code: { type: Sequelize.STRING },
      transaction_date_time: { type: Sequelize.STRING },
      transaction_id: { type: Sequelize.STRING },

      // KKP Response Status
      response_code: { type: Sequelize.STRING },
      response_message: { type: Sequelize.STRING },

      // KKP Error Status
      code: { type: Sequelize.STRING },
      description: { type: Sequelize.STRING },
      message: { type: Sequelize.STRING },

      // KKP LookupConfirmRequest
      txn_reference_no: { type: Sequelize.STRING },

      // KKP Payment Inquiry Result
      fee_amount: { type: Sequelize.INTEGER },

      status_code: { type: Sequelize.STRING },
      status_message: { type: Sequelize.STRING },

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
    await queryInterface.dropTable('kkp_payment_inquiry');
  },
};
