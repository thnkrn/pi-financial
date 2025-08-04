module.exports = {
    up: async (queryInterface, Sequelize) => {
        await queryInterface.createTable('exante_trades', {
            id: {
                primaryKey: true,
                allowNull: false,
                type: Sequelize.UUID,
            },
            time: {
                allowNull: false,
                type: Sequelize.DATE,
            },
            account_id: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            side: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            symbol_id: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            isin: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            type: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            price: {
                allowNull: false,
                type: Sequelize.DECIMAL(65, 8),
            },
            currency: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            quantity: {
                allowNull: false,
                type: Sequelize.INTEGER,
            },
            commission: {
                allowNull: false,
                type: Sequelize.DECIMAL(65, 8),
            },
            commission_currency: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            p_and_l: {
                allowNull: false,
                type: Sequelize.DECIMAL(65, 8),
            },
            traded_volume: {
                allowNull: false,
                type: Sequelize.DECIMAL(65, 8),
            },
            order_id: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            order_pos: {
                allowNull: false,
                type: Sequelize.INTEGER,
            },
            value_date: {
                allowNull: false,
                type: Sequelize.DATEONLY,
            },
            uti: {
                type: Sequelize.STRING,
            },
            trade_type: {
                allowNull: false,
                type: Sequelize.STRING,
            },
        });
    },
    down: async (queryInterface, Sequelize) => {
        await queryInterface.dropTable('exante_trades');
    },
};
