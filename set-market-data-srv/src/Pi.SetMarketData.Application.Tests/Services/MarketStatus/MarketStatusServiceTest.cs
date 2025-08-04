using AutoFixture;
using Pi.SetMarketData.Application.Services.MarketData.MarketStatus;
using MongoDB.Bson;

namespace Pi.SetMarketData.Application.Tests.Services.MarketData.MarketStatus
{
    public class MarketStatusServiceTests
    {
        private readonly Fixture _fixture;

        public MarketStatusServiceTests()
        {
            _fixture = new Fixture();
            _fixture.Register(ObjectId.GenerateNewId);
        }

        [Fact]
        public void GetResult_ShouldReturnCorrectMarketStatus_FromFixtureData()
        {
            // Arrange
            var marketStatus = new Domain.Entities.MarketStatus
            {
                Status = "OPEN"
            };


            // Act
            var result = MarketStatusService.GetResult(marketStatus);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("0", result.Code);
            Assert.Equal("", result.Message);
            Assert.NotNull(result.Response);
            Assert.Equal("OPEN", result.Response.MarketStatus);
        }

        [Fact]
        public void GetResult_ShouldReturnEmptyMarketStatus_WhenNoDataIsProvided()
        {
            // Arrange
            var marketStatus = new Domain.Entities.MarketStatus();

            // Act
            var result = MarketStatusService.GetResult(marketStatus);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("0", result.Code);
            Assert.Equal("", result.Message);
            Assert.NotNull(result.Response);
            Assert.Equal("", result.Response.MarketStatus);
        }
    }
}
