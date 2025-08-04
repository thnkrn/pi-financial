using System;
using Xunit;
using Pi.SetMarketDataWSS.Application.Services.ItchMapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Domain.Entities;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Tests.Services.ItchMapper
{
    public class ItchTickSizeTableEntryMapperServiceTests
    {
        private readonly ItchTickSizeTableEntryMapperService _mapper;

        public ItchTickSizeTableEntryMapperServiceTests()
        {
            _mapper = new ItchTickSizeTableEntryMapperService();
        }

        [Fact]
        public void Map_WithValidTickSizeTableMessage_ReturnsCorrectTickSizeTableEntry()
        {
            // Arrange
            var message = new TickSizeTableMessageWrapper
            {
                OrderBookId = new OrderBookId { Value = 123 },
                TickSize = new Price { Value = 100 },
                PriceFrom = new Price { Value = 1000 },
                PriceTo = new Price { Value = 2000 }
            };
            var instrumentDetail = new InstrumentDetail { Decimals = 2 };

            // Act
            var result = _mapper.Map(message, null, instrumentDetail);

            // Assert
            Assert.Equal(123, result.OrderBookId);
            Assert.Equal(1.00m, result.TickSize);
            Assert.Equal(10.00m, result.PriceFrom);
            Assert.Equal(20.00m, result.PriceTo);
        }

        [Fact]
        public void Map_WithCachedTickSizeTableEntry_UpdatesExistingEntry()
        {
            // Arrange
            var message = new TickSizeTableMessageWrapper
            {
                OrderBookId = new OrderBookId { Value = 456 },
                TickSize = new Price { Value = 50 },
                PriceFrom = new Price { Value = 500 },
                PriceTo = new Price { Value = 1000 }
            };
            var cachedEntry = new TickSizeTableEntry
            {
                OrderBookId = 789,
                TickSize = 2.00m,
                PriceFrom = 20.00m,
                PriceTo = 30.00m
            };
            var instrumentDetail = new InstrumentDetail { Decimals = 2 };

            // Act
            var result = _mapper.Map(message, cachedEntry, instrumentDetail);

            // Assert
            Assert.Same(cachedEntry, result);
            Assert.Equal(456, result.OrderBookId);
            Assert.Equal(0.50m, result.TickSize);
            Assert.Equal(5.00m, result.PriceFrom);
            Assert.Equal(10.00m, result.PriceTo);
        }

        [Fact]
        public void Map_WithNullInstrumentDetail_UsesZeroDecimals()
        {
            // Arrange
            var message = new TickSizeTableMessageWrapper
            {
                OrderBookId = new OrderBookId { Value = 789 },
                TickSize = new Price { Value = 10 },
                PriceFrom = new Price { Value = 100 },
                PriceTo = new Price { Value = 200 }
            };

            // Act
            var result = _mapper.Map(message, null, null);

            // Assert
            Assert.Equal(789, result.OrderBookId);
            Assert.Equal(10m, result.TickSize);
            Assert.Equal(100m, result.PriceFrom);
            Assert.Equal(200m, result.PriceTo);
        }

        [Fact]
        public void Map_WithInvalidMessageType_ThrowsArgumentException()
        {
            // Arrange
            var invalidMessage = new InvalidItchMessage();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _mapper.Map(invalidMessage, null, null));
        }

        private class InvalidItchMessage : ItchMessage { }
    }
}
