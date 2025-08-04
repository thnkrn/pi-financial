'use strict';

module.exports = {
    up: async (queryInterface, Sequelize) => {

        await queryInterface.addColumn('report_history', 'settlement_date_to', {
            type: Sequelize.DATEONLY
        });


        await queryInterface.sequelize.query(
            'UPDATE report_history SET settlement_date_to = settlement_date'
        );


        await queryInterface.renameColumn('report_history', 'settlement_date', 'settlement_date_from');
    },

    down: async (queryInterface, Sequelize) => {

        await queryInterface.renameColumn('report_history', 'settlement_date_from', 'settlement_date');
        await queryInterface.removeColumn('report_history', 'settlement_date_to');
    }
};