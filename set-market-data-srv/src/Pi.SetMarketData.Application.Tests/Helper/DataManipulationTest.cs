using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Tests.Helper;

public class DataManipulationTest
{
    [Theory]
    [InlineData(0.000027, "0.000027")]
    [InlineData(-0.000832, "-0.000832")]
    [InlineData(0.442896, "0.442896")]
    [InlineData(0.002, "0.002")]
    [InlineData(-0.076, "-0.076")]
    [InlineData(0.000000005, "0.000000005")]
    public void ToListString_ShouldReturnOnlyDecimalPlace(double number, string expectedResult)
    {
        var result = DataManipulation.ToListString([number]);

        Assert.Equal(expectedResult, result[0]);
    }
}