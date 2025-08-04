'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'value',
      {
        type: Sequelize.DECIMAL(65, 8),
        allowNull: true,
      }
    );

    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'value_in_thb',
      {
        type: Sequelize.DECIMAL(65, 8),
        allowNull: true,
      }
    );

    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'ccy',
      {
        type: Sequelize.STRING,
        allowNull: true,
      }
    );

    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'qty',
      {
        type: Sequelize.DECIMAL(65, 8),
        allowNull: true,
      }
    );
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'value',
      {
        type: Sequelize.DECIMAL(65, 8),
        allowNull: false,
      }
    );

    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'value_in_thb',
      {
        type: Sequelize.DECIMAL(65, 8),
        allowNull: false,
      }
    );

    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'ccy',
      {
        type: Sequelize.DECIMAL(65, 8),
        allowNull: true,
      }
    );

    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'qty',
      {
        type: Sequelize.INTEGER,
        allowNull: true,
      }
    );
  },
};
