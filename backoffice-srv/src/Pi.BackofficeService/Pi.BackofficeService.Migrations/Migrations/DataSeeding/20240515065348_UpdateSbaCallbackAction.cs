using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class UpdateSbaCallbackAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("b89a1eba-eedd-4509-b427-2c26456f2755"),
                column: "action",
                value: "SbaConfirm");

            migrationBuilder.InsertData(
                table: "response_code_actions",
                columns: new[] { "id", "action", "response_code_id" },
                values: new object[] { new Guid("b06c1068-7d65-4784-822f-5c471a89a21d"), "SbaConfirm", new Guid("d8fa6460-36cf-458f-9f12-f43cdd9c6b28") });

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("d8fa6460-36cf-458f-9f12-f43cdd9c6b28"),
                column: "suggestion",
                value: "Check freewill, Before confirm SBA");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("d9fef26d-a2ef-470e-82be-9f64f2c32d90"),
                column: "suggestion",
                value: "Check freewill, Check Customer Trading Account balance , Before confirm SBA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("b06c1068-7d65-4784-822f-5c471a89a21d"));

            migrationBuilder.UpdateData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("b89a1eba-eedd-4509-b427-2c26456f2755"),
                column: "action",
                value: "SbaDepositConfirm");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("d8fa6460-36cf-458f-9f12-f43cdd9c6b28"),
                column: "suggestion",
                value: "Change transaction status");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("d9fef26d-a2ef-470e-82be-9f64f2c32d90"),
                column: "suggestion",
                value: "Check freewill, Check Customer Trading Account balance , before deposit sba");
        }
    }
}
