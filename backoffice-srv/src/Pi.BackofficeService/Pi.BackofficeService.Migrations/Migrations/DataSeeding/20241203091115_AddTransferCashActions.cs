using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class AddTransferCashActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "is_filterable", "machine", "product_type", "state", "suggestion" },
                values: new object[] { new Guid("9621da04-db2a-4286-a792-8b7000897f43"), "SetTrade Trading Account Transfer Fail", false, "TransferCash", null, "TransferCashFailedRequireActionSetTrade", "Check MT4, check customer SetTrade account balance, before manual allocate" });

            migrationBuilder.InsertData(
                table: "response_code_actions",
                columns: new[] { "id", "action", "response_code_id" },
                values: new object[,]
                {
                    { new Guid("5c329989-d81e-4a81-8e4c-140de3454417"), "ChangeStatusToSuccess", new Guid("9621da04-db2a-4286-a792-8b7000897f43") },
                    { new Guid("641da5c8-cf45-4f3c-812e-c9b13047bca0"), "ChangeStatusToFail", new Guid("9621da04-db2a-4286-a792-8b7000897f43") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("5c329989-d81e-4a81-8e4c-140de3454417"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("641da5c8-cf45-4f3c-812e-c9b13047bca0"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("9621da04-db2a-4286-a792-8b7000897f43"));
        }
    }
}
