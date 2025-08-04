using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.WalletService.Migrations.Migrations.WalletDb
{
    /// <inheritdoc />
    public partial class AddHolidayTable : Migration
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
                defaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 262, DateTimeKind.Local).AddTicks(2300),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 477, DateTimeKind.Local).AddTicks(3190));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "up_back_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 270, DateTimeKind.Local).AddTicks(2520),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 490, DateTimeKind.Local).AddTicks(5960));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "transaction_histories",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 283, DateTimeKind.Local).AddTicks(640),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 511, DateTimeKind.Local).AddTicks(4580));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refund_info",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 282, DateTimeKind.Local).AddTicks(8920),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 511, DateTimeKind.Local).AddTicks(3700));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "qr_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 267, DateTimeKind.Local).AddTicks(930),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 482, DateTimeKind.Local).AddTicks(670));

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
                defaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 269, DateTimeKind.Local).AddTicks(5450),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 489, DateTimeKind.Local).AddTicks(3040));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "odd_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 268, DateTimeKind.Local).AddTicks(3690),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 485, DateTimeKind.Local).AddTicks(1700));

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
                defaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 272, DateTimeKind.Local).AddTicks(4800),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 493, DateTimeKind.Local).AddTicks(2060));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freewill_request_logs",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 281, DateTimeKind.Local).AddTicks(8530),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 511, DateTimeKind.Local).AddTicks(3060));

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
                defaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 256, DateTimeKind.Local).AddTicks(7190),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 472, DateTimeKind.Local).AddTicks(1780));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "deposit_entrypoint_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 265, DateTimeKind.Local).AddTicks(8040),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 480, DateTimeKind.Local).AddTicks(7490));

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
                oldDefaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 499, DateTimeKind.Local).AddTicks(4980));

            migrationBuilder.CreateTable(
                name: "holidays",
                columns: table => new
                {
                    holiday_date = table.Column<DateOnly>(type: "date", nullable: false),
                    holiday_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    valid = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_holidays", x => x.holiday_date);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.InsertData(
                table: "holidays",
                columns: new[] { "holiday_date", "holiday_name", "valid" },
                values: new object[,]
                {
                    { new DateOnly(2024, 1, 1), "New Year's Day", true },
                    { new DateOnly(2024, 1, 2), "Substitution for New Year’s Eve (Sunday 31st December 2023) (cancelled)", true },
                    { new DateOnly(2024, 2, 26), "Substitution for Makha Bucha Day (Saturday 24th February 2024)", true },
                    { new DateOnly(2024, 4, 8), "Substitution for Chakri Memorial Day (Saturday 6th April 2024)", true },
                    { new DateOnly(2024, 4, 12), "Additional special holiday (added)", true },
                    { new DateOnly(2024, 4, 15), "Songkran Festival", true },
                    { new DateOnly(2024, 4, 16), "Substitution for Songkran Festival (Saturday 13th April 2024 and Sunday 14th April 2024)", true },
                    { new DateOnly(2024, 5, 1), "National Labour Day", true },
                    { new DateOnly(2024, 5, 6), "Substitution for Coronation Day (Saturday 4th May 2024)", true },
                    { new DateOnly(2024, 5, 22), "Visakha Bucha Day", true },
                    { new DateOnly(2024, 6, 3), "H.M. Queen Suthida Bajrasudhabimalalakshana’s Birthday", true },
                    { new DateOnly(2024, 7, 22), "Substitution for Asarnha Bucha Day (Saturday 20th July 2024)", true },
                    { new DateOnly(2024, 7, 29), "Substitution for H.M. King Maha Vajiralongkorn Phra Vajiraklaochaoyuhua’s Birthday (Sunday 28th July 2024)", true },
                    { new DateOnly(2024, 8, 12), "H.M. Queen Sirikit The Queen Mother’s Birthday / Mother’s Day", true },
                    { new DateOnly(2024, 10, 14), "Substitution for H.M. King Bhumibol Adulyadej The Great Memorial Day (Sunday 13th October 2024)", true },
                    { new DateOnly(2024, 10, 23), "H.M. King Chulalongkorn The Great Memorial Day", true },
                    { new DateOnly(2024, 12, 5), "H.M. King Bhumibol Adulyadej The Great’s Birthday / National Day / Father’s Day", true },
                    { new DateOnly(2024, 12, 10), "Constitution Day", true },
                    { new DateOnly(2024, 12, 31), "New Year’s Eve", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "holidays");

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
                defaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 477, DateTimeKind.Local).AddTicks(3190),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 262, DateTimeKind.Local).AddTicks(2300));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "up_back_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 490, DateTimeKind.Local).AddTicks(5960),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 270, DateTimeKind.Local).AddTicks(2520));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "transaction_histories",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 511, DateTimeKind.Local).AddTicks(4580),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 283, DateTimeKind.Local).AddTicks(640));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refund_info",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 511, DateTimeKind.Local).AddTicks(3700),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 282, DateTimeKind.Local).AddTicks(8920));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "qr_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 482, DateTimeKind.Local).AddTicks(670),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 267, DateTimeKind.Local).AddTicks(930));

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
                defaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 489, DateTimeKind.Local).AddTicks(3040),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 269, DateTimeKind.Local).AddTicks(5450));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "odd_deposit_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 485, DateTimeKind.Local).AddTicks(1700),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 268, DateTimeKind.Local).AddTicks(3690));

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
                defaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 493, DateTimeKind.Local).AddTicks(2060),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 272, DateTimeKind.Local).AddTicks(4800));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freewill_request_logs",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 511, DateTimeKind.Local).AddTicks(3060),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 281, DateTimeKind.Local).AddTicks(8530));

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
                defaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 472, DateTimeKind.Local).AddTicks(1780),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 256, DateTimeKind.Local).AddTicks(7190));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "deposit_entrypoint_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 480, DateTimeKind.Local).AddTicks(7490),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 265, DateTimeKind.Local).AddTicks(8040));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "ats_withdraw_state",
                type: "datetime(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new DateTime(2024, 4, 26, 13, 31, 25, 499, DateTimeKind.Local).AddTicks(4980),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldMaxLength: 6,
                oldDefaultValue: new DateTime(2024, 5, 13, 15, 37, 18, 277, DateTimeKind.Local).AddTicks(1700));
        }
    }
}
