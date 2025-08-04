module.exports = {
  up: async (queryInterface, Sequelize) => {
    return queryInterface.createTable('scb_dd_register', {
      id: {
        allowNull: false,
        autoIncrement: true,
        primaryKey: true,
        type: Sequelize.INTEGER,
      },

      citizen_id: { type: Sequelize.STRING },
      customer_code: { type: Sequelize.STRING },
      redirect_url: { type: Sequelize.STRING },
      registration_ref_code: { type: Sequelize.STRING },
      remarks: { type: Sequelize.STRING },

      // SCBBaseResponseAttributed
      merchant_id: { type: Sequelize.STRING },
      sub_account_id: { type: Sequelize.STRING },

      // SCBResultStatus
      code: { type: Sequelize.STRING },
      description: { type: Sequelize.STRING },
      details: { type: Sequelize.STRING },

      web_url: { type: Sequelize.STRING },

      created_at: {
        allowNull: false,
        type: Sequelize.DATE,
      },

      updated_at: {
        allowNull: false,
        type: Sequelize.DATE,
      },
    });
  },

  down: async (queryInterface, Sequelize) => {
    await queryInterface.dropTable('scb_dd_register');
  },
};
