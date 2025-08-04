using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.SetService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitSetDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "equity_order_state",
                columns: table => new
                {
                    correlationid = table.Column<Guid>(name: "correlation_id", type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    currentstate = table.Column<string>(name: "current_state", type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tradingaccountid = table.Column<Guid>(name: "trading_account_id", type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tradingaccountno = table.Column<string>(name: "trading_account_no", type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    customercode = table.Column<string>(name: "customer_code", type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    enterid = table.Column<string>(name: "enter_id", type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    orderno = table.Column<string>(name: "order_no", type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    brokerorderid = table.Column<string>(name: "broker_order_id", type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    conditionprice = table.Column<string>(name: "condition_price", type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    brokerstatus = table.Column<string>(name: "broker_status", type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(65,30)", maxLength: 64, nullable: true),
                    volume = table.Column<int>(type: "int", nullable: false),
                    pubvolume = table.Column<int>(name: "pub_volume", type: "int", nullable: false),
                    matchedvolume = table.Column<int>(name: "matched_volume", type: "int", nullable: true),
                    side = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    secsymbol = table.Column<string>(name: "sec_symbol", type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    condition = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trusteeid = table.Column<string>(name: "trustee_id", type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    servicetype = table.Column<string>(name: "service_type", type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    life = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ttf = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    todaysell = table.Column<bool>(name: "today_sell", type: "tinyint(1)", nullable: true),
                    stocktype = table.Column<string>(name: "stock_type", type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ipaddress = table.Column<string>(name: "ip_address", type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    failedreason = table.Column<string>(name: "failed_reason", type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    responseaddress = table.Column<string>(name: "response_address", type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    requestid = table.Column<Guid>(name: "request_id", type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "datetime(6)", maxLength: 6, nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "datetime(6)", maxLength: 6, nullable: true, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equity_order_state", x => x.correlationid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "number_generators",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    module = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    prefix = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    currentcounter = table.Column<int>(name: "current_counter", type: "int", nullable: false),
                    dailyreset = table.Column<bool>(name: "daily_reset", type: "tinyint(1)", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_number_generators", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "equity_order_state");

            migrationBuilder.DropTable(
                name: "number_generators");
        }
    }
}
