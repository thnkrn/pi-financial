using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.StructureNotes.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class mg0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cash",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    symbol = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    currency = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    marketvalue = table.Column<decimal>(name: "market_value", type: "decimal(65,30)", nullable: true),
                    costvalue = table.Column<decimal>(name: "cost_value", type: "decimal(65,30)", nullable: true),
                    gaininportfolio = table.Column<decimal>(name: "gain_in_portfolio", type: "decimal(65,30)", nullable: true),
                    accountid = table.Column<string>(name: "account_id", type: "varchar(100)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    accountno = table.Column<string>(name: "account_no", type: "varchar(100)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "datetime(6)", nullable: false),
                    asof = table.Column<DateTime>(name: "as_of", type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cash", x => x.id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "notes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    isin = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    symbol = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    currency = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    marketvalue = table.Column<decimal>(name: "market_value", type: "decimal(65,30)", nullable: true),
                    costvalue = table.Column<decimal>(name: "cost_value", type: "decimal(65,30)", nullable: true),
                    type = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    yield = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    rebate = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    underlying = table.Column<string>(type: "varchar(400)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    tenors = table.Column<int>(type: "int", nullable: true),
                    tradedate = table.Column<DateTime>(name: "trade_date", type: "datetime(6)", nullable: false),
                    issuedate = table.Column<DateTime>(name: "issue_date", type: "datetime(6)", nullable: false),
                    valuationdate = table.Column<DateTime>(name: "valuation_date", type: "datetime(6)", nullable: false),
                    issuer = table.Column<string>(type: "varchar(400)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    accountid = table.Column<string>(name: "account_id", type: "varchar(100)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    accountno = table.Column<string>(name: "account_no", type: "varchar(100)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "datetime(6)", nullable: false),
                    asof = table.Column<DateTime>(name: "as_of", type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notes", x => x.id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "stocks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    symbol = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    currency = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    units = table.Column<int>(type: "int", nullable: true),
                    available = table.Column<int>(type: "int", nullable: true),
                    costprice = table.Column<decimal>(name: "cost_price", type: "decimal(65,30)", nullable: true),
                    accountid = table.Column<string>(name: "account_id", type: "varchar(100)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    accountno = table.Column<string>(name: "account_no", type: "varchar(100)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "datetime(6)", nullable: false),
                    asof = table.Column<DateTime>(name: "as_of", type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stocks", x => x.id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "ix_cash_account_id",
                table: "cash",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_cash_account_no",
                table: "cash",
                column: "account_no");

            migrationBuilder.CreateIndex(
                name: "ix_cash_currency",
                table: "cash",
                column: "currency");

            migrationBuilder.CreateIndex(
                name: "ix_notes_account_id",
                table: "notes",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_notes_account_no",
                table: "notes",
                column: "account_no");

            migrationBuilder.CreateIndex(
                name: "ix_notes_currency",
                table: "notes",
                column: "currency");

            migrationBuilder.CreateIndex(
                name: "ix_notes_symbol",
                table: "notes",
                column: "symbol");

            migrationBuilder.CreateIndex(
                name: "ix_stocks_account_id",
                table: "stocks",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_stocks_account_no",
                table: "stocks",
                column: "account_no");

            migrationBuilder.CreateIndex(
                name: "ix_stocks_currency",
                table: "stocks",
                column: "currency");

            migrationBuilder.CreateIndex(
                name: "ix_stocks_symbol",
                table: "stocks",
                column: "symbol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cash");

            migrationBuilder.DropTable(
                name: "notes");

            migrationBuilder.DropTable(
                name: "stocks");
        }
    }
}
