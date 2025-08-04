'use strict';
module.exports = {
  up: async (queryInterface, Sequelize) => {
    return queryInterface.createTable('kbank_dd_register', {
      id: {
        allowNull: false,
        autoIncrement: true,
        primaryKey: true,
        type: Sequelize.INTEGER,
      },

      citizen_id: { type: Sequelize.STRING },
      customer_code: { type: Sequelize.STRING },
      redirect_url: { type: Sequelize.STRING },
      registration_ref_code: { type: Sequelize.STRING },
      remarks: { type: Sequelize.STRING },

      // KBANKRegistrationResponse
      reg_id: { type: Sequelize.STRING },
      return_code: { type: Sequelize.STRING },
      return_message: { type: Sequelize.STRING },
      return_status: { type: Sequelize.STRING },

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
    await queryInterface.dropTable('kbank_dd_register');
  },
};
