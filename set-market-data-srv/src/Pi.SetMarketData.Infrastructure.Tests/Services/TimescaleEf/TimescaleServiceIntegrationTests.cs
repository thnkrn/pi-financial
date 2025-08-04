using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Services.TimescaleEf;

namespace Pi.SetMarketData.Infrastructure.Tests.Services.TimescaleEf;

public class TimescaleServiceIntegrationTests : IDisposable
{
    private readonly TimescaleContext _context;
    private readonly IDbContextFactory<TimescaleContext> _contextFactory;
    private readonly TimescaleService<RealtimeMarketData> _service;

    public TimescaleServiceIntegrationTests()
    {
        // Set connection string
        const string connectionString = "Host=localhost;Database=stockdb;Username=admin;Password=admin;Port=5432";

        // Set up the database context
        var options = new DbContextOptionsBuilder<TimescaleContext>()
            .UseNpgsql(connectionString)
            .Options;
        _contextFactory = new Mock<IDbContextFactory<TimescaleContext>>().Object;
        _context = new TimescaleContext(options);

        // Ensure the database is created and the schema is up to date
        _context.Database.EnsureCreated();

        // Ensure the hypertable is created
        _context.TimescaleCreateHypertable();

        // Clean up the test data before each test
        CleanupTestData();

        // Set up mock loggers
        Mock<ILogger<TimescaleService<RealtimeMarketData>>> mockServiceLogger = new();
        Mock<ILogger<TimescaleRepository<RealtimeMarketData>>> mockRepositoryLogger = new();

        // Create the repository and service
        var repository =
            new TimescaleRepository<RealtimeMarketData>(null, _contextFactory, mockRepositoryLogger.Object);
        _service = new TimescaleService<RealtimeMarketData>(repository, mockServiceLogger.Object);
    }

    public void Dispose()
    {
        CleanupTestData();
        _context.Dispose();
    }

    private void CleanupTestData()
    {
        try
        {
            _context.Database.ExecuteSqlRaw(
                new StringBuilder().Append("TRUNCATE TABLE realtime_market_data").ToString());
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during cleanup: {ex.Message}");
            // If truncate fails, try deleting all records
            _context.RealtimeMarketData?.RemoveRange(_context.RealtimeMarketData);
            _context.SaveChanges();
        }
    }

    [Fact]
    public async Task CreateAsync_ShouldInsertNewRecord()
    {
        // Arrange
        var newData = new RealtimeMarketData
        {
            DateTime = DateTimeOffset.UtcNow,
            Symbol = "TEST",
            Venue = "VENUE",
            Price = 100.5,
            Volume = 1000,
            Amount = 100500
        };

        // Act
        await _service.CreateAsync(newData);

        // Assert
        if (_context.RealtimeMarketData != null)
        {
            var result = await _context.RealtimeMarketData.FindAsync(newData.DateTime, newData.Symbol, newData.Venue);
            Assert.NotNull(result);
            Assert.Equal(newData.Price, result.Price);
            Assert.Equal(newData.Volume, result.Volume);
            Assert.Equal(newData.Amount, result.Amount);
        }
    }

    [Fact]
    public async Task GetByFilterAsync_ShouldReturnMatchingRecord()
    {
        // Arrange
        var testData = new RealtimeMarketData
        {
            DateTime = DateTimeOffset.UtcNow,
            Symbol = "FILTER_TEST",
            Venue = "VENUE",
            Price = 200.5,
            Volume = 2000,
            Amount = 401000
        };
        await _service.CreateAsync(testData);

        // Act
        var result = await _service.GetByFilterAsync(x => x.Symbol == "FILTER_TEST" && x.Venue == "VENUE");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testData.Price, result.Price);
        Assert.Equal(testData.Volume, result.Volume);
        Assert.Equal(testData.Amount, result.Amount);
    }

    [Fact]
    public async Task GetSelectedHighestValueAsync_ShouldReturnHighestPrice()
    {
        // Arrange
        var testData1 = new RealtimeMarketData
        {
            DateTime = DateTimeOffset.UtcNow.AddMinutes(-5),
            Symbol = "HIGH_TEST",
            Venue = "VENUE",
            Price = 100.5,
            Volume = 1000,
            Amount = 100500
        };
        var testData2 = new RealtimeMarketData
        {
            DateTime = DateTimeOffset.UtcNow,
            Symbol = "HIGH_TEST",
            Venue = "VENUE",
            Price = 101.5,
            Volume = 1000,
            Amount = 101500
        };
        await _service.CreateAsync(testData1);
        await _service.CreateAsync(testData2);

        // Act
        var result = await _service.GetSelectedHighestValueAsync(
            x => x.Symbol == "HIGH_TEST" && x.Venue == "VENUE",
            x => x.Price,
            x => x.Price
        );

        // Assert
        Assert.Equal(101.5, result);
    }

    [Fact]
    public async Task UpsertAsync_ShouldUpdateExistingRecord()
    {
        // Arrange
        var initialData = new RealtimeMarketData
        {
            DateTime = DateTimeOffset.UtcNow,
            Symbol = "UPSERT_TEST",
            Venue = "VENUE",
            Price = 100.5,
            Volume = 1000,
            Amount = 100500
        };
        await _service.CreateAsync(initialData);

        var updatedData = new RealtimeMarketData
        {
            DateTime = initialData.DateTime,
            Symbol = "UPSERT_TEST",
            Venue = "VENUE",
            Price = 101.5,
            Volume = 1100,
            Amount = 111650
        };

        // Act
        await _service.UpsertAsync(updatedData, nameof(RealtimeMarketData.DateTime), nameof(RealtimeMarketData.Symbol),
            nameof(RealtimeMarketData.Venue));

        // Assert
        if (_context.RealtimeMarketData != null)
        {
            var result =
                await _context.RealtimeMarketData.FindAsync(initialData.DateTime, initialData.Symbol,
                    initialData.Venue);
            Assert.NotNull(result);
            Assert.Equal(updatedData.Price, result.Price);
            Assert.Equal(updatedData.Volume, result.Volume);
            Assert.Equal(updatedData.Amount, result.Amount);
        }
    }
}