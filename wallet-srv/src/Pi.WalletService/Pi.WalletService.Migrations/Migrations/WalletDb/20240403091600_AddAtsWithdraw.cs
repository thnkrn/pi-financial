using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.WalletService.Migrations.Migrations.WalletDb
{
    /// <inheritdoc />
    public partial class AddAtsWithdraw : Migration
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
                defaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 516, DateTimeKind.Local).AddTicks(1990),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 597, DateTimeKind.Local).AddTicks(2830));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "up_back_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 529, DateTimeKind.Local).AddTicks(2630),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 605, DateTimeKind.Local).AddTicks(9540));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "transaction_histories",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 539, DateTimeKind.Local).AddTicks(4700),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 619, DateTimeKind.Local).AddTicks(3400));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refund_info",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 539, DateTimeKind.Local).AddTicks(4120),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 619, DateTimeKind.Local).AddTicks(2520));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "qr_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 519, DateTimeKind.Local).AddTicks(7850),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 602, DateTimeKind.Local).AddTicks(4730));

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
                defaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 528, DateTimeKind.Local).AddTicks(7230),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 605, DateTimeKind.Local).AddTicks(1790));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "odd_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 520, DateTimeKind.Local).AddTicks(6510),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 603, DateTimeKind.Local).AddTicks(8820));

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
                defaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 530, DateTimeKind.Local).AddTicks(9140),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 608, DateTimeKind.Local).AddTicks(2700));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freewill_request_logs",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 539, DateTimeKind.Local).AddTicks(3750),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 619, DateTimeKind.Local).AddTicks(1970));

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
                defaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 512, DateTimeKind.Local).AddTicks(600),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 591, DateTimeKind.Local).AddTicks(6390));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "deposit_entrypoint_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 518, DateTimeKind.Local).AddTicks(8000),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 601, DateTimeKind.Local).AddTicks(590));

            migrationBuilder.CreateTable(
                name: "ats_withdraw_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    current_state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    product = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    channel = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    payment_disbursed_date_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    payment_disbursed_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    fee = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    otp_request_ref = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    otp_request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    otp_confirmed_date_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    failed_reason = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    response_address = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    otp_validation_expire_token_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 534, DateTimeKind.Local).AddTicks(2230)),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ats_withdraw_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "ix_ats_withdraw_state_correlation_id",
                table: "ats_withdraw_state",
                column: "correlation_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ats_withdraw_state");

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
                defaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 597, DateTimeKind.Local).AddTicks(2830),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 516, DateTimeKind.Local).AddTicks(1990));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "up_back_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 605, DateTimeKind.Local).AddTicks(9540),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 529, DateTimeKind.Local).AddTicks(2630));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "transaction_histories",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 619, DateTimeKind.Local).AddTicks(3400),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 539, DateTimeKind.Local).AddTicks(4700));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refund_info",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 619, DateTimeKind.Local).AddTicks(2520),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 539, DateTimeKind.Local).AddTicks(4120));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "qr_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 602, DateTimeKind.Local).AddTicks(4730),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 519, DateTimeKind.Local).AddTicks(7850));

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
                defaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 605, DateTimeKind.Local).AddTicks(1790),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 528, DateTimeKind.Local).AddTicks(7230));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "odd_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 603, DateTimeKind.Local).AddTicks(8820),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 520, DateTimeKind.Local).AddTicks(6510));

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
                defaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 608, DateTimeKind.Local).AddTicks(2700),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 530, DateTimeKind.Local).AddTicks(9140));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freewill_request_logs",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 619, DateTimeKind.Local).AddTicks(1970),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 539, DateTimeKind.Local).AddTicks(3750));

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
                defaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 591, DateTimeKind.Local).AddTicks(6390),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 512, DateTimeKind.Local).AddTicks(600));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "deposit_entrypoint_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 3, 13, 46, 36, 601, DateTimeKind.Local).AddTicks(590),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 3, 16, 16, 0, 518, DateTimeKind.Local).AddTicks(8000));
        }
    }
}
