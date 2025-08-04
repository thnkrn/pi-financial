'use strict';

module.exports = {
    up: async (queryInterface, Sequelize) => {
        await queryInterface.addConstraint('pending_deposit_snapshot', {
            type: 'unique',
            fields: ['qr_transaction_no'],
            name: 'qr_transaction_number_unique',
          });
    },

    down: async (queryInterface, Sequelize) => {
        await queryInterface.removeConstraint('pending_deposit_snapshot','qr_transaction_number_unique');
    }
};