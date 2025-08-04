using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.User.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationPropertyInDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cust_codes_user_infos_user_info_id",
                table: "cust_codes");

            migrationBuilder.DropForeignKey(
                name: "fk_devices_user_infos_user_info_id",
                table: "devices");

            migrationBuilder.DropForeignKey(
                name: "fk_notification_preferences_user_infos_user_info_id",
                table: "notification_preferences");

            migrationBuilder.DropForeignKey(
                name: "fk_trading_accounts_user_infos_user_info_id",
                table: "trading_accounts");

            migrationBuilder.AddForeignKey(
                name: "fk_cust_codes_user_infos_user_info_id1",
                table: "cust_codes",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_devices_user_infos_user_info_id1",
                table: "devices",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_notification_preferences_user_infos_user_info_id1",
                table: "notification_preferences",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_trading_accounts_user_infos_user_info_id1",
                table: "trading_accounts",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cust_codes_user_infos_user_info_id1",
                table: "cust_codes");

            migrationBuilder.DropForeignKey(
                name: "fk_devices_user_infos_user_info_id1",
                table: "devices");

            migrationBuilder.DropForeignKey(
                name: "fk_notification_preferences_user_infos_user_info_id1",
                table: "notification_preferences");

            migrationBuilder.DropForeignKey(
                name: "fk_trading_accounts_user_infos_user_info_id1",
                table: "trading_accounts");

            migrationBuilder.AddForeignKey(
                name: "fk_cust_codes_user_infos_user_info_id",
                table: "cust_codes",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_devices_user_infos_user_info_id",
                table: "devices",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_notification_preferences_user_infos_user_info_id",
                table: "notification_preferences",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_trading_accounts_user_infos_user_info_id",
                table: "trading_accounts",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");
        }
    }
}
