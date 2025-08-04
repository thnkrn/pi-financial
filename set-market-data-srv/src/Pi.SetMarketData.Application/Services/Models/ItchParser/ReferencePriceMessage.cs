using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser
{
    public class ReferencePriceParams
    {
        // Nanoseconds portion of the timestamp.
        public Numeric32 Nanos { get; set; }

        // Orderbook ID.
        public Numeric32 OrderbookId { get; set; }

        // Type of price published.
        public Numeric8 PriceType { get; set; }

        // Price based on Price Type.
        public Price32 Price { get; set; }

        // Timestamp when the Price was updated.
        public Timestamp UpdatedTimestamp { get; set; }
    }

    public class ReferencePriceMessage : ItchMessage
    {
        public Numeric32 Nanos { get; private set; }
        public Numeric32 OrderbookId { get; private set; }
        public Numeric8 PriceType { get; private set; }
        public Price32 Price { get; private set; }
        public Timestamp UpdatedTimestamp { get; private set; }

        public ReferencePriceMessage(ReferencePriceParams referencePriceParams)
        {
            MsgType = 'Q'; // 'Q' – Reference Price Message
            Nanos = referencePriceParams.Nanos;
            OrderbookId = referencePriceParams.OrderbookId;
            PriceType = referencePriceParams.PriceType;
            Price = referencePriceParams.Price;
            UpdatedTimestamp = referencePriceParams.UpdatedTimestamp;
        }

        public static ReferencePriceMessage Parse(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 21) // Ensure byte array is of expected length
            {
                throw new ArgumentException("Invalid data format for ReferencePriceMessage.");
            }

            using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
            {
                return new ReferencePriceMessage(
                    new ReferencePriceParams
                    {
                        Nanos = reader.ReadNumeric32(),
                        OrderbookId = reader.ReadNumeric32(),
                        PriceType = reader.ReadNumeric8(),
                        Price = reader.ReadPrice32(),
                        UpdatedTimestamp = reader.ReadTimestamp()
                    }
                );
            }
        }

        public override string ToString()
        {
            return $"ReferencePriceMessage:\n"
                + $"MsgType: {MsgType}, Nanos: {Nanos}, OrderbookId: {OrderbookId}, PriceType: {PriceType}, "
                + $"Price: {Price}, UpdatedTimestamp: {UpdatedTimestamp}";
        }
    }
}
