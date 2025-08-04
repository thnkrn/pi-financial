using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class ChangeDepositRefundToCommon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("b6bedce3-9801-419a-8b3d-e1b726ba9607"),
                column: "product_type",
                value: null);

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("ca7f7e31-69f1-4d33-a1b6-9f9645f26d00"),
                column: "product_type",
                value: null);

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("d53c7aca-bed8-409b-a163-40f33180960d"),
                column: "product_type",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("b6bedce3-9801-419a-8b3d-e1b726ba9607"),
                column: "product_type",
                value: "ThaiEquity");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("ca7f7e31-69f1-4d33-a1b6-9f9645f26d00"),
                column: "product_type",
                value: "ThaiEquity");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("d53c7aca-bed8-409b-a163-40f33180960d"),
                column: "product_type",
                value: "ThaiEquity");
        }
    }
}
