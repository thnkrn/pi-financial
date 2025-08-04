using MongoDB.Bson;
using Pi.GlobalMarketData.Application.Helpers;
using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Tests.Helpers;

public class RankingHelperTests
{
    [Fact]
    public void Rank_ShouldSortInstrumentsByAmount_HighestFirst()
    {
        // Arrange
        var rankingItems = new List<RankingItem>
        {
            new RankingItem
            {
                Date = DateTime.Today,
                Symbol = "AAPL",
                Venue = "NASDAQ",
                AmountDouble = 1000.50
            },
            new RankingItem
            {
                Date = DateTime.Today,
                Symbol = "MSFT",
                Venue = "NASDAQ",
                AmountDouble = 1500.75
            },
            new RankingItem
            {
                Date = DateTime.Today,
                Symbol = "GOOGL",
                Venue = "NASDAQ",
                AmountDouble = 2000.25
            },
            // Multiple entries for same Symbol/Venue - should use highest
            new RankingItem
            {
                Date = DateTime.Today.AddDays(-1),
                Symbol = "AAPL",
                Venue = "NASDAQ",
                AmountDouble = 1200.50
            }
        };

        var instruments = new List<GeInstrument>
        {
            new GeInstrument
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Symbol = "AAPL",
                Venue = "NASDAQ",
                Name = "Apple Inc."
            },
            new GeInstrument
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Symbol = "MSFT",
                Venue = "NASDAQ",
                Name = "Microsoft Corporation"
            },
            new GeInstrument
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Symbol = "GOOGL",
                Venue = "NASDAQ",
                Name = "Alphabet Inc."
            },
            new GeInstrument
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Symbol = "AMZN",
                Venue = "NASDAQ",
                Name = "Amazon Inc."
            }
        };

        // Act
        var result = RankingHelper.Rank(rankingItems, instruments);

        // Assert
        Assert.Equal(4, result.Count);

        // Verify order: GOOGL (2000.25) > MSFT (1500.75) > AAPL (1200.50) > AMZN (0)
        Assert.Equal("GOOGL", result[0].Symbol);
        Assert.Equal("MSFT", result[1].Symbol);
        Assert.Equal("AAPL", result[2].Symbol);
        Assert.Equal("AMZN", result[3].Symbol);

        // Verify that we use the highest amount for AAPL (1200.50 instead of 1000.50)
        Assert.Equal(instruments.First(i => i.Symbol == "AAPL"), result[2]);
    }

    [Fact]
    public void Rank_ShouldHandleEmptyLists()
    {
        // Arrange
        var emptyRankingItems = new List<RankingItem>();
        var emptyInstruments = new List<GeInstrument>();
        var instruments = new List<GeInstrument>
        {
            new GeInstrument { Id = ObjectId.GenerateNewId().ToString(), Symbol = "AAPL", Venue = "NASDAQ" }
        };

        // Act & Assert
        var result1 = RankingHelper.Rank(emptyRankingItems, instruments);
        Assert.Single(result1);  // Should return all instruments with 0 rank

        var result2 = RankingHelper.Rank(emptyRankingItems, emptyInstruments);
        Assert.Empty(result2);   // Should return empty list
    }

    [Fact]
    public void Rank_ShouldMatchBySymbolAndVenue()
    {
        // Arrange
        var rankingItems = new List<RankingItem>
        {
            new RankingItem
            {
                Date = DateTime.Today,
                Symbol = "AAPL",
                Venue = "NASDAQ",
                AmountDouble = 1000.50
            },
            new RankingItem
            {
                Date = DateTime.Today,
                Symbol = "AAPL",
                Venue = "NYSE",
                AmountDouble = 2000.50
            }
        };

        var instruments = new List<GeInstrument>
        {
            new GeInstrument
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Symbol = "AAPL",
                Venue = "NASDAQ",
                Name = "Apple NASDAQ"
            },
            new GeInstrument
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Symbol = "AAPL",
                Venue = "NYSE",
                Name = "Apple NYSE"
            }
        };

        // Act
        var result = RankingHelper.Rank(rankingItems, instruments);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("NYSE", result[0].Venue);    // NYSE should be first (higher amount)
        Assert.Equal("NASDAQ", result[1].Venue);  // NASDAQ should be second (lower amount)
    }

    [Fact]
    public void CalculateRankingStartDate_OnWeekday_ReturnsCorrectDate()
    {
        // Arrange
        var currentUtcDate = new DateTime(2025, 4, 4, 10, 0, 0, DateTimeKind.Utc); // Friday
        var marketStartTimeBangkokTimeZone = "09:30:00";

        // Act
        var result = RankingHelper.CalculateRankingStartDate(currentUtcDate, marketStartTimeBangkokTimeZone);

        // Assert
        var expectedDate = new DateTime(2025, 4, 4, 2, 30, 0, DateTimeKind.Utc);
        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public void CalculateRankingStartDate_OnWeekday_ButBeforeMarketStartTime_ReturnsPreviousDay()
    {
        // Arrange
        var currentUtcDate = new DateTime(2025, 4, 4, 8, 0, 0, DateTimeKind.Utc); // Friday
        var marketStartTimeBangkokTimeZone = "20:30:00";

        // Act
        var result = RankingHelper.CalculateRankingStartDate(currentUtcDate, marketStartTimeBangkokTimeZone);

        // Assert
        var expectedDate = new DateTime(2025, 4, 3, 13, 30, 0, DateTimeKind.Utc);
        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public void CalculateRankingStartDate_OnMonday_ReturnsFridayWithCorrectTime()
    {
        // Arrange
        var currentDate = new DateTime(2025, 4, 7, 0, 0, 0, DateTimeKind.Utc); // Monday
        var marketStartTime = "09:30:00";

        // Act
        var result = RankingHelper.CalculateRankingStartDate(currentDate, marketStartTime);

        // Assert
        var expectedDate = new DateTime(2025, 4, 4, 2, 30, 0, DateTimeKind.Utc); // Friday

        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public void CalculateRankingStartDate_WithDifferentTimes_ReturnsCorrectTimeAdjusted()
    {
        // Arrange
        var currentDate = new DateTime(2025, 4, 3, 0, 0, 0, DateTimeKind.Utc); // Thursday
        var marketStartTime = "16:45:00";

        // Act
        var result = RankingHelper.CalculateRankingStartDate(currentDate, marketStartTime);

        // Assert
        var expectedDate = new DateTime(2025, 4, 2, 09, 45, 0, DateTimeKind.Utc);

        Assert.Equal(expectedDate, result);
    }
}