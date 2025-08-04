using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class AddConfirmCallbackAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "is_filterable", "machine", "product_type", "state", "suggestion" },
                values: new object[,]
                {
                    { new Guid("b05dbb35-7e00-4ada-9811-4ece7d7e1625"), "Waiting for receiving kkp response", true, "Deposit", null, "WaitingForPayment", "Check kkp report, Check Customer Trading Account balance,  before kkp callback confirm" },
                    { new Guid("caa1b189-aa54-4c5d-933f-8eb8bb066aa2"), "Waiting to receiving freewill ats response", true, "Deposit", null, "WaitingForAtsGatewayConfirmation", "Check freewill report, Check Customer Trading Account balance , before deposit sba ats" },
                    { new Guid("d9fef26d-a2ef-470e-82be-9f64f2c32d90"), "Waiting for receiving freewill response", true, "Deposit", null, "DepositWaitingForGateway", "Check freewill, Check Customer Trading Account balance , before deposit sba" }
                });

            migrationBuilder.InsertData(
                table: "response_code_actions",
                columns: new[] { "id", "action", "response_code_id" },
                values: new object[,]
                {
                    { new Guid("586c694a-dfb1-41b2-af6b-b2ff1fe63fbd"), "SbaDepositAtsCallbackConfirm", new Guid("caa1b189-aa54-4c5d-933f-8eb8bb066aa2") },
                    { new Guid("b89a1eba-eedd-4509-b427-2c26456f2755"), "SbaDepositConfirm", new Guid("d9fef26d-a2ef-470e-82be-9f64f2c32d90") },
                    { new Guid("eb19e2ce-4565-4ea5-9c1b-177f14f582cb"), "DepositKkpConfirm", new Guid("b05dbb35-7e00-4ada-9811-4ece7d7e1625") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("586c694a-dfb1-41b2-af6b-b2ff1fe63fbd"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("b89a1eba-eedd-4509-b427-2c26456f2755"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("eb19e2ce-4565-4ea5-9c1b-177f14f582cb"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("b05dbb35-7e00-4ada-9811-4ece7d7e1625"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("caa1b189-aa54-4c5d-933f-8eb8bb066aa2"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("d9fef26d-a2ef-470e-82be-9f64f2c32d90"));
        }
    }
}
