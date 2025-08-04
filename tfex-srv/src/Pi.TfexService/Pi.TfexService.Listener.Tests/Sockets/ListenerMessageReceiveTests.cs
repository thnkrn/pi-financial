using System.Globalization;
using Google.Type;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Utils;

namespace Pi.TfexService.Listener.Tests.Sockets;

public class ListenerMessageReceiveTests
{
    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Should_Deserialize_Correctly(ArraySegment<byte> raw, SetTradeOrderStatus expected)
    {
        var message = OrderDerivV3.Parser.ParseFrom(raw);

        var result = ConvertOrderDerivToSetTradeOrderStatus(message);

        Assert.Equivalent(result, expected);
    }

    public static IEnumerable<object[]> GetTestData()
    {
        return new List<object[]>
        {
            new object[]
            {
                new byte[] {18, 5, 55, 50, 51, 50, 57, 34, 8, 48, 48, 53, 52, 57, 53, 51, 48, 50, 12, 8, 149, 140, 196, 180, 6, 16, 192, 248, 220, 196, 2, 58, 7, 83, 73, 82, 73, 90, 50, 52, 64, 1, 72, 3, 82, 7, 16, 1, 24, 128, 149, 245, 42, 88, 1, 96, 1, 104, 1, 128, 1, 8, 146, 1, 2, 79, 70, 152, 1, 1, 160, 1, 1, 168, 1, 1},
                new SetTradeOrderStatus(
                    "72329",
                    "00549530",
                    "SIRIZ24",
                    SetTradeListenerOrderEnum.Side.Long,
                    SetTradeOrderUtils.GetOrderPrice(1, 90000000),
                    1,
                    1,
                    0,
                    0,
                    "OF"
                    ),
            }
        };
    }

    private static SetTradeOrderStatus ConvertOrderDerivToSetTradeOrderStatus(OrderDerivV3 message)
    {
        return new SetTradeOrderStatus(
            message.OrderNo,
            message.AccountNo,
            message.SeriesId,
            (SetTradeListenerOrderEnum.Side)message.Side,
            SetTradeOrderUtils.GetOrderPrice(message.Price.Units, message.Price.Nanos),
            message.Volume,
            message.BalanceVolume,
            message.MatchedVolume,
            message.CancelledVolume,
            message.Status
        );
    }
}