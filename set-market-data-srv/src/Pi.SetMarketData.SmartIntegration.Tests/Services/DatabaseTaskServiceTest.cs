
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi.SetMarketData.Application.Interfaces.BrokerIdMapperService;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Entities.SetSmart;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Services.SqlServer;
using Pi.SetMarketData.SmartIntegration.Configurations;
using Pi.SetMarketData.SmartIntegration.Context;
using Pi.SetMarketData.SmartIntegration.Services;

namespace Pi.SetMarketData.SmartIntegration.Tests.Services;

public class DatabaseTaskServiceTests
{
    private readonly TestSqlServerContext _context;
    private readonly Mock<IMongoService<Instrument>> _mockInstrumentService;
    private readonly Mock<IBrokerIdMapperService>_mockBrokerIdMapperService;
    private readonly Mock<IMongoService<BrokerInfo>> _mockBrokerInfoService;
    private readonly Mock<ILogger<DatabaseTaskService>> _mockLogger;
    private readonly DatabaseTaskService _service;

    public DatabaseTaskServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<SqlServerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestSqlServerContext(options);
        _mockInstrumentService = new Mock<IMongoService<Instrument>>();
        _mockBrokerIdMapperService = new Mock<IBrokerIdMapperService>();
        _mockBrokerInfoService = new Mock<IMongoService<BrokerInfo>>();
        _mockLogger = new Mock<ILogger<DatabaseTaskService>>();

        _service = new DatabaseTaskService(
            _context,
            _mockInstrumentService.Object,
            _mockBrokerIdMapperService.Object,
            _mockBrokerInfoService.Object,
            _mockLogger.Object
        );
    }

    internal void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task PerformDatabaseTask_ProcessesDataCorrectly()
    {
        // Arrange
        var security = new Security
        {
            ISecurity = 1,
            ICompany = 1,
            IMarket = '1',
            NSecurity = "TEST1",
            NSecurityE = "Test Security 1"
        };

        var securityDetail = new SecurityDetail
        {
            ISecurity = 1,
            ZMultiplier = 1.0m,
            ZExercise = 100.0m,
            QFirstRatio = 1,
            QLastRatio = 1,
            QTtm = 30,
            DAutoExercise = DateTime.Now.AddDays(30),
            DFirstTrade = DateTime.Now,
            DLastTrade = DateTime.Now.AddDays(30)
        };

        _context.Security.Add(security);
        _context.SecurityDetail.Add(securityDetail);
        await _context.SaveChangesAsync();

        var testInstruments = new List<Instrument>
        {
            new() { Symbol = "TEST1" }
        };

        _mockInstrumentService
            .Setup(s => s.GetAllByFilterAsync(It.IsAny<Expression<Func<Instrument, bool>>>()))
            .ReturnsAsync(testInstruments);

        // Act
        await _service.PerformDatabaseTask(new BatchUpdateOptions { BatchSize = 1, Delay = 0 });

        // Assert
        _mockInstrumentService.Verify(
            s => s.UpdateManyAsync(
                It.IsAny<List<Instrument>>(),
                It.IsAny<Expression<Func<Instrument, object>>>()
            ),
            Times.Once
        );

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)
            ),
            Times.AtLeast(2)
        );
    }

    [Fact]
    public async Task PerformDatabaseTask_HandlesEmptyData()
    {
        // Act
        await _service.PerformDatabaseTask(new BatchUpdateOptions());

        // Assert
        _mockInstrumentService.Verify(
            s => s.UpdateManyAsync(
                It.IsAny<List<Instrument>>(),
                It.IsAny<Expression<Func<Instrument, object>>>()
            ),
            Times.Never
        );
    }
}