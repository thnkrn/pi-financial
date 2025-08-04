module.exports = {
    up: async (queryInterface, Sequelize) => {
        await queryInterface.createTable("report_history", {
            id: {
                primaryKey: true,
                allowNull: false,
                type: Sequelize.UUID,
            },
            report_name: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            user_name: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            status: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            settlement_date: {
                allowNull: false,
                type: Sequelize.DATEONLY,
            },
            file_path: {
                allowNull: true,
                type: Sequelize.STRING(2048),
            },
            created_at: {
                type: Sequelize.DATE,
            },
            updated_at: {
                type: Sequelize.DATE,
            },
        });
    },
    down: async (queryInterface, Sequelize) => {
        await queryInterface.dropTable("report_history");
    },
};
