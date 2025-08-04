using Pi.SetMarketDataWSS.Application.Helpers;

namespace Pi.SetMarketDataWSS.Application.Tests.Helpers;

public class VenueHelperTests
{
    [Theory]
    [InlineData("CS", "Equity")]
    [InlineData("CSF", "Equity")]
    [InlineData("PS", "Equity")]
    [InlineData("PSF", "Equity")]
    [InlineData("W", "Equity")]
    [InlineData("TSR", "Equity")]
    [InlineData("DWC", "Equity")]
    [InlineData("DWP", "Equity")]
    [InlineData("DR", "Equity")]
    [InlineData("ETF", "Equity")]
    [InlineData("UT", "Equity")]
    [InlineData("UL", "Equity")]
    [InlineData("IDX", "Equity")]
    [InlineData("FC", "Derivative")]
    [InlineData("FP", "Derivative")]
    [InlineData("OEC", "Derivative")]
    [InlineData("OEP", "Derivative")]
    [InlineData("WEC", "Derivative")]
    [InlineData("WEP", "Derivative")]
    [InlineData("CMB", "Derivative")]
    [InlineData("SPT", "Derivative")]
    public void GetVenue_ShouldReturnCorrectResult(string securityType, string expected)
    {
        // Act
        var result = VenueHelper.GetVenue(securityType);

        // Assert
        Assert.Equal(expected, result);
    }
}