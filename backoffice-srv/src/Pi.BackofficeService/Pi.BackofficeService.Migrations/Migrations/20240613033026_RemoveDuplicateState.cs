using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("1ff86f44-9818-44f7-a421-89105a3a4a7c"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("bbfeb1ac-453b-4bd3-b3e7-b07c61bb6538"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("69aeef24-d4fe-45b5-b0e8-cce920f67936"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "is_filterable", "machine", "product_type", "state", "suggestion" },
                values: new object[] { new Guid("69aeef24-d4fe-45b5-b0e8-cce920f67936"), "SetTrade Trading Account Deposit Fail", true, "Deposit", null, "UpBackFailedRequireActionSetTrade", "Change transaction status" });

            migrationBuilder.InsertData(
                table: "response_code_actions",
                columns: new[] { "id", "action", "response_code_id" },
                values: new object[,]
                {
                    { new Guid("1ff86f44-9818-44f7-a421-89105a3a4a7c"), "ChangeStatusToFail", new Guid("69aeef24-d4fe-45b5-b0e8-cce920f67936") },
                    { new Guid("bbfeb1ac-453b-4bd3-b3e7-b07c61bb6538"), "ChangeStatusToSuccess", new Guid("69aeef24-d4fe-45b5-b0e8-cce920f67936") }
                });
        }
    }
}
