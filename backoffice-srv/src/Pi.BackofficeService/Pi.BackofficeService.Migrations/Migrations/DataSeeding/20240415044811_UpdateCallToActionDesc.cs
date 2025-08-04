using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class UpdateCallToActionDesc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("caa1b189-aa54-4c5d-933f-8eb8bb066aa2"),
                columns: new[] { "description", "suggestion" },
                values: new object[] { "Waiting for ats response", "Check freewill report, before approve front" });

            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "is_filterable", "machine", "product_type", "state", "suggestion" },
                values: new object[,]
                {
                    { new Guid("4bc8fc98-a84c-4bc0-97cc-99cca7c24e5b"), "Pending Revert Transaction", true, "Withdraw", null, "WithdrawFailedRequireActionRecovery", "Check kkp report, contact IT to revert transaction" },
                    { new Guid("532d2838-9610-4704-a267-4e609032adf9"), "Pending Revert Transaction", true, "Withdraw", null, "UpBackFailedRequireActionRevert", "Check freewill or settrade, Contact IT to revert, Before mark as fail" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("4bc8fc98-a84c-4bc0-97cc-99cca7c24e5b"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("532d2838-9610-4704-a267-4e609032adf9"));

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("caa1b189-aa54-4c5d-933f-8eb8bb066aa2"),
                columns: new[] { "description", "suggestion" },
                values: new object[] { "Waiting to receiving freewill ats response", "Check freewill report, Check Customer Trading Account balance , before deposit sba ats" });
        }
    }
}
