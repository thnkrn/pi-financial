'use strict';

module.exports = {
  up: async (queryInterface, Sequelize) => {
    return queryInterface.changeColumn('kkp_qr_generated_result', 'amount', {
      type: Sequelize.DOUBLE,
      allowNull: true,
    });
  },

  down: async (queryInterface, Sequelize) => {
    return queryInterface.changeColumn('kkp_qr_generated_result', 'amount', {
      type: Sequelize.INTEGER,
      allowNull: true,
    });
  },
};
