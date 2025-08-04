'use strict';

module.exports = {
    up: async (queryInterface, Sequelize) => {

        await queryInterface.addColumn('pending_deposit_snapshot', 'updated_at', {
            type: Sequelize.DATE
        });
    },

    down: async (queryInterface, Sequelize) => {
        await queryInterface.removeColumn('pending_deposit_snapshot', 'updated_at');
    }
};
