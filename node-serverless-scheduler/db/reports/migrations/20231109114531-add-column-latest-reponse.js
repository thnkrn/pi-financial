'use strict';

module.exports = {
    up: async (queryInterface, Sequelize) => {

        await queryInterface.addColumn('pending_deposit_snapshot', 'latest_response', {
            type: Sequelize.STRING
        });
    },

    down: async (queryInterface, Sequelize) => {
        await queryInterface.removeColumn('pending_deposit_snapshot', 'latest_response');
    }
};
