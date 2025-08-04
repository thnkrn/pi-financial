using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.WalletService.Migrations.Migrations.WalletDb
{
    /// <inheritdoc />
    public partial class AddWalletV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_global_wallet_transfer_state_transaction_no",
                table: "global_wallet_transfer_state");

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
                defaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 82, DateTimeKind.Local).AddTicks(6150),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2023, 11, 1, 16, 28, 24, 200, DateTimeKind.Local).AddTicks(5790));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "transaction_histories",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 99, DateTimeKind.Local).AddTicks(8340),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2023, 11, 1, 16, 28, 24, 203, DateTimeKind.Local).AddTicks(1600));

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

            migrationBuilder.AlterColumn<string>(
                name: "transaction_no",
                table: "global_wallet_transfer_state",
                type: "varchar(36)",
                maxLength: 36,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<string>(
                name: "customer_code",
                table: "global_wallet_transfer_state",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freewill_request_logs",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 99, DateTimeKind.Local).AddTicks(7310),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2023, 11, 1, 16, 28, 24, 203, DateTimeKind.Local).AddTicks(780));

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
                defaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 78, DateTimeKind.Local).AddTicks(3970),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2023, 11, 1, 16, 28, 24, 195, DateTimeKind.Local).AddTicks(7160));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "cash_withdraw_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 76, DateTimeKind.Local).AddTicks(5560),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2023, 11, 1, 16, 28, 24, 200, DateTimeKind.Local).AddTicks(7610));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "cash_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 74, DateTimeKind.Local).AddTicks(9780),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2023, 11, 1, 16, 28, 24, 198, DateTimeKind.Local).AddTicks(1820));

            migrationBuilder.CreateTable(
                name: "activity_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    correlation_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    transaction_no = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transaction_type = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    account_code = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    customer_code = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    channel = table.Column<int>(type: "int", nullable: false),
                    product = table.Column<int>(type: "int", nullable: false),
                    purpose = table.Column<int>(type: "int", nullable: false),
                    state_machine = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    state = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    requested_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    payment_received_date_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    payment_received_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    payment_disbursed_date_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    payment_disbursed_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    payment_confirmed_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    otp_request_ref = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    otp_request_id = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    otp_confirmed_date_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    fee = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    bank_account_no = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_account_name = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_name = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_code = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    deposit_generated_date_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    qr_transaction_no = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    qr_transaction_ref = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    qr_value = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    requested_currency = table.Column<int>(type: "int", nullable: true),
                    requested_amount_with_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    requested_fx_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    requested_fx_currency = table.Column<int>(type: "int", nullable: true),
                    requested_fx_amount_with_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    payment_received_currency = table.Column<int>(type: "int", nullable: true),
                    transfer_fee = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    fx_transaction_id = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    fx_initiate_request_date_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    fx_confirmed_date_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    fx_confirmed_exchange_rate = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    fx_confirmed_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    fx_confirmed_currency = table.Column<int>(type: "int", nullable: true),
                    fx_confirmed_amount_with_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_from_account = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    transfer_to_account = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_currency = table.Column<int>(type: "int", nullable: true),
                    transfer_amount_with_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_request_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    transfer_complete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    failed_reason = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    request_id = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    requester_device_id = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_activity_logs", x => x.id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "deposit_entrypoint_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    current_state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transaction_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    user_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    account_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    customer_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    channel = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    product = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    purpose = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    requested_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: false),
                    net_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    customer_name = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_account_name = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_account_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    refund_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: true, collation: "ascii_general_ci"),
                    failed_reason = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    response_address = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    request_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: true, collation: "ascii_general_ci"),
                    requester_device_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: true, collation: "ascii_general_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 85, DateTimeKind.Local).AddTicks(6060)),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deposit_entrypoint_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "global_transfer_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    current_state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    product = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    channel = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transaction_type = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    customer_id = table.Column<long>(type: "bigint", maxLength: 36, nullable: false),
                    global_account = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    requested_currency = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    requested_fx_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: false),
                    requested_fx_currency = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    payment_received_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    payment_received_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    payment_received_date_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    fx_initiate_request_date_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    fx_transaction_id = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    fx_confirmed_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    fx_confirmed_exchange_rate = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    fx_confirmed_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    fx_confirmed_date_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    transfer_from_account = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    transfer_fee = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    transfer_to_account = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_request_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    transfer_complete_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    failed_reason = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    response_address = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 96, DateTimeKind.Local).AddTicks(520)),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_global_transfer_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "odd_deposit_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    current_state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    product = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    channel = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    payment_received_date_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    payment_received_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    fee = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    otp_request_ref = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    otp_request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    otp_confirmed_date_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    failed_reason = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    response_address = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 87, DateTimeKind.Local).AddTicks(9290)),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_odd_deposit_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "odd_withdraw_state",
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
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 89, DateTimeKind.Local).AddTicks(1300)),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_odd_withdraw_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "qr_deposit_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    current_state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transaction_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    product = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    channel = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    payment_received_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    payment_received_date_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    fee = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    deposit_qr_generate_date_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    qr_code_expired_time_in_minute = table.Column<int>(type: "int", nullable: false),
                    qr_transaction_no = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    qr_value = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    qr_transaction_ref = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    failed_reason = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    response_address = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 86, DateTimeKind.Local).AddTicks(7120)),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_qr_deposit_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "refund_info",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: false),
                    transfer_to_account_no = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    transfer_to_account_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    fee = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 99, DateTimeKind.Local).AddTicks(7720)),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refund_info", x => x.id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "up_back_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    current_state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    product = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    channel = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transaction_type = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    failed_reason = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 89, DateTimeKind.Local).AddTicks(7510)),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_up_back_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "withdraw_entrypoint_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    current_state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transaction_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    user_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    account_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    customer_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    channel = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    product = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    purpose = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    requested_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: false),
                    net_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    customer_name = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_account_name = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_account_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    failed_reason = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    response_address = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    request_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: true, collation: "ascii_general_ci"),
                    requester_device_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: true, collation: "ascii_general_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 97, DateTimeKind.Local).AddTicks(4870)),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_withdraw_entrypoint_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_entrypoint_state_transaction_no",
                table: "deposit_entrypoint_state",
                column: "transaction_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_withdraw_entrypoint_state_transaction_no",
                table: "withdraw_entrypoint_state",
                column: "transaction_no",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity_logs");

            migrationBuilder.DropTable(
                name: "deposit_entrypoint_state");

            migrationBuilder.DropTable(
                name: "global_transfer_state");

            migrationBuilder.DropTable(
                name: "odd_deposit_state");

            migrationBuilder.DropTable(
                name: "odd_withdraw_state");

            migrationBuilder.DropTable(
                name: "qr_deposit_state");

            migrationBuilder.DropTable(
                name: "refund_info");

            migrationBuilder.DropTable(
                name: "up_back_state");

            migrationBuilder.DropTable(
                name: "withdraw_entrypoint_state");

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
                defaultValue: new DateTime(2023, 11, 1, 16, 28, 24, 200, DateTimeKind.Local).AddTicks(5790),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 82, DateTimeKind.Local).AddTicks(6150));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "transaction_histories",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2023, 11, 1, 16, 28, 24, 203, DateTimeKind.Local).AddTicks(1600),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 99, DateTimeKind.Local).AddTicks(8340));

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

            migrationBuilder.AlterColumn<string>(
                name: "transaction_no",
                table: "global_wallet_transfer_state",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36,
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<string>(
                name: "customer_code",
                table: "global_wallet_transfer_state",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36)
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freewill_request_logs",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2023, 11, 1, 16, 28, 24, 203, DateTimeKind.Local).AddTicks(780),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 99, DateTimeKind.Local).AddTicks(7310));

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
                defaultValue: new DateTime(2023, 11, 1, 16, 28, 24, 195, DateTimeKind.Local).AddTicks(7160),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 78, DateTimeKind.Local).AddTicks(3970));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "cash_withdraw_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2023, 11, 1, 16, 28, 24, 200, DateTimeKind.Local).AddTicks(7610),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 76, DateTimeKind.Local).AddTicks(5560));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "cash_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2023, 11, 1, 16, 28, 24, 198, DateTimeKind.Local).AddTicks(1820),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 1, 19, 17, 17, 0, 74, DateTimeKind.Local).AddTicks(9780));

            migrationBuilder.CreateIndex(
                name: "ix_global_wallet_transfer_state_transaction_no",
                table: "global_wallet_transfer_state",
                column: "transaction_no",
                unique: true);
        }
    }
}
