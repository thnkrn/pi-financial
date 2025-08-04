'use strict';

/** @type {import('sequelize-cli').Migration} */
// eslint-disable-next-line no-undef
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.changeColumn('exante_trade_vats', 'account_id', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trade_vats', 'symbol_id', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trade_vats', 'order_id', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trade_vats', 'operation_type', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trade_vats', 'when', {
      allowNull: true,
      type: Sequelize.DATE,
    });

    await queryInterface.changeColumn('exante_trade_vats', 'sum', {
      allowNull: true,
      type: Sequelize.DECIMAL(65, 8),
    });

    await queryInterface.changeColumn('exante_trade_vats', 'asset', {
      allowNull: true,
      type: Sequelize.STRING,
    });
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.changeColumn('exante_trade_vats', 'account_id', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trade_vats', 'symbol_id', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trade_vats', 'order_id', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trade_vats', 'operation_type', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trade_vats', 'when', {
      allowNull: false,
      type: Sequelize.DATE,
    });

    await queryInterface.changeColumn('exante_trade_vats', 'sum', {
      allowNull: false,
      type: Sequelize.DECIMAL(65, 8),
    });

    await queryInterface.changeColumn('exante_trade_vats', 'asset', {
      allowNull: false,
      type: Sequelize.STRING,
    });
  },
};
