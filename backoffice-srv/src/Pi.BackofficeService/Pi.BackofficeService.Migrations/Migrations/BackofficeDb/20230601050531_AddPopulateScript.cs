#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb;

/// <inheritdoc />
public partial class AddPopulateScript : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            "banks",
            new[] { "code", "name", "abbreviation" },
            new object[,]
            {
                { "002", "Bangkok Bank Public Company Limited", "BBL" },
                { "004", "Kasikornbank Public Company Limited", "KBNK" },
                { "006", "Krung Thai Bank Public Company Limited", "KTB" },
                { "011", "TMBThanachart Bank Public Company Limited", "TTB" },
                { "014", "Siam Commercial Bank Public Company Limited", "SCB" },
                { "017", "Citibank, N.A", "CITI" },
                { "018", "Sumitomo Mitsui Banking Corporation", "SMBC" },
                { "020", "Standard Chartered Bank (THAI) Public Company Limited", "SCBT" },
                { "022", "CIMB Thai Bank Public Company Limited", "CIMBT" },
                { "024", "United Overseas Bank (THAI) Public Company Limited", "UOBT" },
                { "025", "Bank of Ayudhya Public Company Limited", "BAY" },
                { "029", "Indian Overseas Bank", "IOBA" },
                { "030", "Government Saving Bank", "GSB" },
                { "031", "Hong Kong & Shanghai Corporation Limited", "HSBC" },
                { "032", "Deutsche Bank Aktiengesellschaft", "DBBK" },
                { "033", "Government Housing Bank", "GHBA" },
                { "034", "Bank of Agriculture and Agricultural Cooperatives", "BAAC" },
                { "039", "Mizuho Bank Bangkok Branch", "MHBC" },
                { "045", "BNP Paribas, Bangkok Branch", "BNPP" },
                { "052", "Bank of China Limited", "BOCB" },
                { "066", "Islamic Bank of Thailand", "ISBT" },
                { "067", "Tisco Bank Public Company Limited", "TSCO" },
                { "069", "Kiatnakin Bank Public Company Limited", "KKP" },
                { "070", "Industrial and Commercial Bank of China (THAI) Public Company Limited", "ICBC" },
                { "071", "Thai Credit Retail Bank Public Company Limited", "TCRB" },
                { "073", "Land and Houses Bank Public Company Limited", "LH BA" },
                { "080", "Sumitomo Mitsui Trust Bank (THAI) PCL", "SMTB" }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            "banks",
            "code",
            new object[]
            {
                "002", "004", "006", "011", "014", "017", "018", "020", "022", "024", "025", "029", "030", "031", "032",
                "033", "034", "039", "045", "052", "066", "067", "069", "070", "071", "073", "080"
            });
    }
}
