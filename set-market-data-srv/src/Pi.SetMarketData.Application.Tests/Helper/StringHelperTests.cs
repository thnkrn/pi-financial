using Pi.SetMarketData.Application.Helper;

namespace Pi.SetMarketData.Application.Tests.Helper;

public class StringHelperTests
{
    private readonly Dictionary<string, string> _brokerMap;
    public StringHelperTests()
    {
        _brokerMap = new Dictionary<string, string>
        {
            { "01", "BLS" },
            { "06", "KKPS" },
            { "08", "ASPS" },
            { "11", "KS" },
            { "13", "KGI" },
            { "16", "TNS" },
            { "19", "YUANTA" },
            { "23", "SCBS" },
            { "24", "FSS" },
            { "28", "MACQ" },
            { "41", "JPM" },
            { "42", "MST" },
            { "80", "KTC" }
        };
    }

    [Theory]
    [InlineData("SET5013C2412H", "Call", "SET50 Dec 24 DW Call Series H by KGI")]
    [InlineData("SET5001P2403A", "Put", "SET50 Mar 24 DW Put Series A by BLS")]
    [InlineData("SET5041C2403A", "Call", "SET50 Mar 24 DW Call Series A by JPM")]
    [InlineData("THANI13C2407A", "Call", "THANI Jul 24 DW Call Series A by KGI")]
    [InlineData("JMART01C2403A", "Call", "JMART Mar 24 DW Call Series A by BLS")]
    [InlineData("FORTH01C2403A", "Call", "FORTH Mar 24 DW Call Series A by BLS")]
    [InlineData("PTTEP01P2405X", "Put", "PTTEP May 24 DW Put Series X by BLS")]
    [InlineData("FORTH19C2404A", "Call", "FORTH Apr 24 DW Call Series A by YUANTA")]
    public void DwFormat_ShouldReturnCorrectFormat(string symbol, string expectedDirection, string expectedName)
    {
        // Act
        var (name, direction) = StringHelper.DwFormat(symbol, _brokerMap);

        // Assert
        Assert.Equal(expectedName, name);
        Assert.Equal(expectedDirection, direction);
    }

    [Theory]
    [InlineData("JMART41P2405A", "JPM / A")]
    [InlineData("BLA13C2404A", "KGI / A")]
    [InlineData("COM701P2404X", "BLS / X")]
    [InlineData("COM706C2403A", "KKPS / A")]
    [InlineData("BAM19C2405A", "YUANTA / A")]
    [InlineData("SET5028C2412B", "MACQ / B")]
    public void GetIssuerSeries_ShouldReturnCorrectResult(string symbol, string expected)
    {
        // Act
        var result = StringHelper.GetIssuerSeries(symbol, _brokerMap);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("NER-W2", "NER Warrant Series W2")]
    [InlineData("MINT-W7", "MINT Warrant Series W7")]
    [InlineData("NOBLE-W2", "NOBLE Warrant Series W2")]
    [InlineData("BEYOND-W2", "BEYOND Warrant Series W2")]
    [InlineData("SAMART-W3", "SAMART Warrant Series W3")]
    [InlineData("XPG-W4", "XPG Warrant Series W4")]
    public void WarrantsFormat_ShouldReturnCorrectResult(string symbol, string expected)
    {
        // Act
        var result = StringHelper.WarrantsFormat(symbol);

        // Assert
        Assert.Equal(expected, result);
    }
}