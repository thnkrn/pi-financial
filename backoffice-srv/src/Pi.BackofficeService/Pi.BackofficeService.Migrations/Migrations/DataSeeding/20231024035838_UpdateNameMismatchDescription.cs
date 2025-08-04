#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class UpdateNameMismatchDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("6a5113c7-5381-40a8-b49f-b1751c44d22b"),
                column: "description",
                value: "Name Mismatch");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f3c9ed99-0978-4797-ae3d-3d0ef7854caa"),
                column: "description",
                value: "Name Mismatch");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("6a5113c7-5381-40a8-b49f-b1751c44d22b"),
                column: "description",
                value: "Name Mismatch (Thai Equity)");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f3c9ed99-0978-4797-ae3d-3d0ef7854caa"),
                column: "description",
                value: "Name Mismatch (Global Equity)");
        }
    }
}
