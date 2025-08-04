using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.Financial.FundService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFundInfoFieldsOnFundOderState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "instrument_id",
                table: "fund_order_state");

            migrationBuilder.DropColumn(
                name: "venue_id",
                table: "fund_order_state");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "instrument_id",
                table: "fund_order_state",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "venue_id",
                table: "fund_order_state",
                type: "int",
                nullable: true);
        }
    }
}
