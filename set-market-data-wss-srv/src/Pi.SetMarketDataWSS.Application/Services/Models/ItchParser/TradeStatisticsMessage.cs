using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Application.Utils;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

public class TradeStatisticsParams
{
    public Numeric32 Nanos { get; set; } // Nanoseconds portion of the timestamp.
    public Numeric32 OrderBookId { get; set; } // Orderbook ID.
    public Price32 OpenPrice { get; set; } // Open price of current day.
    public Price32 HighPrice { get; set; } // Highest price of current day.
    public Price32 LowPrice { get; set; } // Lowest price of current day.
    public Price32 LastPrice { get; set; } // Last auto-match trade price.
    public Price32 LastAuctionPrice { get; set; } // Last auction price.
    public Numeric64 TurnoverQuantity { get; set; } // Sum of Traded Quantity.
    public Numeric64 ReportedTurnoverQuantity { get; set; } // Sum of Traded Quantity of trade report only.
    public Price64 TurnoverValue { get; set; } // Sum of Traded Price * Traded Quantity.
    public Price64 ReportedTurnoverValue { get; set; } // Sum of Traded Price * Traded Quantity of trade report only.
    public Price32 AveragePrice { get; set; } // Average price of current day.
    public Numeric64 TotalNumberOfTrades { get; set; } // The total number of trades today.
}

public class TradeStatisticsMessage : ItchMessage
{
    public TradeStatisticsMessage(TradeStatisticsParams tradeStatisticsParams)
    {
        MsgType = 'I'; // 'I' – Trade Statistics Message
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

    public Numeric32 Nanos { get; }
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
        if (bytes == null || bytes.Length < 72) // Check for the minimum length of the message
            throw new ArgumentException("Invalid data format for TradeStatisticsMessage.");

        using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
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
    }

    public string ToStringUnitTest()
    {
        return $"TradeStatisticsMessage:\n"
               + $"Nanos: {Nanos},\n"
               + $"OrderBookId: {OrderBookId},\n"
               + $"OpenPrice: {OpenPrice},\n"
               + $"HighPrice: {HighPrice},\n"
               + $"LowPrice: {LowPrice},\n"
               + $"LastPrice: {LastPrice},\n"
               + $"LastAuctionPrice: {LastAuctionPrice},\n"
               + $"TurnoverQuantity: {TurnoverQuantity},\n"
               + $"ReportedTurnoverQuantity: {ReportedTurnoverQuantity},\n"
               + $"TurnoverValue: {TurnoverValue},\n"
               + $"ReportedTurnoverValue: {ReportedTurnoverValue},\n"
               + $"AveragePrice: {AveragePrice},\n"
               + $"TotalNumberOfTrades: {TotalNumberOfTrades}";
    }
}