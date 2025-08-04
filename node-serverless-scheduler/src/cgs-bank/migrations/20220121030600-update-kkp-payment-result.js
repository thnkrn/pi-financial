'use strict';

module.exports = {
  up: async (queryInterface, Sequelize) => {
    return [
      queryInterface.changeColumn('kkp_payment_result', 'amount', {
        type: Sequelize.DOUBLE,
        allowNull: true,
      }),
      queryInterface.changeColumn('kkp_payment_result', 'transfer_amount', {
        type: Sequelize.DOUBLE,
        allowNull: true,
      }),
      queryInterface.changeColumn('kkp_payment_result', 'fee_amount', {
        type: Sequelize.DOUBLE,
        allowNull: true,
      }),
    ];
  },

  down: async (queryInterface, Sequelize) => {
    return [
      queryInterface.changeColumn('kkp_payment_result', 'amount', {
        type: Sequelize.INTEGER,
        allowNull: true,
      }),
      queryInterface.changeColumn('kkp_payment_result', 'transfer_amount', {
        type: Sequelize.INTEGER,
        allowNull: true,
      }),
      queryInterface.changeColumn('kkp_payment_result', 'fee_amount', {
        type: Sequelize.INTEGER,
        allowNull: true,
      }),
    ];
  },
};
