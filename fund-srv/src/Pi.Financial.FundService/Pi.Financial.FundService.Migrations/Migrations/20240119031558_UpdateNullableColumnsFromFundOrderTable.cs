using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.Financial.FundService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNullableColumnsFromFundOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "unit",
                table: "fund_order_state",
                type: "decimal(65,30)",
                precision: 65,
                scale: 30,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldPrecision: 65,
                oldScale: 30);

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "fund_order_state",
                type: "decimal(65,30)",
                precision: 65,
                scale: 30,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldPrecision: 65,
                oldScale: 30);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "unit",
                table: "fund_order_state",
                type: "decimal(65,30)",
                precision: 65,
                scale: 30,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldPrecision: 65,
                oldScale: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "fund_order_state",
                type: "decimal(65,30)",
                precision: 65,
                scale: 30,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldPrecision: 65,
                oldScale: 30,
                oldNullable: true);
        }
    }
}
