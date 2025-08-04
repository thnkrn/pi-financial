using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.WalletService.Migrations.Migrations.WalletDb
{
    /// <inheritdoc />
    public partial class AddTransactionView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                CREATE OR REPLACE VIEW deposit_transactions AS
                SELECT
	                des.transaction_no,
	                des.current_state,
	                des.channel,
	                des.product,
	                des.requested_amount as 'amount',
	                gts.transfer_amount as 'global_transfer_amount',
                    CASE
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 002%' THEN 'Freewill Failed (002)'
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 003%' THEN 'Freewill Failed (003)'
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 005%' THEN 'Freewill Failed (005)'
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 008%' THEN 'Freewill Failed (008)'
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 023%' THEN 'Freewill Failed (023)'
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 900%' THEN 'Freewill Failed (900)'
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 906%' THEN 'Freewill Failed (906)'
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 999%' THEN 'Freewill Failed (999)'
                        WHEN ubs.failed_reason LIKE 'SetTrade%' THEN 'SetTrade Failed'
                        ELSE ubs.failed_reason
                    END AS 'upback_failed_reason',
	                des.created_at
                FROM deposit_entrypoint_state des
                LEFT JOIN up_back_state ubs ON des.correlation_id = ubs.correlation_id
                LEFT JOIN global_transfer_state gts ON des.correlation_id = gts.correlation_id
                WHERE des.transaction_no IS NOT NULL;

                CREATE OR REPLACE VIEW withdraw_transactions AS
                SELECT
	                wes.transaction_no,
	                wes.current_state,
	                wes.channel,
	                wes.product,
	                CASE
		                WHEN wes.product = 'GlobalEquities' THEN wes.requested_amount * gts.requested_fx_rate
		                ELSE wes.requested_amount
	                END AS 'amount',
                    gts.transfer_amount AS 'global_transfer_amount',
                    CASE
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 002%' THEN 'Freewill Failed (002)'
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 003%' THEN 'Freewill Failed (003)'
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 005%' THEN 'Freewill Failed (005)'
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 008%' THEN 'Freewill Failed (008)'
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 023%' THEN 'Freewill Failed (023)'
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 900%' THEN 'Freewill Failed (900)'
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 906%' THEN 'Freewill Failed (906)'
                        WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 999%' THEN 'Freewill Failed (999)'
                        WHEN ubs.failed_reason LIKE 'SetTrade%' THEN 'SetTrade Failed'
                        ELSE ubs.failed_reason
                    END AS 'upback_failed_reason',
	                wes.created_at
                FROM withdraw_entrypoint_state wes
                LEFT JOIN up_back_state ubs ON wes.correlation_id = ubs.correlation_id
                LEFT JOIN global_transfer_state gts ON wes.correlation_id = gts.correlation_id
                WHERE wes.transaction_no IS NOT NULL;

                CREATE OR REPLACE VIEW activity_view AS
                SELECT
                    al.correlation_id,
                    al.transaction_no,
                    al.transaction_type,
                    al.product,
                    al.channel,
                    al.state_machine,
                    al.state,
                    al.requested_amount,
                    al.payment_received_date_time,
                    al.payment_received_amount,
                    al.payment_disbursed_date_time,
                    al.payment_disbursed_amount,
                    al.failed_reason,
                    al.created_at,
                    al.updated_at
                FROM activity_logs al;");

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
                defaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 820, DateTimeKind.Local).AddTicks(3440),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 885, DateTimeKind.Local).AddTicks(3920));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "up_back_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 835, DateTimeKind.Local).AddTicks(6640),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 893, DateTimeKind.Local).AddTicks(2310));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "transaction_histories",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 844, DateTimeKind.Local).AddTicks(8490),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 905, DateTimeKind.Local).AddTicks(9990));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refund_info",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 844, DateTimeKind.Local).AddTicks(6840),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 905, DateTimeKind.Local).AddTicks(8330));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "qr_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 824, DateTimeKind.Local).AddTicks(5620),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 890, DateTimeKind.Local).AddTicks(2910));

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
                defaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 835, DateTimeKind.Local).AddTicks(430),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 892, DateTimeKind.Local).AddTicks(5580));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "odd_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 833, DateTimeKind.Local).AddTicks(6510),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 891, DateTimeKind.Local).AddTicks(4470));

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
                defaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 837, DateTimeKind.Local).AddTicks(6950),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 895, DateTimeKind.Local).AddTicks(7500));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freewill_request_logs",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 843, DateTimeKind.Local).AddTicks(8300),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 904, DateTimeKind.Local).AddTicks(9140));

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
                defaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 815, DateTimeKind.Local).AddTicks(7190),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 880, DateTimeKind.Local).AddTicks(5360));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "deposit_entrypoint_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 823, DateTimeKind.Local).AddTicks(4310),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 888, DateTimeKind.Local).AddTicks(8730));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "ats_withdraw_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 841, DateTimeKind.Local).AddTicks(4840),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 902, DateTimeKind.Local).AddTicks(2230));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                DROP VIEW IF EXISTS deposit_transactions;
                DROP VIEW IF EXISTS withdraw_transactions;
                DROP VIEW IF EXISTS activity_view;
                ");

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
                defaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 885, DateTimeKind.Local).AddTicks(3920),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 820, DateTimeKind.Local).AddTicks(3440));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "up_back_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 893, DateTimeKind.Local).AddTicks(2310),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 835, DateTimeKind.Local).AddTicks(6640));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "transaction_histories",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 905, DateTimeKind.Local).AddTicks(9990),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 844, DateTimeKind.Local).AddTicks(8490));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refund_info",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 905, DateTimeKind.Local).AddTicks(8330),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 844, DateTimeKind.Local).AddTicks(6840));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "qr_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 890, DateTimeKind.Local).AddTicks(2910),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 824, DateTimeKind.Local).AddTicks(5620));

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
                defaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 892, DateTimeKind.Local).AddTicks(5580),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 835, DateTimeKind.Local).AddTicks(430));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "odd_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 891, DateTimeKind.Local).AddTicks(4470),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 833, DateTimeKind.Local).AddTicks(6510));

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
                defaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 895, DateTimeKind.Local).AddTicks(7500),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 837, DateTimeKind.Local).AddTicks(6950));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freewill_request_logs",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 904, DateTimeKind.Local).AddTicks(9140),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 843, DateTimeKind.Local).AddTicks(8300));

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
                defaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 880, DateTimeKind.Local).AddTicks(5360),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 815, DateTimeKind.Local).AddTicks(7190));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "deposit_entrypoint_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 888, DateTimeKind.Local).AddTicks(8730),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 823, DateTimeKind.Local).AddTicks(4310));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "ats_withdraw_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 27, 17, 2, 51, 902, DateTimeKind.Local).AddTicks(2230),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 28, 18, 31, 18, 841, DateTimeKind.Local).AddTicks(4840));
        }
    }
}
