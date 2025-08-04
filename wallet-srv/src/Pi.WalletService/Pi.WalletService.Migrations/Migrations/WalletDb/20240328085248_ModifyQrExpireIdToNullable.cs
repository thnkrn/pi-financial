using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.WalletService.Migrations.Migrations.WalletDb
{
    /// <inheritdoc />
    public partial class ModifyQrExpireIdToNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "row_version",
                table: "withdraw_state",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "withdraw_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 481, DateTimeKind.Local).AddTicks(3770),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 241, DateTimeKind.Local).AddTicks(9620));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "up_back_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 489, DateTimeKind.Local).AddTicks(3610),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 248, DateTimeKind.Local).AddTicks(3450));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "transaction_histories",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 500, DateTimeKind.Local).AddTicks(7820),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 257, DateTimeKind.Local).AddTicks(8290));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refund_info",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 500, DateTimeKind.Local).AddTicks(7000),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 257, DateTimeKind.Local).AddTicks(7640));

            migrationBuilder.AlterColumn<Guid>(
                name: "qr_expire_token_id",
                table: "qr_deposit_state",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "qr_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 486, DateTimeKind.Local).AddTicks(2560),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 245, DateTimeKind.Local).AddTicks(9960));

            migrationBuilder.AlterColumn<DateTime>(
                name: "row_version",
                table: "outbox_state",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "odd_withdraw_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 488, DateTimeKind.Local).AddTicks(6850),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 247, DateTimeKind.Local).AddTicks(7780));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "odd_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 487, DateTimeKind.Local).AddTicks(5560),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 246, DateTimeKind.Local).AddTicks(9050));

            migrationBuilder.AlterColumn<DateTime>(
                name: "row_version",
                table: "inbox_state",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "global_transfer_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 491, DateTimeKind.Local).AddTicks(5630),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 250, DateTimeKind.Local).AddTicks(2060));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freewill_request_logs",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 500, DateTimeKind.Local).AddTicks(6470),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 257, DateTimeKind.Local).AddTicks(7190));

            migrationBuilder.AlterColumn<DateTime>(
                name: "row_version",
                table: "deposit_state",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 473, DateTimeKind.Local).AddTicks(260),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 234, DateTimeKind.Local).AddTicks(2560));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "deposit_entrypoint_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 484, DateTimeKind.Local).AddTicks(9150),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 244, DateTimeKind.Local).AddTicks(9290));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "row_version",
                table: "withdraw_state",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "withdraw_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 241, DateTimeKind.Local).AddTicks(9620),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 481, DateTimeKind.Local).AddTicks(3770));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "up_back_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 248, DateTimeKind.Local).AddTicks(3450),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 489, DateTimeKind.Local).AddTicks(3610));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "transaction_histories",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 257, DateTimeKind.Local).AddTicks(8290),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 500, DateTimeKind.Local).AddTicks(7820));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refund_info",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 257, DateTimeKind.Local).AddTicks(7640),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 500, DateTimeKind.Local).AddTicks(7000));

            migrationBuilder.AlterColumn<Guid>(
                name: "qr_expire_token_id",
                table: "qr_deposit_state",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "qr_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 245, DateTimeKind.Local).AddTicks(9960),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 486, DateTimeKind.Local).AddTicks(2560));

            migrationBuilder.AlterColumn<DateTime>(
                name: "row_version",
                table: "outbox_state",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "odd_withdraw_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 247, DateTimeKind.Local).AddTicks(7780),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 488, DateTimeKind.Local).AddTicks(6850));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "odd_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 246, DateTimeKind.Local).AddTicks(9050),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 487, DateTimeKind.Local).AddTicks(5560));

            migrationBuilder.AlterColumn<DateTime>(
                name: "row_version",
                table: "inbox_state",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "global_transfer_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 250, DateTimeKind.Local).AddTicks(2060),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 491, DateTimeKind.Local).AddTicks(5630));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freewill_request_logs",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 257, DateTimeKind.Local).AddTicks(7190),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 500, DateTimeKind.Local).AddTicks(6470));

            migrationBuilder.AlterColumn<DateTime>(
                name: "row_version",
                table: "deposit_state",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6)",
                oldRowVersion: true,
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 234, DateTimeKind.Local).AddTicks(2560),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 473, DateTimeKind.Local).AddTicks(260));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "deposit_entrypoint_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 28, 15, 38, 48, 244, DateTimeKind.Local).AddTicks(9290),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 28, 15, 52, 48, 484, DateTimeKind.Local).AddTicks(9150));
        }
    }
}
