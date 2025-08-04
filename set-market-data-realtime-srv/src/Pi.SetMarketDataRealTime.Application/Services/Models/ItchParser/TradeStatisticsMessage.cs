using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class TradeStatisticsParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 OrderBookId { get; init; }
    public required Price32 OpenPrice { get; init; }
    public required Price32 HighPrice { get; init; }
    public required Price32 LowPrice { get; init; }
    public required Price32 LastPrice { get; init; }
    public required Price32 LastAuctionPrice { get; init; }
    public required Numeric64 TurnoverQuantity { get; init; }
    public required Numeric64 ReportedTurnoverQuantity { get; init; }
    public required Price64 TurnoverValue { get; init; }
    public required Price64 ReportedTurnoverValue { get; init; }
    public required Price32 AveragePrice { get; init; }
    public required Numeric64 TotalNumberOfTrades { get; init; }
}

public class TradeStatisticsMessage : ItchMessage
{
    private const int MinimumByteLength = 72;
    private const char MessageTypeChar = 'I';

    public TradeStatisticsMessage(TradeStatisticsParams tradeStatisticsParams)
    {
        MsgType = MessageTypeChar;
        Nanos = tradeStatisticsParams.Nanos;
        OrderBookId = tradeStatisticsParams.OrderBookId;
        OpenPrice = tradeStatisticsParams.OpenPrice;
        HighPrice = tradeStatisticsParams.HighPrice;
        LowPrice = tradeStatisticsParams.LowPrice;
        LastPrice = tradeStatisticsParams.LastPrice;
        LastAuctionPrice = tradeStatisticsParams.LastAuctionPrice;
        TurnoverQuantity = tradeStatisticsParams.TurnoverQuantity;
        ReportedTurnoverQuantity = tradeStatisticsParams.ReportedTurnoverQuantity;
        TurnoverValue = tradeStatisticsParams.TurnoverValue;
        ReportedTurnoverValue = tradeStatisticsParams.ReportedTurnoverValue;
        AveragePrice = tradeStatisticsParams.AveragePrice;
        TotalNumberOfTrades = tradeStatisticsParams.TotalNumberOfTrades;
    }

    public override Numeric32 Nanos { get; }
    public Numeric32 OrderBookId { get; }
    public Price32 OpenPrice { get; }
    public Price32 HighPrice { get; }
    public Price32 LowPrice { get; }
    public Price32 LastPrice { get; }
    public Price32 LastAuctionPrice { get; }
    public Numeric64 TurnoverQuantity { get; }
    public Numeric64 ReportedTurnoverQuantity { get; }
    public Price64 TurnoverValue { get; }
    public Price64 ReportedTurnoverValue { get; }
    public Price32 AveragePrice { get; }
    public Numeric64 TotalNumberOfTrades { get; }

    public static TradeStatisticsMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < MinimumByteLength)
            throw new ArgumentException(
                $"Invalid data format for TradeStatisticsMessage. Expected at least {MinimumByteLength} bytes.",
                nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        try
        {
            var tradeStatisticsParams = new TradeStatisticsParams
            {
                Nanos = reader.ReadNumeric32(),
                OrderBookId = reader.ReadNumeric32(),
                OpenPrice = reader.ReadPrice32(),
                HighPrice = reader.ReadPrice32(),
                LowPrice = reader.ReadPrice32(),
                LastPrice = reader.ReadPrice32(),
                LastAuctionPrice = reader.ReadPrice32(),
                TurnoverQuantity = reader.ReadNumeric64(),
                ReportedTurnoverQuantity = reader.ReadNumeric64(),
                TurnoverValue = reader.ReadPrice64(),
                ReportedTurnoverValue = reader.ReadPrice64(),
                AveragePrice = reader.ReadPrice32(),
                TotalNumberOfTrades = reader.ReadNumeric64()
            };

            return new TradeStatisticsMessage(tradeStatisticsParams);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Error parsing TradeStatisticsMessage", nameof(bytes), ex);
        }
    }

    public string ToStringUnitTest()
    {
        return $"""
                TradeStatisticsMessage:
                Nanos: {Nanos},
                OrderBookId: {OrderBookId},
                OpenPrice: {OpenPrice},
                HighPrice: {HighPrice},
                LowPrice: {LowPrice},
                LastPrice: {LastPrice},
                LastAuctionPrice: {LastAuctionPrice},
                TurnoverQuantity: {TurnoverQuantity},
                ReportedTurnoverQuantity: {ReportedTurnoverQuantity},
                TurnoverValue: {TurnoverValue},
                ReportedTurnoverValue: {ReportedTurnoverValue},
                AveragePrice: {AveragePrice},
                TotalNumberOfTrades: {TotalNumberOfTrades}
                """;
    }
}