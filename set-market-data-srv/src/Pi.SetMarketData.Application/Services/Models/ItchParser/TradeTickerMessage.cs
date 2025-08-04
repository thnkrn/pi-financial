using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser
{
    public class TradeTickerParams
    {
        public Numeric32 Nanos { get; set; }
        public Numeric32 OrderbookId { get; set; }
        public Numeric64 DealId { get; set; }
        public Numeric8 DealSource { get; set; }
        public Price32 Price { get; set; }
        public Numeric64 Quantity { get; set; }
        public Timestamp DealDateTime { get; set; }
        public Numeric8 Action { get; set; }
        public Alpha Aggressor { get; set; }
        public Numeric16 TradeReportCode { get; set; }
    }

    public class TradeTickerMessage : ItchMessage
    {
        public Numeric32 Nanos { get; private set; }
        public Numeric32 OrderbookId { get; private set; }
        public Numeric64 DealId { get; private set; } // Please display as 16 char hexadecimal
        public Numeric8 DealSource { get; private set; }
        public Price32 Price { get; private set; }
        public Numeric64 Quantity { get; private set; }
        public Timestamp DealDateTime { get; private set; } // For cancelled deal, this field retains original deal execution time.
        public Numeric8 Action { get; private set; }
        public Alpha Aggressor { get; private set; }
        public Numeric16 TradeReportCode { get; private set; }

        public TradeTickerMessage(TradeTickerParams tradeTickerParams)
        {
            MsgType = 'i'; // 'i' – Trade Ticker Message, utilizing MsgType from the ITCHMessage parent class
            Nanos = tradeTickerParams.Nanos;
            OrderbookId = tradeTickerParams.OrderbookId;
            DealId = tradeTickerParams.DealId;
            DealSource = tradeTickerParams.DealSource;
            Price = tradeTickerParams.Price;
            Quantity = tradeTickerParams.Quantity;
            DealDateTime = tradeTickerParams.DealDateTime;
            Action = tradeTickerParams.Action;
            Aggressor = tradeTickerParams.Aggressor;
            TradeReportCode = tradeTickerParams.TradeReportCode;
        }

        public static TradeTickerMessage Parse(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 41) // Check for the minimum length of the message
            {
                throw new ArgumentException("Invalid data format for TradeTickerMessage.");
            }

            using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
            {
                var nanos = reader.ReadNumeric32();
                var orderbookId = reader.ReadNumeric32();
                var dealId = reader.ReadNumeric64();
                var dealSource = reader.ReadNumeric8();
                var price = reader.ReadPrice32();
                var quantity = reader.ReadNumeric64();
                var dealDateTime = reader.ReadTimestamp();
                var action = reader.ReadNumeric8();
                var aggressor = reader.ReadAlpha(1);
                var tradeReportCode = reader.ReadNumeric16();

                var tradeTickerParams = new TradeTickerParams
                {
                    Nanos = nanos,
                    OrderbookId = orderbookId,
                    DealId = dealId,
                    DealSource = dealSource,
                    Price = price,
                    Quantity = quantity,
                    DealDateTime = dealDateTime,
                    Action = action,
                    Aggressor = aggressor,
                    TradeReportCode = tradeReportCode
                };

                return new TradeTickerMessage(tradeTickerParams);
            }
        }

        public override string ToString()
        {
            return $"TradeTickerMessage:\n"
                + $"Nanos: {Nanos},\n"
                + $"OrderbookId: {OrderbookId},\n"
                + $"DealId: {DealId},\n"
                + $"DealSource: {DealSource},\n"
                + $"Price: {Price},\n"
                + $"Quantity: {Quantity},\n"
                + $"DealDateTime: {DealDateTime},\n"
                + $"Action: {Action},\n"
                + $"Aggressor: {Aggressor},\n"
                + $"TradeReportCode: {TradeReportCode}";
        }
    }
}
