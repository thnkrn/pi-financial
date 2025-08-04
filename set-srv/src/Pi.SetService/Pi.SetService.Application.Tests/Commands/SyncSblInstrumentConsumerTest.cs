using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.SetService.Application.Commands;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Services.SblService;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Application.Tests.Commands;

public class SyncSblInstrumentConsumerTest : ConsumerTest
{
    private readonly Mock<ISblService> _sblService;
    private readonly Mock<IInstrumentRepository> _instrumentRepo;

    public SyncSblInstrumentConsumerTest()
    {
        _sblService = new Mock<ISblService>();
        _instrumentRepo = new Mock<IInstrumentRepository>();

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<SyncSblInstrumentConsumer>(); })
            .AddScoped<ISblService>(_ => _sblService.Object)
            .AddScoped<IInstrumentRepository>(_ => _instrumentRepo.Object)
            .AddScoped<ILogger<SyncSblInstrumentConsumer>>(_ => Mock.Of<ILogger<SyncSblInstrumentConsumer>>())
            .BuildServiceProvider(true);
    }

    #region Sync Margin Instrument

    [Fact]
    public async Task Should_Return_Expected_Response_When_SyncMarginInstrument_Succeed()
    {
        // Arrange
        var client = Harness.GetRequestClient<SyncSblInstrument>();
        var request = new SyncSblInstrument
        {
            BucketName = "some-bucket-for-testing",
            FileKey = "sbl_upload 01082024.csv"
        };

        _sblService.Setup(q => q.GetSblInstrumentInfoFromStorage(It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .Returns(GenerateSblInstrumentInfos(800));
        _instrumentRepo.Setup(q => q.ClearSblInstrumentsAsync(It.IsAny<CancellationToken>()))
            .Verifiable();
        _instrumentRepo.SetupSequence(q => q.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(500)
            .ReturnsAsync(300);

        // Act
        var response = await client.GetResponse<SyncProcessResult>(request);

        // Assert
        _instrumentRepo.Verify(q => q.ClearSblInstrumentsAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(800, response.Message.Create);
        Assert.Equal(0, response.Message.Update);
        Assert.Equal(0, response.Message.Skip);
        Assert.Equal(800, response.Message.Execution);
    }

    private static async IAsyncEnumerable<SblInstrumentSyncInfo> GenerateSblInstrumentInfos(int length)
    {
        for (var i = 0; i < length; i++)
        {
            yield return new SblInstrumentSyncInfo
            {
                Symbol = $"Symbol-{i}",
                InterestRate = 5.00m,
                RetailLender = 5.00m
            };
        }

        await Task.CompletedTask;
    }
    #endregion
}
