'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.changeColumn('exante_trades', 'quantity', {
      allowNull: true,
      type: Sequelize.DECIMAL(65, 8),
    });
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.changeColumn('exante_trades', 'quantity', {
      allowNull: true,
      type: Sequelize.INTEGER,
    });
  },
};
