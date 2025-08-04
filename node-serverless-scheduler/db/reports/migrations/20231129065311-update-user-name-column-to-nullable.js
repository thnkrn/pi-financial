'use strict';

module.exports = {
  up: async (queryInterface, sequelize) => {
    await queryInterface.changeColumn('report_history', 'user_name', {
      type: sequelize.STRING,
      allowNull: true,
    });
  },

  down: async (queryInterface, sequelize) => {
    await queryInterface.changeColumn('report_history', 'user_name', {
      type: sequelize.STRING,
      allowNull: false,
    });
  },
};
