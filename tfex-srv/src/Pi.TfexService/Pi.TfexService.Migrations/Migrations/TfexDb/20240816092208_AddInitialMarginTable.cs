using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.TfexService.Migrations.Migrations.TfexDb
{
    /// <inheritdoc />
    public partial class AddInitialMarginTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "initial_margins",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    symbol = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    product_type = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    im = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: false),
                    im_outright = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: false),
                    im_spread = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: false),
                    as_of_date = table.Column<DateOnly>(type: "date", maxLength: 6, nullable: false),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_initial_margins", x => x.id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "ix_initial_margins_symbol",
                table: "initial_margins",
                column: "symbol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "initial_margins");
        }
    }
}
