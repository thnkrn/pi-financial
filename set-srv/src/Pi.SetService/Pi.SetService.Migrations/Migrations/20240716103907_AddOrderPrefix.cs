using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.SetService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderPrefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "type",
                table: "equity_order_state",
                newName: "order_type");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "equity_order_state",
                newName: "order_status");

            migrationBuilder.RenameColumn(
                name: "side",
                table: "equity_order_state",
                newName: "order_side");

            migrationBuilder.RenameColumn(
                name: "action",
                table: "equity_order_state",
                newName: "order_action");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "order_type",
                table: "equity_order_state",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "order_status",
                table: "equity_order_state",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "order_side",
                table: "equity_order_state",
                newName: "side");

            migrationBuilder.RenameColumn(
                name: "order_action",
                table: "equity_order_state",
                newName: "action");
        }
    }
}
