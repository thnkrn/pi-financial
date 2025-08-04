'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  up: async (queryInterface, Sequelize) => {
    await queryInterface.addColumn(
      'portfolio_job_history',
      'identification_hash',
      {
        type: Sequelize.STRING,
      }
    );
    await queryInterface.addIndex(
      'portfolio_job_history',
      ['identification_hash'],
      {
        name: 'portfolio_job_history_identification_hash_idx',
      }
    );
    await queryInterface.addIndex(
      'portfolio_job_history',
      ['identification_hash', 'marketing_id'],
      {
        name: 'portfolio_job_history_identification_hash_marketing_id_idx',
      }
    );
  },

  down: async (queryInterface, Sequelize) => {
    await queryInterface.removeIndex(
      'portfolio_job_history',
      'portfolio_job_history_identification_hash_marketing_id_idx'
    );
    await queryInterface.removeIndex(
      'portfolio_job_history',
      'portfolio_job_history_identification_hash_idx'
    );
    await queryInterface.removeColumn(
      'portfolio_job_history',
      'identification_hash'
    );
  },
};
