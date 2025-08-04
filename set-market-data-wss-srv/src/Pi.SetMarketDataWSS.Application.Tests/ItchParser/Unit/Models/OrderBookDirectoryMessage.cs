using System.Globalization;
using Pi.SetMarketDataWSS.Application.Interfaces.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Models;
public class OrderBookDirectoryParserTests
{
    private readonly IItchParserService itchParserService = new ItchParserService();

    [Fact]
    private void Parse_OrderBookDirectoryMessage_VerifyToStringOutput()
    {
        // Arrange
        string basePath = Path.GetDirectoryName(
            System.Reflection.Assembly.GetExecutingAssembly().Location
        );
        string binFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "OrderBookDirectoryMessage",
            "R.bin"
        );
        string expectedOutputFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "OrderBookDirectoryMessage",
            "R.txt"
        );

        // Ensure the files exist to avoid confusing errors during test runs
        Assert.True(File.Exists(binFilePath), $"Mock binary file not found: {binFilePath}");
        Assert.True(
            File.Exists(expectedOutputFilePath),
            $"Expected output file not found: {expectedOutputFilePath}"
        );

        // Read the expected output
        string expectedOutput = File.ReadAllText(expectedOutputFilePath);

        // Read the binary data
        byte[] orderBookDirectoryData = File.ReadAllBytes(binFilePath);

        // Act
        var orderBookDirectoryMessage =
            itchParserService.Parse(orderBookDirectoryData) as OrderBookDirectoryMessage;

        // Ensure parsing succeeded and resulted in the correct type
        Assert.NotNull(orderBookDirectoryMessage);

        // Preparing the expected output for comparison by normalizing space for easier comparison
        var expectedParts = expectedOutput.Split('\n');

        // Assert - Direct comparison of the normalized strings
        Assert.Equal(uint.Parse(expectedParts[1]), orderBookDirectoryMessage.Nanos.Value);
        Assert.Equal(uint.Parse(expectedParts[2]), orderBookDirectoryMessage.OrderBookID.Value);
        Assert.Equal(expectedParts[3], orderBookDirectoryMessage.Symbol.Value);
        Assert.Equal(expectedParts[4], orderBookDirectoryMessage.LongName.Value);
        Assert.Equal(expectedParts[5], orderBookDirectoryMessage.ISIN.Value);
        Assert.Equal(expectedParts[6], orderBookDirectoryMessage.FinancialProduct.Value);
        Assert.Equal(expectedParts[7], orderBookDirectoryMessage.TradingCurrency.Value);
        Assert.Equal(
            ushort.Parse(expectedParts[8]),
            orderBookDirectoryMessage.DecimalsInPrice.Value
        );
        Assert.Equal(
            ushort.Parse(expectedParts[9]),
            orderBookDirectoryMessage.DecimalsInNominalValue.Value
        );
        Assert.Equal(uint.Parse(expectedParts[10]), orderBookDirectoryMessage.RoundLotSize.Value);
        Assert.Equal(ulong.Parse(expectedParts[11]), orderBookDirectoryMessage.NominalValue.Value);
        Assert.Equal(byte.Parse(expectedParts[12]), orderBookDirectoryMessage.NumberOfLegs.Value);
        Assert.Equal(expectedParts[13], orderBookDirectoryMessage.UnderlyingName.Value);
        Assert.Equal(uint.Parse(expectedParts[14]), orderBookDirectoryMessage.Underlying.Value);
        Assert.Equal(
            uint.Parse(expectedParts[15]),
            orderBookDirectoryMessage.UnderlyingOrderBookID.Value
        );
        Assert.Equal(int.Parse(expectedParts[16]), orderBookDirectoryMessage.StrikePrice.Value);
        Assert.Equal(
            DateTime.ParseExact(expectedParts[17], "yyyyMMdd", null),
            orderBookDirectoryMessage.ExpirationDate
        );
        Assert.Equal(
            ushort.Parse(expectedParts[18]),
            orderBookDirectoryMessage.DecimalsInStrikePrice.Value
        );
        Assert.Equal(byte.Parse(expectedParts[19]), orderBookDirectoryMessage.OptionType.Value);
        Assert.Equal(byte.Parse(expectedParts[20]), orderBookDirectoryMessage.ExchangeCode.Value);
        Assert.Equal(byte.Parse(expectedParts[21]), orderBookDirectoryMessage.MarketCode.Value);
        Assert.Equal(
            int.Parse(expectedParts[22]),
            orderBookDirectoryMessage.PriceQuotationFactor.Value
        );
        Assert.Equal(expectedParts[23], orderBookDirectoryMessage.CorporateActionCode.Value);
        Assert.Equal(expectedParts[24], orderBookDirectoryMessage.NotificationSign.Value);
        Assert.Equal(expectedParts[25], orderBookDirectoryMessage.OtherSign.Value);
        Assert.Equal(expectedParts[26], orderBookDirectoryMessage.AllowNvdr.Value);
        Assert.Equal(expectedParts[27], orderBookDirectoryMessage.AllowShortSell.Value);
        Assert.Equal(expectedParts[28], orderBookDirectoryMessage.AllowShortSellOnNvdr.Value);
        Assert.Equal(expectedParts[29], orderBookDirectoryMessage.AllowTtf.Value);
        Assert.Equal(ulong.Parse(expectedParts[30]), orderBookDirectoryMessage.ParValue.Value);
        // Date format assumed as YYYYMMDD
        Assert.Equal(
            expectedParts[31] == "0"
                ? DateTime.MinValue
                : DateTime.ParseExact(expectedParts[31], "yyyyMMdd", null),
            orderBookDirectoryMessage.FirstTradingDate
        );
        var expectedFirstTradingTime =
            expectedParts[32] == "0"
                ? TimeSpan.Zero
                : TimeSpan.ParseExact(expectedParts[32], "hhmmss", CultureInfo.InvariantCulture);
        var actualFirstTradingTime = orderBookDirectoryMessage.FirstTradingTime;

        Assert.Equal(expectedFirstTradingTime.Hours, actualFirstTradingTime.Hours);
        Assert.Equal(expectedFirstTradingTime.Minutes, actualFirstTradingTime.Minutes);
        Assert.Equal(expectedFirstTradingTime.Seconds, actualFirstTradingTime.Seconds);

        Assert.Equal(
            expectedParts[33] == "0"
                ? DateTime.MinValue
                : DateTime.ParseExact(expectedParts[33], "yyyyMMdd", null),
            orderBookDirectoryMessage.LastTradingDate
        );

        var expectedLastTradingTime =
            expectedParts[34] == "0"
                ? TimeSpan.Zero
                : TimeSpan.ParseExact(expectedParts[34], "hhmmss", CultureInfo.InvariantCulture);
        var actualLastTradingTime = orderBookDirectoryMessage.LastTradingTime;

        Assert.Equal(expectedLastTradingTime.Hours, actualLastTradingTime.Hours);
        Assert.Equal(expectedLastTradingTime.Minutes, actualLastTradingTime.Minutes);
        Assert.Equal(expectedLastTradingTime.Seconds, actualLastTradingTime.Seconds);

        Assert.Equal(expectedParts[35], orderBookDirectoryMessage.MarketSegment.Value);
        Assert.Equal(expectedParts[36], orderBookDirectoryMessage.PhysicalDelivery.Value);
        Assert.Equal(uint.Parse(expectedParts[37]), orderBookDirectoryMessage.ContractSize.Value);
        Assert.Equal(expectedParts[38], orderBookDirectoryMessage.SectorCode.Value);
        Assert.Equal(expectedParts[39], orderBookDirectoryMessage.OriginatesFrom.Value);
        Assert.Equal(expectedParts[40], orderBookDirectoryMessage.Status.Value);
        Assert.Equal(ushort.Parse(expectedParts[41]), orderBookDirectoryMessage.Modifier.Value);
        Assert.Equal(
            DateTime.ParseExact(expectedParts[42], "yyyyMMdd", null),
            orderBookDirectoryMessage.NotationDate
        );
        Assert.Equal(
            ushort.Parse(expectedParts[43]),
            orderBookDirectoryMessage.DecimalsInContractSizePQF
        );
    }
}
