'use strict';

module.exports = {
    up: async (queryInterface, Sequelize) => {
        await queryInterface.addColumn('pending_deposit_snapshot', 'payment_received_amount', {
            type: Sequelize.DECIMAL(65, 8)
        });
    },

    down: async (queryInterface, Sequelize) => {
        await queryInterface.removeColumn('pending_deposit_snapshot', 'payment_received_amount');
    }
};
