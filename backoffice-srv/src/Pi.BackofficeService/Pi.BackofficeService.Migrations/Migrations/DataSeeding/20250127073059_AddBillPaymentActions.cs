using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class AddBillPaymentActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("9621da04-db2a-4286-a792-8b7000897f43"),
                column: "state",
                value: "TransferFailedRequireActionSetTrade");

            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "is_filterable", "machine", "product_type", "state", "suggestion" },
                values: new object[] { new Guid("a10a5dfb-265b-457d-996b-4858ad450bd5"), "Account Number Mismatch (Ref1)", false, "Deposit", null, "BillPaymentRequestInvalid", "Look up sender bank name and verify with customers before input correct account number" });

            migrationBuilder.InsertData(
                table: "response_code_actions",
                columns: new[] { "id", "action", "response_code_id" },
                values: new object[,]
                {
                    { new Guid("6952e94c-38aa-44eb-9f2b-9fb2fb280907"), "UpdateBillPaymentReference", new Guid("a10a5dfb-265b-457d-996b-4858ad450bd5") },
                    { new Guid("c54b668a-32a0-478f-bd57-ae40fda48a00"), "ChangeStatusToSuccess", new Guid("a10a5dfb-265b-457d-996b-4858ad450bd5") },
                    { new Guid("fe1f553b-c87e-43ec-868d-d19cd05f6e28"), "ChangeStatusToFail", new Guid("a10a5dfb-265b-457d-996b-4858ad450bd5") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("6952e94c-38aa-44eb-9f2b-9fb2fb280907"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("c54b668a-32a0-478f-bd57-ae40fda48a00"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("fe1f553b-c87e-43ec-868d-d19cd05f6e28"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("a10a5dfb-265b-457d-996b-4858ad450bd5"));

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("9621da04-db2a-4286-a792-8b7000897f43"),
                column: "state",
                value: "TransferCashFailedRequireActionSetTrade");
        }
    }
}
