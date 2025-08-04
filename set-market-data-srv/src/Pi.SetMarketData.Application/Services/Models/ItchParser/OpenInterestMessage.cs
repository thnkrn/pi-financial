using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser
{
    public class OpenInterestParams
    {
        // Nanoseconds portion of the timestamp.
        public Numeric32 Nanos { get; set; }

        // Orderbook ID.
        public Numeric32 OrderbookId { get; set; }

        // Total number of outstanding contracts.
        public Numeric64 OpenInterest { get; set; }

        // Timestamp of the message.
        public Timestamp Timestamp { get; set; }
    }

    public class OpenInterestMessage : ItchMessage
    {
        public Numeric32 Nanos { get; private set; }
        public Numeric32 OrderbookId { get; private set; }
        public Numeric64 OpenInterest { get; private set; }
        public Timestamp Timestamp { get; private set; }

        public OpenInterestMessage(OpenInterestParams openInterestParams)
        {
            MsgType = 'h'; // 'h' – Open Interest Message
            Nanos = openInterestParams.Nanos;
            OrderbookId = openInterestParams.OrderbookId;
            OpenInterest = openInterestParams.OpenInterest;
            Timestamp = openInterestParams.Timestamp;
        }

        public static OpenInterestMessage Parse(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 24) // Ensure byte array is of expected length
            {
                throw new ArgumentException("Invalid data format for OpenInterestMessage.");
            }

            using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
            {
                return new OpenInterestMessage(
                    new OpenInterestParams
                    {
                        Nanos = reader.ReadNumeric32(),
                        OrderbookId = reader.ReadNumeric32(),
                        OpenInterest = reader.ReadNumeric64(),
                        Timestamp = reader.ReadTimestamp(),
                    }
                );
            }
        }

        public override string ToString()
        {
            return $"OpenInterestMessage:\n"
                + $"MsgType: {MsgType}, Nanos: {Nanos}, OrderbookId: {OrderbookId}, OpenInterest: {OpenInterest}, "
                + $"Timestamp: {Timestamp}";
        }
    }
}
