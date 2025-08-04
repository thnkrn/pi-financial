using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Infrastructure.Services;

namespace Pi.BackofficeService.Infrastructure.Tests.Services;

public class ReportServiceTests
{
    private readonly Mock<ILogger<ReportService>> _logger;

    public ReportServiceTests()
    {
        _logger = new Mock<ILogger<ReportService>>();
    }

    [Theory]
    [InlineData("2024-06-01", "2024-06-30", ReportType.PiAppDepositWithdrawDailyReport)]
    [InlineData("2024-06-01", "2024-06-30", ReportType.PiAppGlobalReconcileReport)]
    public async Task DownloadDepositWithdrawDailyReport_PiAppDaily(string dateFrom, string dateTo, ReportType reportType)
    {
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            });
        var underTest = new ReportService(new HttpClient(mockMessageHandler.Object), "https://www.mock.url", "xxx", _logger.Object);
        var result =
            await underTest.DownloadDepositWithdrawDailyReport(DateOnly.Parse(dateFrom), DateOnly.Parse(dateTo), reportType);

        Assert.NotNull(result);
    }
}