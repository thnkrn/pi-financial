using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.WalletService.Migrations.Migrations.WalletDb
{
    /// <inheritdoc />
    public partial class GlobalManualAllocateIdToEntryPoint : Migration
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
                defaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 140, DateTimeKind.Local).AddTicks(9910),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 896, DateTimeKind.Local).AddTicks(1120));

            migrationBuilder.AddColumn<Guid>(
                name: "global_manual_allocate_id",
                table: "withdraw_entrypoint_state",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "up_back_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 147, DateTimeKind.Local).AddTicks(1180),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 902, DateTimeKind.Local).AddTicks(6480));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "transaction_histories",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 156, DateTimeKind.Local).AddTicks(2950),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 912, DateTimeKind.Local).AddTicks(2320));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refund_info",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 156, DateTimeKind.Local).AddTicks(2300),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 912, DateTimeKind.Local).AddTicks(1650));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "qr_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 144, DateTimeKind.Local).AddTicks(8320),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 900, DateTimeKind.Local).AddTicks(2540));

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
                defaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 146, DateTimeKind.Local).AddTicks(5710),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 902, DateTimeKind.Local).AddTicks(690));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "odd_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 145, DateTimeKind.Local).AddTicks(7080),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 901, DateTimeKind.Local).AddTicks(1880));

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
                defaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 148, DateTimeKind.Local).AddTicks(9130),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 904, DateTimeKind.Local).AddTicks(4860));

            migrationBuilder.AddColumn<Guid>(
                name: "transaction_id",
                table: "global_manual_allocation_state",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freewill_request_logs",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 156, DateTimeKind.Local).AddTicks(1870),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 912, DateTimeKind.Local).AddTicks(1250));

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
                defaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 133, DateTimeKind.Local).AddTicks(8010),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 887, DateTimeKind.Local).AddTicks(7520));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "deposit_entrypoint_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 143, DateTimeKind.Local).AddTicks(8270),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 899, DateTimeKind.Local).AddTicks(1210));

            migrationBuilder.AddColumn<Guid>(
                name: "global_manual_allocate_id",
                table: "deposit_entrypoint_state",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "global_manual_allocate_id",
                table: "withdraw_entrypoint_state");

            migrationBuilder.DropColumn(
                name: "transaction_id",
                table: "global_manual_allocation_state");

            migrationBuilder.DropColumn(
                name: "global_manual_allocate_id",
                table: "deposit_entrypoint_state");

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
                defaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 896, DateTimeKind.Local).AddTicks(1120),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 140, DateTimeKind.Local).AddTicks(9910));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "up_back_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 902, DateTimeKind.Local).AddTicks(6480),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 147, DateTimeKind.Local).AddTicks(1180));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "transaction_histories",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 912, DateTimeKind.Local).AddTicks(2320),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 156, DateTimeKind.Local).AddTicks(2950));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refund_info",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 912, DateTimeKind.Local).AddTicks(1650),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 156, DateTimeKind.Local).AddTicks(2300));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "qr_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 900, DateTimeKind.Local).AddTicks(2540),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 144, DateTimeKind.Local).AddTicks(8320));

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
                defaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 902, DateTimeKind.Local).AddTicks(690),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 146, DateTimeKind.Local).AddTicks(5710));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "odd_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 901, DateTimeKind.Local).AddTicks(1880),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 145, DateTimeKind.Local).AddTicks(7080));

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
                defaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 904, DateTimeKind.Local).AddTicks(4860),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 148, DateTimeKind.Local).AddTicks(9130));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freewill_request_logs",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 912, DateTimeKind.Local).AddTicks(1250),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 156, DateTimeKind.Local).AddTicks(1870));

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
                defaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 887, DateTimeKind.Local).AddTicks(7520),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 133, DateTimeKind.Local).AddTicks(8010));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "deposit_entrypoint_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 3, 14, 16, 41, 14, 899, DateTimeKind.Local).AddTicks(1210),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 3, 27, 17, 32, 46, 143, DateTimeKind.Local).AddTicks(8270));
        }
    }
}
