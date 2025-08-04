module.exports = {
    up: async (queryInterface, Sequelize) => {
        await queryInterface.createTable('exante_trade_vats', {
            id: {
                primaryKey: true,
                allowNull: false,
                type: Sequelize.UUID,
            },
            transaction_id: {
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
            order_id: {
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
            commission_before_vat: {
                type: Sequelize.DECIMAL(65, 8),
            },
            other_fees: {
                type: Sequelize.DECIMAL(65, 8),
            },
            vat_amount: {
                type: Sequelize.DECIMAL(65, 8),
            },
            total_commission: {
                type: Sequelize.DECIMAL(65, 8),
            },
            exante_commission_with_other_fees: {
                type: Sequelize.DECIMAL(65, 8),
            },
            partner_rebate: {
                type: Sequelize.DECIMAL(65, 8),
            },
            asset: {
                allowNull: false,
                type: Sequelize.STRING,
            },
        });
    },
    down: async (queryInterface, Sequelize) => {
        await queryInterface.dropTable('exante_trade_vats');
    },
};
