#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class RemoveOutdatedResponseCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "response_codes",
                new[] { "state" },
                new object[,]
                {
                    {"NotApprovedFrontOffice"},
                    {"LockTableBackOffice"},
                    {"ConnectionTimeOut"},
                    {"DepositGenerateQrCodeFailed"},
                    {"InternalServerError"},
                    {"KKPWithdrawalFailed"},
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
