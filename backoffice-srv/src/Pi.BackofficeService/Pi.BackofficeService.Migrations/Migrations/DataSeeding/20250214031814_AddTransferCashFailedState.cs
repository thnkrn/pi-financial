using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class AddTransferCashFailedState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "is_filterable", "machine", "product_type", "state", "suggestion" },
                values: new object[] { new Guid("f4c3f9ab-1873-4b1b-b301-9002b5aaf85f"), "SetTrade Trading Account Transfer Fail", false, "TransferCash", null, "TransferCashFailedRequireActionSetTrade", "Check MT4, check customer SetTrade account balance, before manual allocate" });

            migrationBuilder.InsertData(
                table: "response_code_actions",
                columns: new[] { "id", "action", "response_code_id" },
                values: new object[,]
                {
                    { new Guid("0c08be70-9370-44ba-bd4a-8e6404392a2f"), "ChangeStatusToSuccess", new Guid("f4c3f9ab-1873-4b1b-b301-9002b5aaf85f") },
                    { new Guid("53cd518f-6f3a-4d7d-b211-097fce7873a5"), "ChangeStatusToFail", new Guid("f4c3f9ab-1873-4b1b-b301-9002b5aaf85f") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("0c08be70-9370-44ba-bd4a-8e6404392a2f"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("53cd518f-6f3a-4d7d-b211-097fce7873a5"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f4c3f9ab-1873-4b1b-b301-9002b5aaf85f"));
        }
    }
}
