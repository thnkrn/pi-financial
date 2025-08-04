using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.SetService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddMacthedPriceAndCanlledVolume : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cancelled_volume",
                table: "equity_order_state",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "matched_price",
                table: "equity_order_state",
                type: "decimal(65,30)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cancelled_volume",
                table: "equity_order_state");

            migrationBuilder.DropColumn(
                name: "matched_price",
                table: "equity_order_state");
        }
    }
}
