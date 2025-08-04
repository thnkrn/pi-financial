'use strict';

module.exports = {
    up: async (queryInterface, Sequelize) => {
        await queryInterface.addColumn('pending_deposit_snapshot', 'qr_transaction_no', {
            type: Sequelize.STRING
        });
    },

    down: async (queryInterface, Sequelize) => {
        await queryInterface.removeColumn('pending_deposit_snapshot', 'qr_transaction_no');
    }
};
