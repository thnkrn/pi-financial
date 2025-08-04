using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Utils;

namespace Pi.TfexService.Application.Tests.Utils;

public class SetTradeOrderUtilsTests
{
    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Should_Convert_Price_Correctly(long units, int nanos, double expected)
    {
        var price = SetTradeOrderUtils.GetOrderPrice(units, nanos);

        Assert.Equal(price, expected);
    }

    public static IEnumerable<object[]> GetTestData()
    {
        return new List<object[]>
        {
            new object[]
            {
                1,
                90_000_000,
                1.09,
            },
            new object[]
            {
                1,
                900_000_000,
                1.90,
            },
            new object[]
            {
                -1,
                -90_000_000,
                -1.09,
            },
            new object[]
            {
                -1,
                -900_000_000,
                -1.90,
            }
        };
    }
}