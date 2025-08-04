using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Infrastructure.Factories;
using Pi.BackofficeService.Infrastructure.Models.ReportService;

namespace Pi.BackofficeService.Infrastructure.Tests.Factories;

public class QueryFactoryTest
{
    [Fact]
    public void Should_Return_ReportHistory_When_NewReportHistory()
    {
        // Arrange
        var reportResponse = new ReportHistoryResponse()
        {
            Id = Guid.NewGuid(),
            ReportName = "report name",
            UserName = "some username",
            Status = "some status",
            DateFrom = DateTime.Now,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        // Act
        var actual = QueryFactory.NewReportHistory(reportResponse);

        // Assert
        Assert.IsType<ReportHistory>(actual);
    }

    [Fact]
    public void Should_Return_ReportHistory_And_MappingAsExpected_When_NewReportHistory()
    {
        // Arrange
        var reportResponse = new ReportHistoryResponse
        {
            Id = Guid.NewGuid(),
            ReportName = "Deposit/Withdraw Reconcile Report (ALL)",
            UserName = "some username",
            Status = "fail",
            DateFrom = DateTime.Now,
            DateTo = DateTime.Now,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        // Act
        var actual = QueryFactory.NewReportHistory(reportResponse);

        // Assert
        Assert.IsType<ReportType>(actual.Type);
        Assert.IsType<ReportStatus>(actual.Status);
        Assert.NotNull(actual.Name);
        Assert.Equal(reportResponse.Id, actual.Id);
        Assert.Equal(reportResponse.UserName, actual.UserName);
        Assert.Equal(DateOnly.FromDateTime((DateTime)reportResponse.DateFrom), actual.DateFrom);
        Assert.Equal(DateOnly.FromDateTime((DateTime)reportResponse.DateTo), actual.DateTo);
        Assert.Equal(reportResponse.UpdatedAt, actual.GeneratedAt);
        Assert.Equal(reportResponse.UpdatedAt, actual.UpdatedAt);
        Assert.Equal(reportResponse.CreatedAt, actual.CreatedAt);
    }

    [Fact]
    public void Should_Return_ReportHistory_And_TypeIsNuull_When_NewReportHistory_And_ReportNameMisMatched()
    {
        // Arrange
        var reportResponse = new ReportHistoryResponse
        {
            Id = Guid.NewGuid(),
            ReportName = "random",
            UserName = "some username",
            Status = "some status",
            DateFrom = DateTime.Now,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        // Act
        var actual = QueryFactory.NewReportHistory(reportResponse);

        // Assert
        Assert.Null(actual.Type);
    }

    [Theory]
    [InlineData(ReportStatus.Success, "Done")]
    [InlineData(ReportStatus.Fail, "Failed")]
    [InlineData(ReportStatus.Processing, "Processing")]
    public void Should_Return_ExpectedReportStatus_When_NewReportStatus(ReportStatus expected, string reportStatus)
    {
        // Act
        var actual = QueryFactory.NewReportStatus(reportStatus);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(ReportType.AllDWReconcile, "Deposit/Withdraw Reconcile Report (ALL)")]
    [InlineData(ReportType.VelexaEODTradeVAT, "Velexa Trades Vat")]
    [InlineData(ReportType.PendingTransaction, "Pending Transaction Report")]
    [InlineData(ReportType.VelexaEODTrade, "Velexa Trades")]
    [InlineData(ReportType.VelexaEODAccountSummary, "Velexa Account Summary")]
    [InlineData(ReportType.BloombergEOD, "Bloomberg Equity Closeprice")]
    [InlineData(ReportType.VelexaEODTransaction, "Velexa Transactions")]
    public void Should_Return_ExpectedReportType_When_NewReportType(ReportType expected, string reportName)
    {
        // Act
        var actual = QueryFactory.NewReportType(reportName);

        // Assert
        Assert.Equal(expected, actual);
    }
}
