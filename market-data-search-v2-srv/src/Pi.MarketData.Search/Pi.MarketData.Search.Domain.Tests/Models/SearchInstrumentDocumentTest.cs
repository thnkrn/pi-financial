using Pi.MarketData.Search.Domain.Models;

namespace Pi.MarketData.Search.Domain.Tests.Models;

public class SearchInstrumentDocumentTest
{
    [Theory]
    [InlineData("JD.com Inc", "JD com Inc")]
    [InlineData("Schwab Fundamental US Large Co. Index ETF", "")]
    [InlineData("ETRACS Monthly Pay 1.5x Leveraged Mortgage REIT ETN", "")]
    [InlineData("T. Rowe Price Value ETF`", "")]
    [InlineData("Avantis U.S Small Cap Equity ETF", "")]
    [InlineData("Ronshine Service Holding Co Ltd.", "")]
    [InlineData("International Genius Co.", "")]
    [InlineData("iShares U.S. Utilities ETF", "")]
    public void Should_SetExpectedCustomIndex_When_SetCustomIndex(string friendlyName, string expected)
    {
        // Arrange
        var search = new SearchInstrumentDocument()
        {
            FriendlyName = friendlyName
        };

        // Act
        search.SetCustomIndex();

        // Assert
        Assert.Equal(expected, search.CustomIndex);
    }
}
