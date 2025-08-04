using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.Financial.FundService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddBrokerOrderIdAndOrderTypeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_fund_order_state_broker_order_id_order_type",
                table: "fund_order_state",
                columns: new[] { "broker_order_id", "order_type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_fund_order_state_broker_order_id_order_type",
                table: "fund_order_state");
        }
    }
}
