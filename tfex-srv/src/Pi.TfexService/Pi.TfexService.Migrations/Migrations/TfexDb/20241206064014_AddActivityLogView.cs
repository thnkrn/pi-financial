using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.TfexService.Migrations.Migrations.TfexDb
{
    /// <inheritdoc />
    public partial class AddActivityLogView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                CREATE OR REPLACE VIEW activities_logs_view AS
                SELECT
                    al.user_id,
                    al.customer_code,
                    al.account_code,
                    al.order_no,
                    al.request_type,
                    al.request_body,
                    al.response_body,
                    al.is_success,
                    al.failed_reason,
                    al.symbol,
                    al.side,
                    al.price_type,
                    al.price,
                    al.qty,
                    al.reject_code,
                    al.reject_reason,
                    al.requested_at,
                    al.completed_at,
                    al.created_at,
                    al.updated_at
                FROM activities_logs al;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                DROP VIEW IF EXISTS activities_logs_view;
                ");
        }
    }
}
