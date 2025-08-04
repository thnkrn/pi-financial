using Pi.SetMarketDataWSS.Domain.Entities;
using Xunit;

namespace Pi.SetMarketDataWSS.Domain.Tests.Entities
{
    public class PriceInfoTests
    {
        [Fact]
        public void DefaultConstructor_ShouldCreateInstance()
        {
            var priceInfo = new PriceInfo();
            Assert.NotNull(priceInfo);
        }

        [Fact]
        public void ConstructorWithCurrency_ShouldSetCurrency()
        {
            var currency = "USD";
            var priceInfo = new PriceInfo(currency);
            Assert.Equal(currency, priceInfo.Currency);
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            var priceInfo = new PriceInfo();
            priceInfo.PriceInfoId = 1;
            priceInfo.InstrumentId = 2;
            priceInfo.Price = "100.00";
            // ... set other properties ...

            Assert.Equal(1, priceInfo.PriceInfoId);
            Assert.Equal(2, priceInfo.InstrumentId);
            Assert.Equal("100.00", priceInfo.Price);
            // ... assert other properties ...
        }

        [Fact]
        public void CalculatedPriceChangedRate_DefaultValue_ShouldBeFalse()
        {
            var priceInfo = new PriceInfo();
            Assert.False(priceInfo.CalculatedPriceChangedRate);
        }

        [Fact]
        public void Instrument_ShouldBeSettable()
        {
            var priceInfo = new PriceInfo();
            var instrument = new Instrument();
            priceInfo.Instrument = instrument;
            Assert.Same(instrument, priceInfo.Instrument);
        }

    }
}