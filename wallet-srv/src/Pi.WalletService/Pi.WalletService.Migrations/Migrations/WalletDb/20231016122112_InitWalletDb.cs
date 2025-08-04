using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.WalletService.Migrations.Migrations.WalletDb
{
    /// <inheritdoc />
    public partial class InitWalletDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cash_deposit_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    transaction_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    user_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    account_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    customer_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    channel = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    product = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    purpose = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    current_state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    requested_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: false),
                    payment_received_date_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    bank_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    failed_reason = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2023, 10, 16, 19, 21, 12, 36, DateTimeKind.Local).AddTicks(8350)),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cash_deposit_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "cash_withdraw_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    transaction_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    user_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    account_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    customer_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    channel = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    product = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    current_state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_fee = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    requested_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    bank_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_account_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    otp_request_ref = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    otp_request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    otp_confirmed_date_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    device_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    failed_reason = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    response_address = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2023, 10, 16, 19, 21, 12, 39, DateTimeKind.Local).AddTicks(1340)),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cash_withdraw_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "deposit_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    transaction_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    user_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    account_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    customer_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    channel = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    product = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    purpose = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    current_state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    requested_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: false),
                    bank_fee = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    payment_received_date_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    payment_received_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    customer_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_account_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_account_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    deposit_qr_generate_date_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    qr_code_expired_time_in_minute = table.Column<int>(type: "int", nullable: false),
                    qr_transaction_no = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    qr_value = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    qr_transaction_ref = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    failed_reason = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2023, 10, 16, 19, 21, 12, 34, DateTimeKind.Local).AddTicks(3900)),
                    response_address = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    requester_device_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deposit_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "freewill_request_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    refer_id = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    trans_id = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    request = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    response = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    callback = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    type = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2023, 10, 16, 19, 21, 12, 30, DateTimeKind.Local).AddTicks(3540)),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_freewill_request_logs", x => x.id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "global_manual_allocation_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    current_state = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transaction_no = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    global_account = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    currency = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    initiate_transfer_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    completed_transfer_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    response_address = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    request_type = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    failed_reason = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_global_manual_allocation_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "global_wallet_transaction_histories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    correlation_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    transaction_no = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transaction_type = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    customer_code = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    global_account = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    current_state = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    requested_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    requested_currency = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    requested_amount_with_currency = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    requested_fx_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    requested_fx_currency = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    requested_fx_amount_with_currency = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    payment_received_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    payment_received_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transaction_fee = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    fx_transaction_id = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    fx_initiate_request_date_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    fx_confirmed_date_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    fx_confirmed_exchange_rate = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    fx_confirmed_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    fx_confirmed_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    fx_confirmed_amount_with_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_from_account = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    transfer_to_account = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_amount_with_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_request_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    transfer_complete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    failed_reason = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    refund_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    net_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    requester_device_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_global_wallet_transaction_histories", x => x.id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "global_wallet_transfer_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    transaction_no = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transaction_type = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    user_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    customer_code = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    global_account = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    current_state = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    product = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    requested_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    requested_currency = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    requested_fx_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    requested_fx_currency = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    payment_received_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    payment_received_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    fx_initiate_request_date_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    fx_transaction_id = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    fx_confirmed_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    fx_confirmed_exchange_rate = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    fx_confirmed_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    fx_confirmed_date_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    transfer_from_account = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    transfer_fee = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    transfer_to_account = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_currency = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transfer_request_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    transfer_complete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    failed_reason = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    refund_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    net_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    response_address = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    requester_device_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_global_wallet_transfer_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "online_direct_debit_registrations",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    bank = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_success = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    external_status_code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_online_direct_debit_registrations", x => x.id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "refund_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    deposit_transaction_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    transaction_no = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    user_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    account_code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    customer_code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    channel = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    product = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    current_state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    bank_account_no = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_fee = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    refunded_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    response_address = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    failed_reason = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refund_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "transaction_histories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    correlation_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    transaction_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    transaction_type = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    user_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    account_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    customer_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    global_account = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    channel = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    product = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    purpose = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    requested_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: false),
                    bank_fee = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    transaction_date_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    transaction_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    customer_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_account_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_account_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    failed_reason = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2023, 10, 16, 19, 21, 12, 30, DateTimeKind.Local).AddTicks(4300)),
                    requester_device_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transaction_histories", x => x.id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "withdraw_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    transaction_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    user_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    account_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    customer_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    channel = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    product = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    current_state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_fee = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: false, defaultValue: new DateTime(2023, 10, 16, 19, 21, 12, 38, DateTimeKind.Local).AddTicks(9770)),
                    payment_disbursed_date_time = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    payment_disbursed_amount = table.Column<decimal>(type: "decimal(65,30)", precision: 65, scale: 30, nullable: true),
                    payment_confirmed_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    bank_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    bank_account_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    otp_request_ref = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    otp_request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    otp_confirmed_date_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    failed_reason = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    response_address = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    requester_device_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_withdraw_state", x => x.correlation_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "ix_cash_deposit_state_transaction_no",
                table: "cash_deposit_state",
                column: "transaction_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cash_withdraw_state_transaction_no",
                table: "cash_withdraw_state",
                column: "transaction_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_deposit_state_transaction_no",
                table: "deposit_state",
                column: "transaction_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_global_manual_allocation_state_transaction_no",
                table: "global_manual_allocation_state",
                column: "transaction_no");

            migrationBuilder.CreateIndex(
                name: "ix_global_wallet_transfer_state_transaction_no",
                table: "global_wallet_transfer_state",
                column: "transaction_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_withdraw_state_transaction_no",
                table: "withdraw_state",
                column: "transaction_no",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cash_deposit_state");

            migrationBuilder.DropTable(
                name: "cash_withdraw_state");

            migrationBuilder.DropTable(
                name: "deposit_state");

            migrationBuilder.DropTable(
                name: "freewill_request_logs");

            migrationBuilder.DropTable(
                name: "global_manual_allocation_state");

            migrationBuilder.DropTable(
                name: "global_wallet_transaction_histories");

            migrationBuilder.DropTable(
                name: "global_wallet_transfer_state");

            migrationBuilder.DropTable(
                name: "online_direct_debit_registrations");

            migrationBuilder.DropTable(
                name: "refund_state");

            migrationBuilder.DropTable(
                name: "transaction_histories");

            migrationBuilder.DropTable(
                name: "withdraw_state");
        }
    }
}
