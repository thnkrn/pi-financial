module.exports = {
    up: async (queryInterface, Sequelize) => {
        await queryInterface.createTable("pending_deposit_snapshot", {
            id: {
                primaryKey: true,
                allowNull: false,
                type: Sequelize.UUID
            },
            payment_received_datetime: {
                allowNull: false,
                type: Sequelize.DATE,
            },
            sender_account: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            bank_code: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            sender_channel: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            transaction_number: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            customer_bank_name: {
                allowNull: false,
                type: Sequelize.STRING,
            },
            created_at: {
                type: Sequelize.DATE
            },
            deleted_at: {
                type: Sequelize.DATE
            }
        });
    },
    down: async (queryInterface, Sequelize) => {
        await queryInterface.dropTable("pending_deposit_snapshot");
    },
};
