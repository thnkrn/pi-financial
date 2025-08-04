'use strict';

/** @type {import('sequelize-cli').Migration} */
// eslint-disable-next-line no-undef
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.changeColumn('exante_transactions', 'transaction_id', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_transactions', 'account_id', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_transactions', 'symbol_id', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_transactions', 'isin', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_transactions', 'operation_type', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_transactions', 'when', {
      allowNull: true,
      type: Sequelize.DATE,
    });

    await queryInterface.changeColumn('exante_transactions', 'sum', {
      allowNull: true,
      type: Sequelize.DECIMAL(65, 8),
    });

    await queryInterface.changeColumn('exante_transactions', 'asset', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_transactions', 'eur_equivalent', {
      allowNull: true,
      type: Sequelize.DECIMAL(65, 8),
    });

    await queryInterface.changeColumn('exante_transactions', 'comment', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_transactions', 'uuid', {
      allowNull: true,
      type: Sequelize.UUID,
    });
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.changeColumn('exante_transactions', 'transaction_id', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_transactions', 'account_id', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_transactions', 'symbol_id', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_transactions', 'isin', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_transactions', 'operation_type', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_transactions', 'when', {
      allowNull: false,
      type: Sequelize.DATE,
    });

    await queryInterface.changeColumn('exante_transactions', 'sum', {
      allowNull: false,
      type: Sequelize.DECIMAL(65, 8),
    });

    await queryInterface.changeColumn('exante_transactions', 'asset', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_transactions', 'eur_equivalent', {
      allowNull: false,
      type: Sequelize.DECIMAL(65, 8),
    });

    await queryInterface.changeColumn('exante_transactions', 'comment', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_transactions', 'uuid', {
      allowNull: false,
      type: Sequelize.UUID,
    });
  },
};
