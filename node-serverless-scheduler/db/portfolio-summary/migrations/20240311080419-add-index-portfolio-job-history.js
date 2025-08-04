'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.addIndex('portfolio_job_history', ['custcode']);
    await queryInterface.addIndex('portfolio_job_history', [
      'custcode',
      'marketing_id',
    ]);
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.removeIndex('portfolio_job_history', ['custcode']);
    await queryInterface.removeIndex('portfolio_job_history', [
      'custcode',
      'marketing_id',
    ]);
  },
};
