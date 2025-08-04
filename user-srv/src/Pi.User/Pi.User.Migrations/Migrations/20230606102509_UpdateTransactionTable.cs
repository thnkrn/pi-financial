using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.User.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_transaction_ids",
                table: "transaction_ids");

            migrationBuilder.AddColumn<string>(
                name: "refer_id",
                table: "transaction_ids",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "customer_code",
                table: "transaction_ids",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "pk_transaction_ids",
                table: "transaction_ids",
                columns: new[] { "prefix", "date", "refer_id", "customer_code" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_transaction_ids",
                table: "transaction_ids");

            migrationBuilder.DropColumn(
                name: "refer_id",
                table: "transaction_ids");

            migrationBuilder.DropColumn(
                name: "customer_code",
                table: "transaction_ids");

            migrationBuilder.AddPrimaryKey(
                name: "pk_transaction_ids",
                table: "transaction_ids",
                columns: new[] { "prefix", "date" });
        }
    }
}
