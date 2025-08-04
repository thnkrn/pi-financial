'use strict';

module.exports = {
  up: async (queryInterface, Sequelize) => {
    return [
      queryInterface.changeColumn('kkp_payment_inquiry', 'amount', {
        type: Sequelize.DOUBLE,
        allowNull: true,
      }),
      queryInterface.changeColumn('kkp_payment_inquiry', 'fee_amount', {
        type: Sequelize.DOUBLE,
        allowNull: true,
      }),
    ];
  },

  down: async (queryInterface, Sequelize) => {
    return [
      queryInterface.changeColumn('kkp_payment_inquiry', 'amount', {
        type: Sequelize.INTEGER,
        allowNull: true,
      }),
      queryInterface.changeColumn('kkp_payment_inquiry', 'fee_amount', {
        type: Sequelize.INTEGER,
        allowNull: true,
      }),
    ];
  },
};
