namespace Pi.SetMarketData.IndicatorWorker.Tests
{
    using Pi.SetMarketData.Domain.Models.Indicator;
    using Newtonsoft.Json;
    using Xunit;

    public class TestParseCandleMessage
    {
        [Fact]
        public void parse_candle_message()
        {
            var message = "{\"before\":null,\"after\":{\"bucket\":\"2018-05-03T05:07:00.000000Z\",\"symbol\":\"KBANK\",\"venue\":\"Equity\",\"open\":192.5,\"high\":\"193.0\",\"low\":192.5,\"close\":\"193.0\",\"volume\":{\"scale\":0,\"value\":\"AMg=\"},\"amount\":\"38600.0\"},\"source\":{\"version\":\"2.7.2.Final\",\"connector\":\"postgresql\",\"name\":\"dbserver1\",\"ts_ms\":1737541131908,\"snapshot\":\"false\",\"db\":\"postgres\",\"sequence\":[\"85427408248\",\"82628990552\"],\"ts_us\":1737541131908456,\"ts_ns\":1737541131908456000,\"schema\":\"public\",\"table\":\"candle_1_min\",\"txId\":57530085,\"lsn\":82628990552,\"xmin\":null},\"transaction\":null,\"op\":\"c\",\"ts_ms\":1737595843797,\"ts_us\":1737595843797864,\"ts_ns\":\"1737595843797864313\"}";
            var candleEventMessage = JsonConvert.DeserializeObject<CandleEventMessage>(message);
            Console.WriteLine(candleEventMessage);
            Assert.NotNull(candleEventMessage);
            Assert.Equal("KBANK", candleEventMessage.After?.Symbol);
            Assert.Equal("Equity", candleEventMessage.After?.Venue);
            Assert.Equal(new DateTimeOffset(2018, 5, 3, 5, 7, 0, TimeSpan.Zero), candleEventMessage.After?.Bucket);
        }

        [Fact]
        public void parse_candle_message2()
        {
            var message = "{\"before\":null,\"after\":{\"bucket\":\"2020-11-04T08:36:00.000000Z\",\"symbol\":\"PTTEP\",\"venue\":\"Equity\",\"open\":80.5,\"high\":80.5,\"low\":80.5,\"close\":80.5,\"volume\":{\"scale\":0,\"value\":\"AA==\"},\"amount\":\"0.0\"},\"source\":{\"version\":\"2.7.2.Final\",\"connector\":\"postgresql\",\"name\":\"dbserver1\",\"ts_ms\":1737541131908,\"snapshot\":\"false\",\"db\":\"postgres\",\"sequence\":[\"85427408248\",\"82628989304\"],\"ts_us\":1737541131908456,\"ts_ns\":1737541131908456000,\"schema\":\"public\",\"table\":\"candle_1_min\",\"txId\":57530085,\"lsn\":82628989304,\"xmin\":null},\"transaction\":null,\"op\":\"c\",\"ts_ms\":1737595843797,\"ts_us\":1737595843797743,\"ts_ns\":\"1737595843797743891\"}";
            var candleEventMessage = JsonConvert.DeserializeObject<CandleEventMessage>(message);
            Console.WriteLine(candleEventMessage);
            Assert.NotNull(candleEventMessage);
            Assert.Equal("PTTEP", candleEventMessage.After?.Symbol);
            Assert.Equal("Equity", candleEventMessage.After?.Venue);
            Assert.Equal(new DateTimeOffset(2020, 11, 4, 8, 36, 0, TimeSpan.Zero), candleEventMessage.After?.Bucket);
        }
    }
}
