module.exports = {
  up: async (queryInterface, Sequelize) => {
    await queryInterface.createTable('portfolio_job_history', {
      id: {
        primaryKey: true,
        allowNull: false,
        type: Sequelize.UUID,
      },
      custcode: {
        allowNull: false,
        type: Sequelize.STRING,
      },
      marketing_id: {
        allowNull: false,
        type: Sequelize.STRING,
      },
      metadata: {
        allowNull: false,
        type: Sequelize.STRING,
      },
      send_date: {
        allowNull: false,
        type: Sequelize.DATEONLY,
      },
      status: {
        allowNull: false,
        type: Sequelize.STRING,
      },
      created_at: {
        type: Sequelize.DATE,
      },
    });
  },
  down: async (queryInterface, Sequelize) => {
    await queryInterface.dropTable('portfolio_job_history');
  },
};
