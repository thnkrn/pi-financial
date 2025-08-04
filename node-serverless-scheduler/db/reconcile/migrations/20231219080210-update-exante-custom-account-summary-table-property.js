'use strict';

/** @type {import('sequelize-cli').Migration} */
// eslint-disable-next-line no-undef
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'date',
      {
        allowNull: true,
        type: Sequelize.DATE,
      }
    );

    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'account',
      {
        allowNull: true,
        type: Sequelize.STRING,
      }
    );

    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'instrument',
      {
        allowNull: true,
        type: Sequelize.STRING,
      }
    );

    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'iso',
      {
        allowNull: true,
        type: Sequelize.STRING,
      }
    );

    await queryInterface.renameColumn(
      'exante_custom_account_summaries',
      'p_and_l_in_thb',
      'p_and_l_in_eur'
    );

    await queryInterface.renameColumn(
      'exante_custom_account_summaries',
      'value_in_thb',
      'value_in_eur'
    );

    await queryInterface.renameColumn(
      'exante_custom_account_summaries',
      'daily_p_and_l_in_thb',
      'daily_p_and_l_in_eur'
    );
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'date',
      {
        allowNull: false,
        type: Sequelize.DATE,
      }
    );

    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'account',
      {
        allowNull: false,
        type: Sequelize.STRING,
      }
    );

    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'instrument',
      {
        allowNull: false,
        type: Sequelize.STRING,
      }
    );

    await queryInterface.changeColumn(
      'exante_custom_account_summaries',
      'iso',
      {
        allowNull: false,
        type: Sequelize.STRING,
      }
    );

    await queryInterface.renameColumn(
      'exante_custom_account_summaries',
      'p_and_l_in_eur',
      'p_and_l_in_thb'
    );

    await queryInterface.renameColumn(
      'exante_custom_account_summaries',
      'value_in_eur',
      'value_in_thb'
    );

    await queryInterface.renameColumn(
      'exante_custom_account_summaries',
      'daily_p_and_l_in_eur',
      'daily_p_and_l_in_thb'
    );
  },
};
