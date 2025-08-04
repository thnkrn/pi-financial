using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class MarketStatisticParams
{
    public required Numeric32 Nanos { get; init; }
    public required Alpha MarketStatisticsID { get; init; }
    public required Alpha Currency { get; init; }
    public required Timestamp MarketStatisticsTime { get; init; }
    public required Numeric32 TotalTrades { get; init; }
    public required Numeric64 TotalQuantity { get; init; }
    public required Numeric64 TotalValue { get; init; }
    public required Numeric64 UpQuantity { get; init; }
    public required Numeric64 DownQuantity { get; init; }
    public required Numeric64 NoChangeVolume { get; init; }
    public required Numeric32 UpShares { get; init; }
    public required Numeric32 DownShares { get; init; }
    public required Numeric32 NoChangeShares { get; init; }
}

public class MarketStatisticMessage : ItchMessage
{
    public MarketStatisticMessage(MarketStatisticParams marketStatisticParams)
    {
        MsgType = 'g'; // 'g' – Market Statistics Message
        Nanos = marketStatisticParams.Nanos;
        MarketStatisticsID = marketStatisticParams.MarketStatisticsID;
        Currency = marketStatisticParams.Currency;
        MarketStatisticsTime = marketStatisticParams.MarketStatisticsTime;
        TotalTrades = marketStatisticParams.TotalTrades;
        TotalQuantity = marketStatisticParams.TotalQuantity;
        TotalValue = marketStatisticParams.TotalValue;
        UpQuantity = marketStatisticParams.UpQuantity;
        DownQuantity = marketStatisticParams.DownQuantity;
        NoChangeVolume = marketStatisticParams.NoChangeVolume;
        UpShares = marketStatisticParams.UpShares;
        DownShares = marketStatisticParams.DownShares;
        NoChangeShares = marketStatisticParams.NoChangeShares;
    }

    public override Numeric32 Nanos { get; }
    public Alpha MarketStatisticsID { get; }
    public Alpha Currency { get; }
    public Timestamp MarketStatisticsTime { get; }
    public Numeric32 TotalTrades { get; }
    public Numeric64 TotalQuantity { get; }
    public Numeric64 TotalValue { get; }
    public Numeric64 UpQuantity { get; }
    public Numeric64 DownQuantity { get; }
    public Numeric64 NoChangeVolume { get; }
    public Numeric32 UpShares { get; }
    public Numeric32 DownShares { get; }
    public Numeric32 NoChangeShares { get; }

    public static MarketStatisticMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < 83)
            throw new ArgumentException(
                "Invalid data format for MarketStatisticMessage. Expected at least 83 bytes.", nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        return new MarketStatisticMessage(
            new MarketStatisticParams
            {
                Nanos = reader.ReadNumeric32(),
                MarketStatisticsID = reader.ReadAlpha(12),
                Currency = reader.ReadAlpha(3),
                MarketStatisticsTime = reader.ReadTimestamp(),
                TotalTrades = reader.ReadNumeric32(),
                TotalQuantity = reader.ReadNumeric64(),
                TotalValue = reader.ReadNumeric64(),
                UpQuantity = reader.ReadNumeric64(),
                DownQuantity = reader.ReadNumeric64(),
                NoChangeVolume = reader.ReadNumeric64(),
                UpShares = reader.ReadNumeric32(),
                DownShares = reader.ReadNumeric32(),
                NoChangeShares = reader.ReadNumeric32()
            }
        );
    }

    public string ToStringUnitTest()
    {
        return $"""
                MarketStatisticMessage:
                MsgType: {MsgType}, Nanos: {Nanos}, MarketStatisticsID: {MarketStatisticsID}, Currency: {Currency},
                MarketStatisticsTime: {MarketStatisticsTime}, TotalTrades: {TotalTrades}, TotalQuantity: {TotalQuantity},
                TotalValue: {TotalValue}, UpQuantity: {UpQuantity}, DownQuantity: {DownQuantity},
                NoChangeVolume: {NoChangeVolume}, UpShares: {UpShares}, DownShares: {DownShares},
                NoChangeShares: {NoChangeShares}
                """;
    }
}