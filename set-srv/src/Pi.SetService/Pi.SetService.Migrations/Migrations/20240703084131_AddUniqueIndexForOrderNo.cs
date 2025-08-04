using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.SetService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexForOrderNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "number_generators");

            migrationBuilder.AddColumn<DateOnly>(
                name: "created_at_date",
                table: "equity_order_state",
                type: "date",
                nullable: false,
                computedColumnSql: "DATE(created_at)");

            migrationBuilder.CreateIndex(
                name: "unique_order_date",
                table: "equity_order_state",
                columns: new[] { "order_no", "created_at_date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "unique_order_date",
                table: "equity_order_state");

            migrationBuilder.DropColumn(
                name: "created_at_date",
                table: "equity_order_state");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "equity_order_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP(6)",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIESTAMP(6)");

            migrationBuilder.CreateTable(
                name: "number_generators",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    currentcounter = table.Column<int>(name: "current_counter", type: "int", nullable: false),
                    dailyreset = table.Column<bool>(name: "daily_reset", type: "tinyint(1)", nullable: false),
                    module = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    prefix = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_number_generators", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
