using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.User.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddCitizenIdColumnToUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "citizen_id",
                table: "user_infos",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "citizen_id",
                table: "user_infos");
        }
    }
}
