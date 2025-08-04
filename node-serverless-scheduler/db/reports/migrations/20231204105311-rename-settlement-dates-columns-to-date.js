'use strict';

module.exports = {
    up: async (queryInterface, Sequelize) => {
        await queryInterface.renameColumn('report_history', 'settlement_date_from', 'date_from');
        await queryInterface.renameColumn('report_history', 'settlement_date_to', 'date_to');
    },

    down: async (queryInterface, Sequelize) => {
        await queryInterface.renameColumn('report_history', 'date_from', 'settlement_date_from');
        await queryInterface.renameColumn('report_history', 'date_to', 'settlement_date_to');
    }
};