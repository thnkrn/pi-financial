'use strict';

module.exports = {
  up: async (queryInterface, Sequelize) => {
    return [
      queryInterface.addColumn('kkp_qr_payment_result', 'transaction_status', {
        type: Sequelize.ENUM('PD', 'RV'),
        allowNull: true,
      }),
    ];
  },

  down: async (queryInterface, Sequelize) => {
    return [queryInterface.removeColumn('kkp_qr_payment_result', 'transaction_status')];
  },
};
