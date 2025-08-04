'use strict';
module.exports = {
  up: async (queryInterface, Sequelize) => {
    return queryInterface.createTable('kbank_dd_register_result', {
      id: {
        allowNull: false,
        autoIncrement: true,
        primaryKey: true,
        type: Sequelize.INTEGER,
      },
      account_no: { type: Sequelize.STRING },
      espa_id: { type: Sequelize.STRING },
      external_reference: { type: Sequelize.STRING },
      id_matching: { type: Sequelize.STRING },
      payer_short_name: { type: Sequelize.STRING },
      return_code: { type: Sequelize.STRING },
      return_message: { type: Sequelize.STRING },
      return_status: { type: Sequelize.STRING },
      timestamp: { type: Sequelize.STRING },
      user_email_matching: { type: Sequelize.STRING },
      user_mobile_matching: { type: Sequelize.STRING },

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
    await queryInterface.dropTable('kbank_dd_register_result');
  },
};
