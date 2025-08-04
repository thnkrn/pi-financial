using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.User.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddUserInfoPhoneNumberHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "phone_number_hash",
                table: "user_infos",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_user_infos_phone_number_hash",
                table: "user_infos",
                column: "phone_number_hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_infos_phone_number_hash",
                table: "user_infos");

            migrationBuilder.DropColumn(
                name: "phone_number_hash",
                table: "user_infos");
        }
    }
}
