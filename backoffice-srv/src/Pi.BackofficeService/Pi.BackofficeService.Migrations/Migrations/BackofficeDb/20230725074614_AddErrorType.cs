#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class AddErrorType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "error_type",
                table: "error_mappings",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: null,
                collation: "utf8mb4_0900_ai_ci");
            migrationBuilder.UpdateData("error_mappings", "state", "DepositFailedNameMismatch", "error_type", ProductType.ThaiEquity.ToString());
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "error_type",
                table: "error_mappings");
        }
    }
}
