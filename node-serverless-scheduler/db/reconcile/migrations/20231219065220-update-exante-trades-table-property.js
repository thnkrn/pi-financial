'use strict';

/** @type {import('sequelize-cli').Migration} */
// eslint-disable-next-line no-undef
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.changeColumn('exante_trades', 'time', {
      allowNull: true,
      type: Sequelize.DATE,
    });

    await queryInterface.changeColumn('exante_trades', 'account_id', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'side', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'symbol_id', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'isin', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'type', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'price', {
      allowNull: true,
      type: Sequelize.DECIMAL(65, 8),
    });

    await queryInterface.changeColumn('exante_trades', 'currency', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'quantity', {
      allowNull: true,
      type: Sequelize.INTEGER,
    });

    await queryInterface.changeColumn('exante_trades', 'commission', {
      allowNull: true,
      type: Sequelize.DECIMAL(65, 8),
    });

    await queryInterface.changeColumn('exante_trades', 'commission_currency', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'p_and_l', {
      allowNull: true,
      type: Sequelize.DECIMAL(65, 8),
    });

    await queryInterface.changeColumn('exante_trades', 'traded_volume', {
      allowNull: true,
      type: Sequelize.DECIMAL(65, 8),
    });

    await queryInterface.changeColumn('exante_trades', 'order_id', {
      allowNull: true,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'order_pos', {
      allowNull: true,
      type: Sequelize.INTEGER,
    });

    await queryInterface.changeColumn('exante_trades', 'value_date', {
      allowNull: true,
      type: Sequelize.DATEONLY,
    });

    await queryInterface.changeColumn('exante_trades', 'trade_type', {
      allowNull: true,
      type: Sequelize.STRING,
    });
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.changeColumn('exante_trades', 'time', {
      allowNull: false,
      type: Sequelize.DATE,
    });

    await queryInterface.changeColumn('exante_trades', 'account_id', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'side', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'symbol_id', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'isin', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'type', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'price', {
      allowNull: false,
      type: Sequelize.DECIMAL(65, 8),
    });

    await queryInterface.changeColumn('exante_trades', 'currency', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'quantity', {
      allowNull: false,
      type: Sequelize.INTEGER,
    });

    await queryInterface.changeColumn('exante_trades', 'commission', {
      allowNull: false,
      type: Sequelize.DECIMAL(65, 8),
    });

    await queryInterface.changeColumn('exante_trades', 'commission_currency', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'p_and_l', {
      allowNull: false,
      type: Sequelize.DECIMAL(65, 8),
    });

    await queryInterface.changeColumn('exante_trades', 'traded_volume', {
      allowNull: false,
      type: Sequelize.DECIMAL(65, 8),
    });

    await queryInterface.changeColumn('exante_trades', 'order_id', {
      allowNull: false,
      type: Sequelize.STRING,
    });

    await queryInterface.changeColumn('exante_trades', 'order_pos', {
      allowNull: false,
      type: Sequelize.INTEGER,
    });

    await queryInterface.changeColumn('exante_trades', 'value_date', {
      allowNull: false,
      type: Sequelize.DATEONLY,
    });

    await queryInterface.changeColumn('exante_trades', 'trade_type', {
      allowNull: false,
      type: Sequelize.STRING,
    });
  },
};
