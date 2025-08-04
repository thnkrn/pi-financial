using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.WalletService.Migrations.Migrations.WalletDb
{
    /// <inheritdoc />
    public partial class ReviseGlobalTransferColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pre_mark_up_confirmed_exchange_rate",
                table: "global_transfer_state");

            migrationBuilder.DropColumn(
                name: "pre_mark_up_requested_fx_rate",
                table: "global_transfer_state");

            migrationBuilder.RenameColumn(
                name: "payment_received_currency",
                table: "global_transfer_state",
                newName: "exchange_currency");

            migrationBuilder.RenameColumn(
                name: "payment_received_amount",
                table: "global_transfer_state",
                newName: "exchange_amount");

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
                defaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 709, DateTimeKind.Local).AddTicks(8690),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 806, DateTimeKind.Local).AddTicks(8000));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "up_back_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 717, DateTimeKind.Local).AddTicks(9490),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 822, DateTimeKind.Local).AddTicks(3210));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "transaction_histories",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 730, DateTimeKind.Local).AddTicks(9370),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 834, DateTimeKind.Local).AddTicks(4440));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refund_info",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 730, DateTimeKind.Local).AddTicks(7630),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 834, DateTimeKind.Local).AddTicks(3680));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "qr_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 714, DateTimeKind.Local).AddTicks(8750),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 811, DateTimeKind.Local).AddTicks(380));

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
                defaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 717, DateTimeKind.Local).AddTicks(2560),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 821, DateTimeKind.Local).AddTicks(6220));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "odd_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 716, DateTimeKind.Local).AddTicks(580),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 812, DateTimeKind.Local).AddTicks(150));

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
                defaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 720, DateTimeKind.Local).AddTicks(4600),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 824, DateTimeKind.Local).AddTicks(5160));

            migrationBuilder.AddColumn<decimal>(
                name: "fx_confirmed_exchange_amount",
                table: "global_transfer_state",
                type: "decimal(65,30)",
                precision: 65,
                scale: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "fx_confirmed_exchange_currency",
                table: "global_transfer_state",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freewill_request_logs",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 729, DateTimeKind.Local).AddTicks(8370),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 834, DateTimeKind.Local).AddTicks(3260));

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
                defaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 704, DateTimeKind.Local).AddTicks(7720),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 802, DateTimeKind.Local).AddTicks(1690));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "deposit_entrypoint_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 713, DateTimeKind.Local).AddTicks(4960),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 809, DateTimeKind.Local).AddTicks(8970));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "ats_withdraw_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 727, DateTimeKind.Local).AddTicks(2260),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 277, DateTimeKind.Local).AddTicks(1700));

            migrationBuilder.AddColumn<decimal>(
                name: "exchange_amount",
                table: "activity_logs",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "exchange_currency",
                table: "activity_logs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fx_confirmed_exchange_currency",
                table: "global_transfer_state");

            migrationBuilder.DropColumn(
                name: "fx_confirmed_exchange_amount",
                table: "global_transfer_state");

            migrationBuilder.DropColumn(
                name: "exchange_amount",
                table: "activity_logs");

            migrationBuilder.DropColumn(
                name: "exchange_currency",
                table: "activity_logs");

            migrationBuilder.RenameColumn(
                name: "exchange_currency",
                table: "global_transfer_state",
                newName: "payment_received_currency");

            migrationBuilder.RenameColumn(
                name: "exchange_amount",
                table: "global_transfer_state",
                newName: "payment_received_amount");

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
                defaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 806, DateTimeKind.Local).AddTicks(8000),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 709, DateTimeKind.Local).AddTicks(8690));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "up_back_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 822, DateTimeKind.Local).AddTicks(3210),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 717, DateTimeKind.Local).AddTicks(9490));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "transaction_histories",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 834, DateTimeKind.Local).AddTicks(4440),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 730, DateTimeKind.Local).AddTicks(9370));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refund_info",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 834, DateTimeKind.Local).AddTicks(3680),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 730, DateTimeKind.Local).AddTicks(7630));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "qr_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 811, DateTimeKind.Local).AddTicks(380),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 714, DateTimeKind.Local).AddTicks(8750));

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
                defaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 821, DateTimeKind.Local).AddTicks(6220),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 717, DateTimeKind.Local).AddTicks(2560));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "odd_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 812, DateTimeKind.Local).AddTicks(150),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 716, DateTimeKind.Local).AddTicks(580));

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
                defaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 824, DateTimeKind.Local).AddTicks(5160),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 720, DateTimeKind.Local).AddTicks(4600));

            migrationBuilder.AddColumn<decimal>(
                name: "pre_mark_up_confirmed_exchange_rate",
                table: "global_transfer_state",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "pre_mark_up_requested_fx_rate",
                table: "global_transfer_state",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freewill_request_logs",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 834, DateTimeKind.Local).AddTicks(3260),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 729, DateTimeKind.Local).AddTicks(8370));

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
                defaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 802, DateTimeKind.Local).AddTicks(1690),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 704, DateTimeKind.Local).AddTicks(7720));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "deposit_entrypoint_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 11, 4, 51, 809, DateTimeKind.Local).AddTicks(8970),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 713, DateTimeKind.Local).AddTicks(4960));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "ats_withdraw_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 277, DateTimeKind.Local).AddTicks(1700),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 24, 14, 35, 19, 727, DateTimeKind.Local).AddTicks(2260));
        }
    }
}
