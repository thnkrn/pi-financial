using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class AddBillPaymentFailedNameMismatchState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "is_filterable", "machine", "product_type", "state", "suggestion" },
                values: new object[] { new Guid("aa6e979e-7304-4731-b535-52c3ec111ca4"), "Name Mismatch", true, "Deposit", "ThaiEquity", "BillPaymentFailedNameMismatch", "Investigate Name" });

            migrationBuilder.InsertData(
                table: "response_code_actions",
                columns: new[] { "id", "action", "response_code_id" },
                values: new object[,]
                {
                    { new Guid("124f57f9-d44b-41e3-adba-5d75f9f26ae1"), "ChangeStatusToSuccess", new Guid("aa6e979e-7304-4731-b535-52c3ec111ca4") },
                    { new Guid("651698ea-702a-41df-883e-fb9e56691264"), "Approve", new Guid("aa6e979e-7304-4731-b535-52c3ec111ca4") },
                    { new Guid("7045ca97-1700-4d5e-a8e7-51aa67ea8ea1"), "ChangeStatusToFail", new Guid("aa6e979e-7304-4731-b535-52c3ec111ca4") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("124f57f9-d44b-41e3-adba-5d75f9f26ae1"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("651698ea-702a-41df-883e-fb9e56691264"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("7045ca97-1700-4d5e-a8e7-51aa67ea8ea1"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("aa6e979e-7304-4731-b535-52c3ec111ca4"));
        }
    }
}
