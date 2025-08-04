module.exports = {
    up: async (queryInterface) => {
        await queryInterface.addIndex("pending_deposit_snapshot", ['updated_at'])
        await queryInterface.addIndex("pending_deposit_snapshot", ['deleted_at'])
    },
    down: async (queryInterface) => {
        await queryInterface.removeIndex("pending_deposit_snapshot", ['updated_at'])
        await queryInterface.removeIndex("pending_deposit_snapshot", ['deleted_at'])
    }
};