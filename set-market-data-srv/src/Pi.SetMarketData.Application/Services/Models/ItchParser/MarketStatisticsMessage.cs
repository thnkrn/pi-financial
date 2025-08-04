using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser
{
    public class MarketStatisticParams
    {
        // Nanoseconds portion of the timestamp.
        public Numeric32 Nanos { get; set; }

        // Unique ID for market statistics record (e.g., SET, MAI).
        public Alpha MarketStatisticsID { get; set; }

        // Trading currency (e.g., THB, USD).
        public Alpha Currency { get; set; }

        // Time of Market statistics.
        public Timestamp MarketStatisticsTime { get; set; }

        // Total number of trades across the corresponding market segment and currency.
        public Numeric32 TotalTrades { get; set; }

        // Total quantity traded across the corresponding market segment and currency.
        public Numeric64 TotalQuantity { get; set; }

        // Price * quantity for all trades in the corresponding market segment and currency (no decimal place).
        public Numeric64 TotalValue { get; set; }

        // Total volume of securities traded that current price change is positive compared to previous closing price.
        public Numeric64 UpQuantity { get; set; }

        // Total volume of securities traded that current price change is negative compared to previous closing price.
        public Numeric64 DownQuantity { get; set; }

        // Total volume of securities traded that current price change is zero compared to previous closing price.
        public Numeric64 NoChangeVolume { get; set; }

        // Number of securities that have a trade and current price change is positive compared to previous closing price.
        public Numeric32 UpShares { get; set; }

        // Number of securities that have a trade and current price change is negative compared to previous closing price.
        public Numeric32 DownShares { get; set; }

        // Number of securities that have a trade and current price change is zero compared to previous closing price.
        public Numeric32 NoChangeShares { get; set; }
    }

    public class MarketStatisticMessage : ItchMessage
    {
        public Numeric32 Nanos { get; private set; }
        public Alpha MarketStatisticsID { get; private set; }
        public Alpha Currency { get; private set; }
        public Timestamp MarketStatisticsTime { get; private set; }
        public Numeric32 TotalTrades { get; private set; }
        public Numeric64 TotalQuantity { get; private set; }
        public Numeric64 TotalValue { get; private set; }
        public Numeric64 UpQuantity { get; private set; }
        public Numeric64 DownQuantity { get; private set; }
        public Numeric64 NoChangeVolume { get; private set; }
        public Numeric32 UpShares { get; private set; }
        public Numeric32 DownShares { get; private set; }
        public Numeric32 NoChangeShares { get; private set; }

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

        public static MarketStatisticMessage Parse(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 83) // Ensure byte array is of expected length
            {
                throw new ArgumentException("Invalid data format for MarketStatisticMessage.");
            }

            using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
            {
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
        }

        public override string ToString()
        {
            return $"MarketStatisticMessage:\n"
                + $"MsgType: {MsgType}, Nanos: {Nanos}, MarketStatisticsID: {MarketStatisticsID}, Currency: {Currency}, "
                + $"MarketStatisticsTime: {MarketStatisticsTime}, TotalTrades: {TotalTrades}, TotalQuantity: {TotalQuantity}, "
                + $"TotalValue: {TotalValue}, UpQuantity: {UpQuantity}, DownQuantity: {DownQuantity}, "
                + $"NoChangeVolume: {NoChangeVolume}, UpShares: {UpShares}, DownShares: {DownShares}, "
                + $"NoChangeShares: {NoChangeShares}";
        }
    }
}
