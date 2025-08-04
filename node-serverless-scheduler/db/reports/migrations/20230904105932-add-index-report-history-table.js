module.exports = {
    up: async (queryInterface) => {
        await queryInterface.addIndex("report_history", ['updated_at'])
    },
    down: async (queryInterface) => {
        await queryInterface.removeIndex("report_history", ['updated_at'])
    }
};