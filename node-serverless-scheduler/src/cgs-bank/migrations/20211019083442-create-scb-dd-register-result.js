'use strict';
module.exports = {
  up: async (queryInterface, Sequelize) => {
    return queryInterface.createTable('scb_dd_register_result', {
      id: {
        allowNull: false,
        autoIncrement: true,
        primaryKey: true,
        type: Sequelize.INTEGER,
      },
      account_no: { type: Sequelize.STRING },
      back_url: { type: Sequelize.STRING },
      error_code: { type: Sequelize.STRING },
      ref1: { type: Sequelize.STRING },
      ref2: { type: Sequelize.STRING },
      reg_ref: { type: Sequelize.STRING },
      status_code: { type: Sequelize.STRING },
      status_desc: { type: Sequelize.STRING },

      registration_ref_code: { type: Sequelize.STRING },

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
    await queryInterface.dropTable('scb_dd_register_result');
  },
};
