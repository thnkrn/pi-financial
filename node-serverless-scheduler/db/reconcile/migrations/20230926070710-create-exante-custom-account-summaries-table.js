module.exports = {
    up: async (queryInterface, Sequelize) => {
        await queryInterface.createTable('exante_custom_account_summaries', {
            id: {
                primaryKey: true,
                allowNull: false,
                type: Sequelize.UUID,
            },
            date: {
                allowNull: false,
                type: Sequelize.DATE,
            },
            account: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            instrument: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            iso: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            instrument_name: {
                type: Sequelize.STRING,
            },
            qty: {
                type: Sequelize.INTEGER,
            },
            avg_price: {
                type: Sequelize.DECIMAL(65, 8),
            },
            price: {
                type: Sequelize.DECIMAL(65, 8),
            },
            ccy: {
                type: Sequelize.DECIMAL(65, 8),
            },
            p_and_l: {
                type: Sequelize.DECIMAL(65, 8),
            },
            p_and_l_in_thb: {
                type: Sequelize.DECIMAL(65, 8),
            },
            p_and_l_percent: {
                type: Sequelize.DECIMAL(65, 8),
            },
            value: {
                allowNull: false,
                type: Sequelize.DECIMAL(65, 8),
            },
            value_in_thb: {
                allowNull: false,
                type: Sequelize.DECIMAL(65, 8),
            },
            daily_p_and_l: {
                type: Sequelize.DECIMAL(65, 8),
            },
            daily_p_and_l_in_thb: {
                type: Sequelize.DECIMAL(65, 8),
            },
            daily_p_and_l_percent: {
                type: Sequelize.DECIMAL(65, 8),
            },
            isin: {
                type: Sequelize.STRING,
            },
        });
    },
    down: async (queryInterface, Sequelize) => {
        await queryInterface.dropTable('exante_custom_account_summaries');
    },
};
