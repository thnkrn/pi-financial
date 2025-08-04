module.exports = {
  async up(queryInterface, Sequelize) {
    await queryInterface.createTable('thai_bma_instruments', {
      id: {
        primaryKey: true,
        allowNull: false,
        type: Sequelize.UUID,
      },
      symbol: {
        allowNull: false,
        type: Sequelize.STRING,
      },
      issue_rating: {
        allowNull: true,
        type: Sequelize.STRING,
      },
      company_rating: {
        allowNull: true,
        type: Sequelize.STRING,
      },
      coupon_type: {
        allowNull: true,
        type: Sequelize.STRING,
      },
      coupon_rate: {
        allowNull: true,
        type: Sequelize.DECIMAL(65, 8),
      },
      issued_date: {
        allowNull: true,
        type: Sequelize.DATEONLY,
      },
      maturity_date: {
        allowNull: true,
        type: Sequelize.DATEONLY,
      },
      ttm: {
        allowNull: true,
        type: Sequelize.DECIMAL(65, 8),
      },
      outstanding: {
        allowNull: true,
        type: Sequelize.DECIMAL(65, 8),
      },
      created_at: {
        allowNull: false,
        type: Sequelize.DATE,
      },
    });
  },

  async down(queryInterface, Sequelize) {
    await queryInterface.dropTable('thai_bma_instruments');
  },
};
