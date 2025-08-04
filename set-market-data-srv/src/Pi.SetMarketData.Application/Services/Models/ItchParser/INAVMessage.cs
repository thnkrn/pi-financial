using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser
{
    public class INAVParams
    {
        public Numeric32 Nanos { get; set; } // Nanoseconds portion of the timestamp.
        public Numeric32 OrderBookId { get; set; } // Orderbook ID.
        public Price32 INAV { get; set; } // Indicative NAV.
        public Price32 Change { get; set; } // iNAV change from the previous day.
        public Price32 PercentageChange { get; set; } // Percentage of iNAV change from the previous day (2 decimal digits).
        public Timestamp Timestamp { get; set; } // Time of INAV data.
    }

    public class INAVMessage : ItchMessage
    {
        public Numeric32 Nanos { get; private set; }
        public Numeric32 OrderBookId { get; private set; }
        public Price32 INAV { get; private set; }
        public Price32 Change { get; private set; }
        public Price32 PercentageChange { get; private set; }
        public Timestamp Timestamp { get; private set; }

        public INAVMessage(INAVParams inavParams)
        {
            MsgType = ItchMessageType.f; // 'f' – iNAV Message.
            Nanos = inavParams.Nanos;
            OrderBookId = inavParams.OrderBookId;
            INAV = inavParams.INAV;
            Change = inavParams.Change;
            PercentageChange = inavParams.PercentageChange;
            Timestamp = inavParams.Timestamp;
        }

        public static INAVMessage Parse(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 28) // Ensure byte array is of expected minimum length for iNAV Message.
            {
                throw new ArgumentException("Invalid data format for INAVMessage.");
            }

            using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
            {
                var nanos = reader.ReadNumeric32();
                var OrderBookId = reader.ReadNumeric32();
                var inav = reader.ReadPrice32();
                var change = reader.ReadPrice32();
                var percentageChange = reader.ReadPrice32();
                var timestamp = reader.ReadTimestamp();

                var inavParams = new INAVParams
                {
                    Nanos = nanos,
                    OrderBookId = OrderBookId,
                    INAV = inav,
                    Change = change,
                    PercentageChange = percentageChange,
                    Timestamp = timestamp
                };

                return new INAVMessage(inavParams);
            }
        }

        public override string ToString()
        {
            return $"INAVMessage:\n"
                + $"MsgType: {MsgType},\n"
                + $"Nanos: {Nanos},\n"
                + $"OrderBookId: {OrderBookId},\n"
                + $"INAV: {INAV},\n"
                + $"Change: {Change},\n"
                + $"PercentageChange: {PercentageChange},\n"
                + $"Timestamp: {Timestamp}";
        }
    }
}
