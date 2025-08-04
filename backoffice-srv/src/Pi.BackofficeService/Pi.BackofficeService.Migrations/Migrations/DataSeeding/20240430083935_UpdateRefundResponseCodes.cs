using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class UpdateRefundResponseCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("2203f732-3fbe-4738-95d2-1c0f70603914"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("220ed567-701a-4903-a8fe-ad5d3cfc43c1"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("ca7f7e31-69f1-4d33-a1b6-9f9645f26d00"));

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("b6bedce3-9801-419a-8b3d-e1b726ba9607"),
                column: "state",
                value: "RefundSuccess");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("b6bedce3-9801-419a-8b3d-e1b726ba9607"),
                column: "state",
                value: "DepositRefundSucceed");

            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "is_filterable", "machine", "product_type", "state", "suggestion" },
                values: new object[,]
                {
                    { new Guid("2203f732-3fbe-4738-95d2-1c0f70603914"), "Refund Success", true, "Deposit", "GlobalEquity", "RefundSucceed", null },
                    { new Guid("220ed567-701a-4903-a8fe-ad5d3cfc43c1"), "Refund Fail", true, "Deposit", "GlobalEquity", "RefundFailed", "Contact Technical Support and Manual Refund Required" },
                    { new Guid("ca7f7e31-69f1-4d33-a1b6-9f9645f26d00"), "Refund Fail", true, "Deposit", null, "DepositRefundFailed", "Contact Technical Support and Manual Refund Required" }
                });
        }
    }
}
