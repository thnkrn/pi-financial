'use strict';

module.exports = {
  up: async (queryInterface, Sequelize) => {
    return [
      queryInterface.addColumn('kkp_qr_payment_result', 'biller_id', {
        type: Sequelize.STRING,
        allowNull: true,
      }),
      queryInterface.addColumn('kkp_qr_payment_result', 'account_bank', {
        type: Sequelize.STRING,
        allowNull: true,
      }),
      queryInterface.addColumn('kkp_qr_payment_result', 'account_no', {
        type: Sequelize.STRING,
        allowNull: true,
      }),
    ];
  },

  down: async (queryInterface, Sequelize) => {
    return [
      queryInterface.removeColumn('kkp_qr_payment_result', 'biller_id'),
      queryInterface.removeColumn('kkp_qr_payment_result', 'account_bank'),
      queryInterface.removeColumn('kkp_qr_payment_result', 'account_no'),
    ];
  },
};
