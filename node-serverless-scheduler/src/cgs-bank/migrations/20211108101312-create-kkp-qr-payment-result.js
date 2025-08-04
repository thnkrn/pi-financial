'use strict';

module.exports = {
  up: async (queryInterface, Sequelize) => {
    return queryInterface.createTable('kkp_qr_payment_result', {
      id: {
        allowNull: false,
        autoIncrement: true,
        primaryKey: true,
        type: Sequelize.INTEGER,
      },

      biller_reference_no: { type: Sequelize.STRING },
      customer_name: { type: Sequelize.STRING },
      payment_amount: { type: Sequelize.STRING },
      payment_date: { type: Sequelize.STRING },
      payment_type: { type: Sequelize.STRING },

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
    await queryInterface.dropTable('kkp_qr_payment_result');
  },
};
