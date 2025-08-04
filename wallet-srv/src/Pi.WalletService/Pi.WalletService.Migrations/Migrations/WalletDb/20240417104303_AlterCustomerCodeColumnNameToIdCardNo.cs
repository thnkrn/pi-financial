using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.WalletService.Migrations.Migrations.WalletDb
{
    /// <inheritdoc />
    public partial class AlterCustomerCodeColumnNameToIdCardNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customer_code",
                table: "online_direct_debit_registrations");

            migrationBuilder.AddColumn<string>(
                name: "identification_card_no",
                table: "online_direct_debit_registrations",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                collation: "utf8mb4_0900_ai_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "identification_card_no",
                table: "online_direct_debit_registrations");

            migrationBuilder.AddColumn<string>(
                name: "customer_code",
                table: "online_direct_debit_registrations",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                collation: "utf8mb4_0900_ai_ci");
        }
    }
}
