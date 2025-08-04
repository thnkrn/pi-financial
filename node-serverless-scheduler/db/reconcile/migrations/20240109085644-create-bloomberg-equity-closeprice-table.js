'use strict';

/** @type {import('sequelize-cli').Migration} */
module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.createTable('bloomberg_equity_closeprice', {
      id: {
        primaryKey: true,
        allowNull: false,
        type: Sequelize.UUID,
      },
      dl_request_id: {
        type: Sequelize.STRING,
      },
      dl_request_name: {
        type: Sequelize.STRING,
      },
      dl_snapshot_start_time: {
        type: Sequelize.DATE,
      },
      dl_snapshot_tz: {
        type: Sequelize.STRING,
      },
      identifier: {
        type: Sequelize.STRING,
      },
      rc: {
        type: Sequelize.INTEGER,
      },
      px_close_dt: {
        type: Sequelize.DATEONLY,
      },
      id_exch_symbol: {
        type: Sequelize.STRING,
      },
      name: {
        type: Sequelize.STRING,
      },
      px_last_eod: {
        type: Sequelize.DECIMAL,
      },
      crncy: {
        type: Sequelize.STRING,
      },
      composite_exch_code: {
        type: Sequelize.STRING,
      },
      id_isin: {
        type: Sequelize.STRING,
      },
      last_update_date_eod: {
        type: Sequelize.DATEONLY,
      },
      icb_supersector_name: {
        allowNull: false,
        type: Sequelize.STRING,
      },
      icb_sector_name: {
        allowNull: false,
        type: Sequelize.STRING,
      },
    });
  },

  async down(queryInterface) {
    await queryInterface.dropTable('bloomberg_equity_closeprice');
  },
};
