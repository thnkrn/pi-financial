#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class UpdateResponseCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("2258bbbc-2dbf-4519-9d40-3bfa7e4b6609"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("5afa5a4e-d054-4377-a9f4-e808c1c3706f"));

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("2203f732-3fbe-4738-95d2-1c0f70603914"),
                column: "product_type",
                value: "GlobalEquity");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("220ed567-701a-4903-a8fe-ad5d3cfc43c1"),
                column: "product_type",
                value: "GlobalEquity");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f28492e9-1ee4-4ea7-bfb2-a965eb8cb107"),
                column: "suggestion",
                value: "Contact Technical Team and check Customer Trading Account Balance");

            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "machine", "product_type", "state", "suggestion" },
                values: new object[,]
                {
                    { new Guid("06b0657c-9338-4db1-baf0-95351bec3de2"), "Refunding", "Deposit", "GlobalEquity", "Refunding", null },
                    { new Guid("b6bedce3-9801-419a-8b3d-e1b726ba9607"), "Refund Success", "Deposit", "ThaiEquity", "DepositRefundSucceed", null },
                    { new Guid("ca7f7e31-69f1-4d33-a1b6-9f9645f26d00"), "Refund Fail", "Deposit", "ThaiEquity", "DepositRefundFailed", "Contact Technical Support and Manual Refund Required" },
                    { new Guid("d53c7aca-bed8-409b-a163-40f33180960d"), "Refunding", "Deposit", "ThaiEquity", "DepositRefunding", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("06b0657c-9338-4db1-baf0-95351bec3de2"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("b6bedce3-9801-419a-8b3d-e1b726ba9607"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("ca7f7e31-69f1-4d33-a1b6-9f9645f26d00"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("d53c7aca-bed8-409b-a163-40f33180960d"));

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("2203f732-3fbe-4738-95d2-1c0f70603914"),
                column: "product_type",
                value: null);

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("220ed567-701a-4903-a8fe-ad5d3cfc43c1"),
                column: "product_type",
                value: null);

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f28492e9-1ee4-4ea7-bfb2-a965eb8cb107"),
                column: "suggestion",
                value: "Contact Technical Team");

            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "machine", "product_type", "state", "suggestion" },
                values: new object[,]
                {
                    { new Guid("2258bbbc-2dbf-4519-9d40-3bfa7e4b6609"), "Revert Transfer Fail", "Withdraw", "ThaiEquity", "RevertTransferFailed", "Contact Technical Team and check Customer Trading Account Balance" },
                    { new Guid("5afa5a4e-d054-4377-a9f4-e808c1c3706f"), "Revert Transfer Success", "Withdraw", "ThaiEquity", "RevertTransferSuccess", null }
                });
        }
    }
}
