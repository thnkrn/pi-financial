#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class PopulateBankChannels : Migration
    {
        private readonly string[] _allBankCodes = {
            "002", "004", "006", "011", "014", "017", "018", "020", "022", "024", "025", "029", "030", "031", "032",
            "033", "034", "039", "045", "052", "066", "067", "069", "070", "071", "073", "080"
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var channels = new Dictionary<string, string[]>()
            {
                {WithdrawChannel.OnlineTransfer.ToString(), _allBankCodes},
                {WithdrawChannel.AtsBatch.ToString(), new []{"002","004","006","011","014","025","073",}},
                {DepositChannel.Odd.ToString(), new []{"002", "004", "014"}},
                {DepositChannel.SetTrade.ToString(), new []{"002", "004", "006", "014"}},
                {DepositChannel.QR.ToString(), _allBankCodes},
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
            foreach (var bankCode in _allBankCodes)
            {
                migrationBuilder.DeleteData("bank_channels", "bank_code", bankCode);
            }
        }
    }
}
