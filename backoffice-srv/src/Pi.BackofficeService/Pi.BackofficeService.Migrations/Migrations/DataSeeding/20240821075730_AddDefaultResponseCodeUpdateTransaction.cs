using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class AddDefaultResponseCodeUpdateTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "is_filterable", "machine", "product_type", "state", "suggestion" },
                values: new object[,]
                {
                    { new Guid("961c1faf-963c-40ed-87e1-01ebfdebb0a0"), "Update Transaction", false, "Deposit", null, "", null },
                    { new Guid("bba989b7-8103-4841-802b-acc9146b0bd9"), "Update Transaction", false, "Withdraw", null, "", null }
                });

            migrationBuilder.InsertData(
                table: "response_code_actions",
                columns: new[] { "id", "action", "response_code_id" },
                values: new object[,]
                {
                    { new Guid("22872725-cdba-4ab8-9b04-c2748be6e2cb"), "ChangeStatusToFail", new Guid("961c1faf-963c-40ed-87e1-01ebfdebb0a0") },
                    { new Guid("362f0108-15c5-4bba-899f-92fd33a6f6ae"), "ChangeStatusToFail", new Guid("bba989b7-8103-4841-802b-acc9146b0bd9") },
                    { new Guid("b4bab9c1-fd63-4b41-83d6-2465226ed4d6"), "ChangeStatusToSuccess", new Guid("bba989b7-8103-4841-802b-acc9146b0bd9") },
                    { new Guid("f1f75678-b77f-4091-8267-cc284484cb0c"), "ChangeStatusToSuccess", new Guid("961c1faf-963c-40ed-87e1-01ebfdebb0a0") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("22872725-cdba-4ab8-9b04-c2748be6e2cb"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("362f0108-15c5-4bba-899f-92fd33a6f6ae"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("b4bab9c1-fd63-4b41-83d6-2465226ed4d6"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("f1f75678-b77f-4091-8267-cc284484cb0c"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("961c1faf-963c-40ed-87e1-01ebfdebb0a0"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("bba989b7-8103-4841-802b-acc9146b0bd9"));
        }
    }
}
