module.exports = {
    up: async (queryInterface, Sequelize) => {
        await queryInterface.createTable('exante_transactions', {
            id: {
                primaryKey: true,
                allowNull: false,
                type: Sequelize.UUID,
            },
            transaction_id: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            account_id: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            symbol_id: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            isin: {
                type: Sequelize.STRING,
            },
            operation_type: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            when: {
                allowNull: false,
                type: Sequelize.DATE,
            },
            sum: {
                allowNull: false,
                type: Sequelize.DECIMAL(65, 8),
            },
            asset: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            eur_equivalent: {
                allowNull: false,
                type: Sequelize.DECIMAL(65, 8),
            },
            comment: {
                type: Sequelize.STRING,
            },
            uuid: {
                allowNull: false,
                type: Sequelize.UUID,
            },
            parent_uuid: {
                type: Sequelize.UUID,
            },
        });
    },
    down: async (queryInterface, Sequelize) => {
        await queryInterface.dropTable('exante_transactions');
    },
};
