#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class UpdateProductTypeToNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(name: "product_type", table: "response_codes", nullable: true);
            migrationBuilder.UpdateData("response_codes", "product_type", "Common", "product_type", null);
            migrationBuilder.UpdateData("response_codes", "product_type", "", "product_type", null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(name: "product_type", table: "response_codes", nullable: false);
            migrationBuilder.UpdateData("response_codes", "product_type", null, "product_type", "Common");
        }
    }
}
