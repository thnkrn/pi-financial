#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class UpdateStateName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData("error_mappings", "state", "DepositFailedNameMismatched", "state", "DepositFailedNameMismatch");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData("error_mappings", "state", "DepositFailedNameMismatch", "state", "DepositFailedNameMismatched");
        }
    }
}
