'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.changeColumn(
      'bloomberg_equity_closeprice',
      'px_last_eod',
      {
        type: Sequelize.DECIMAL(10, 4),
      }
    );
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.changeColumn(
      'bloomberg_equity_closeprice',
      'px_last_eod',
      {
        type: Sequelize.DECIMAL(10, 0),
      }
    );
  },
};
