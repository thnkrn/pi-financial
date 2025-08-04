using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.SetService.Application.Commands;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Models.InitialMargin;
using Pi.SetService.Application.Services.SbaService;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Application.Tests.Commands;

public class SyncInitialMarginConsumerTest : ConsumerTest
{
    private readonly Mock<ISbaService> _sbaService;
    private readonly Mock<IInstrumentRepository> _instrumentRepo;

    public SyncInitialMarginConsumerTest()
    {
        _sbaService = new Mock<ISbaService>();
        _instrumentRepo = new Mock<IInstrumentRepository>();

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<SyncInitialMarginConsumer>(); })
            .AddScoped<ISbaService>(_ => _sbaService.Object)
            .AddScoped<IInstrumentRepository>(_ => _instrumentRepo.Object)
            .AddScoped<ILogger<SyncInitialMarginConsumer>>(_ => Mock.Of<ILogger<SyncInitialMarginConsumer>>())
            .BuildServiceProvider(true);
    }

    #region Sync Margin Instrument

    [Fact]
    public async Task Should_Return_Expected_Response_When_SyncMarginInstrument_Succeed()
    {
        // Arrange
        var client = Harness.GetRequestClient<SyncInitialMarginRequest>();
        var request = new SyncInitialMarginRequest
        {
            BucketName = "some-bucket-for-testing",
            FileKey = "osec_ctrl.dat"
        };

        _sbaService.Setup(q => q.GetMarginInstrumentInfoFromStorage(It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .Returns(GenerateMarginInstrumentInfos(102));
        _instrumentRepo.SetupSequence(q =>
                q.GetEquityInfos(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenerateEquityInfos(50))
            .ReturnsAsync([]);
        _instrumentRepo.Setup(q => q.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        // Act
        var response = await client.GetResponse<SyncProcessResult>(request);

        // Assert
        Assert.Equal(52, response.Message.Create);
        Assert.Equal(17, response.Message.Update);
        Assert.Equal(33, response.Message.Skip);
        Assert.Equal(20, response.Message.Execution);
    }

    private static async IAsyncEnumerable<MarginInstrumentInfo> GenerateMarginInstrumentInfos(int length)
    {
        for (var i = 0; i < length; i++)
        {
            yield return new MarginInstrumentInfo()
            {
                Symbol = $"Symbol-{i}",
                MarginCode = "050",
                IsTurnoverList = i % 4 == 0
            };
        }

        await Task.CompletedTask;
    }

    private static IEnumerable<EquityInfo> GenerateEquityInfos(int length)
    {
        for (var i = 0; i < length; i++)
        {
            yield return new EquityInfo(Guid.NewGuid(), $"Symbol-{i}", "050", i % 5 == 0);
        }
    }
    #endregion

    #region Sync Margin Rate

    [Fact]
    public async Task Should_Return_Expected_Response_When_SyncMarginRate_Succeed()
    {
        // Arrange
        var client = Harness.GetRequestClient<SyncInitialMarginRequest>();
        var request = new SyncInitialMarginRequest
        {
            BucketName = "some-bucket-for-testing",
            FileKey = "omrg_tbl.dat"
        };

        _sbaService.Setup(q => q.GetMarginRatesFromStorage(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(GenerateMarginRateInfo(102));
        _instrumentRepo.SetupSequence(q =>
                q.GetEquityInitialMargins(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenerateEquityInitialMargin(50))
            .ReturnsAsync([]);
        _instrumentRepo.Setup(q => q.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        // Act
        var response = await client.GetResponse<SyncProcessResult>(request);

        // Assert
        Assert.Equal(52, response.Message.Create);
        Assert.Equal(25, response.Message.Update);
        Assert.Equal(25, response.Message.Skip);
        Assert.Equal(20, response.Message.Execution);
    }

    private static async IAsyncEnumerable<MarginRateInfo> GenerateMarginRateInfo(int length)
    {
        for (var i = 0; i < length; i++)
        {
            yield return new MarginRateInfo
            {
                MarginCode = $"0{i}",
                MarginRate = i
            };
        }

        await Task.CompletedTask;
    }

    private static IEnumerable<EquityInitialMargin> GenerateEquityInitialMargin(int length)
    {
        for (var i = 0; i < length; i++)
        {
            yield return new EquityInitialMargin(Guid.NewGuid(), $"0{i}", i % 2 == 0 ? i : i + 2);
        }
    }
    #endregion

    [Theory]
    [InlineData("random.dat")]
    [InlineData("test.dat")]
    [InlineData("osec_ctrl.docs")]
    [InlineData("omrg_tbl.pdf")]
    public async Task Should_Error_When_File_Not_Supported(string fileKey)
    {
        // Arrange
        var client = Harness.GetRequestClient<SyncInitialMarginRequest>();
        var request = new SyncInitialMarginRequest
        {
            BucketName = "some-bucket-for-testing",
            FileKey = fileKey
        };

        // Act
        var act = async () => await client.GetResponse<SyncProcessResult>(request);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(NotSupportedException).ToString()));
    }
}
