using Microsoft.EntityFrameworkCore.Migrations;
using Pi.BackofficeService.Application.Models;

#nullable disable

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class AddMissingBankChannels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            var channels = new Dictionary<string, string[]>()
            {
                {WithdrawChannel.AtsBatch.ToString(), new []{"022","024"}},
            };

            foreach (var (key, bankCodes) in channels)
            {
                foreach (var bankCode in bankCodes)
                {
                    migrationBuilder.InsertData(
                        "bank_channels",
                        new[] { "channel", "bank_code" },
                        new object[] { key, bankCode });
                }
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("bank_channels", new[] { "channel", "bank_code" }, new[]
            {
                WithdrawChannel.AtsBatch.ToString(),
                "022"
            });
            migrationBuilder.DeleteData("bank_channels", new[] { "channel", "bank_code" }, new[]
            {
                WithdrawChannel.AtsBatch.ToString(),
                "024"
            });
        }
    }
}
