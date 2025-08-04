using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.Financial.FundService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnsFromFundOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "extra_info",
                table: "fund_order_state");

            migrationBuilder.AlterColumn<string>(
                name: "redemption_type",
                table: "fund_order_state",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "fund_order_state",
                keyColumn: "redemption_type",
                keyValue: null,
                column: "redemption_type",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "redemption_type",
                table: "fund_order_state",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "extra_info",
                table: "fund_order_state",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
