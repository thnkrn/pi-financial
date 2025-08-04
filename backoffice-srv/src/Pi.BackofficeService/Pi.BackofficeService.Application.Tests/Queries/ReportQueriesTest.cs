using Moq;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Models.Commons;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Application.Services.ReportService;

namespace Pi.BackofficeService.Application.Tests.Queries;

public class ReportQueriesTest
{
    private readonly ReportQueries _reportQueries;
    private readonly Mock<IReportService> _reportService;

    public ReportQueriesTest()
    {
        _reportService = new Mock<IReportService>();
        _reportQueries = new ReportQueries(_reportService.Object);
    }

    [Fact]
    public async Task Should_Return_ReportTypes_When_reportGeneratedTypeIsAuto()
    {
        // Arrange
        _reportService.Setup(q => q.GetReportTypesByGeneratedTypes(ReportGeneratedType.Auto)
        ).ReturnsAsync(new List<ReportType>() { ReportType.PendingTransaction, ReportType.BloombergEOD, ReportType.VelexaEODTrade, ReportType.VelexaEODTransaction });

        // Act
        var actual = await _reportQueries.GetReportTypes(ReportGeneratedType.Auto);

        // Assert
        Assert.NotEmpty(actual);
        Assert.IsType<List<ReportType>>(actual);
    }

    [Fact]
    public async Task Should_Return_ReportTypes_When_reportGeneratedTypeIsOnDemand()
    {
        // Arrange
        _reportService.Setup(q => q.GetReportTypesByGeneratedTypes(ReportGeneratedType.OnDemand)
        ).ReturnsAsync(new List<ReportType>() { ReportType.PendingTransaction });

        // Act
        var actual = await _reportQueries.GetReportTypes(ReportGeneratedType.OnDemand);

        // Assert
        Assert.NotEmpty(actual);
        Assert.IsType<List<ReportType>>(actual);
    }

    [Fact]
    public async Task Should_Return_ReportTypes_When_reportGeneratedTypeIsNull()
    {
        // Act
        var actual = await _reportQueries.GetReportTypes(null);

        // Assert
        Assert.NotEmpty(actual);
        Assert.IsType<List<ReportType>>(actual);
    }

    [Fact]
    public async Task Should_Return_PaginateResultReportQueryResult_When_GetPaginateReportHistories()
    {
        // Arrange
        var records = new List<ReportHistory>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Type = ReportType.AllDWReconcile,
                UserName = "Test User",
                Status = ReportStatus.Processing,
                Name = "report name",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                DateFrom = DateOnly.FromDateTime(DateTime.Now),
                DateTo = DateOnly.FromDateTime(DateTime.Now),
                GeneratedAt = DateTime.Now
            }
        };
        _reportService.Setup(q => q.GetReportHistories(It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<ReportFilter>())
        ).ReturnsAsync(new PaginateResult<ReportHistory>(records, 1, 10, records.Count, null, null));

        // Act
        var actual = await _reportQueries.GetPaginateReportHistories(1, 10, null, null, new ReportFilter { });
        // Assert
        Assert.IsType<PaginateResult<ReportHistory>>(actual);
    }

    [Fact]
    public async Task Should_Return_RecordsNotEmpty_When_GetPaginateReportHistories_And_RecordsNotEmpty()
    {
        // Arrange
        var records = new List<ReportHistory>()
        {
            new() {
                Id = Guid.NewGuid(),
                Type = ReportType.AllDWReconcile,
                UserName = "Test User",
                Status = ReportStatus.Success,
                Name = "report name",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                DateFrom = DateOnly.FromDateTime(DateTime.Now),
                DateTo = DateOnly.FromDateTime(DateTime.Now),
                GeneratedAt = DateTime.Now
            }
        };
        _reportService.Setup(q => q.GetReportHistories(It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<ReportFilter>())
        ).ReturnsAsync(new PaginateResult<ReportHistory>(records, 1, 10, records.Count, null, null));

        // Act
        var actual = await _reportQueries.GetPaginateReportHistories(1, 10, null, null, new ReportFilter { });

        // Assert
        Assert.NotEmpty(actual.Records);
    }

    [Fact]
    public async Task Should_Return_RecordsEmpty_When_GetPaginateReportHistories_And_RecordsEmpty()
    {
        // Arrange
        var records = new List<ReportHistory>();
        _reportService.Setup(q => q.GetReportHistories(It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<ReportFilter>())
        ).ReturnsAsync(new PaginateResult<ReportHistory>(records, 1, 10, records.Count, null, null));

        // Act
        var actual = await _reportQueries.GetPaginateReportHistories(1, 10, null, null, new ReportFilter { });

        // Assert
        Assert.Empty(actual.Records);
    }

    [Fact]
    public async Task Should_Return_Paginate_With_Page_EqualServiceResponse_When_GetPaginateReportHistories()
    {
        // Arrange
        var records = new List<ReportHistory>();
        var expectedPage = 1;
        _reportService.Setup(q => q.GetReportHistories(It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<ReportFilter>())
        ).ReturnsAsync(new PaginateResult<ReportHistory>(records, expectedPage, 10, records.Count, null, null));

        // Act
        var actual = await _reportQueries.GetPaginateReportHistories(1, 10, null, null, new ReportFilter { });

        // Assert
        Assert.Equal(actual.Page, expectedPage);
    }

    [Fact]
    public async Task Should_Return_Paginate_With_PageSize_EqualServiceResponse_When_GetPaginateReportHistories()
    {
        // Arrange
        var records = new List<ReportHistory>();
        var expectedPageSize = 1;
        _reportService.Setup(q => q.GetReportHistories(It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<ReportFilter>())
        ).ReturnsAsync(new PaginateResult<ReportHistory>(records, 1, expectedPageSize, records.Count, null, null));

        // Act
        var actual = await _reportQueries.GetPaginateReportHistories(1, 10, null, null, new ReportFilter { });

        // Assert
        Assert.Equal(actual.PageSize, expectedPageSize);
    }

    [Fact]
    public async Task Should_Return_Paginate_With_Total_EqualServiceResponse_When_GetPaginateReportHistories()
    {
        // Arrange
        var records = new List<ReportHistory>();
        var expectedTotal = 1;
        _reportService.Setup(q => q.GetReportHistories(It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ReportFilter>()
            )
        ).ReturnsAsync(new PaginateResult<ReportHistory>(records, 1, 1, expectedTotal, null, null));

        // Act
        var actual = await _reportQueries.GetPaginateReportHistories(1, 10, null, null, new ReportFilter { });

        // Assert
        Assert.Equal(actual.Total, expectedTotal);
    }

    [Fact]
    public async Task Should_Return_Paginate_With_Orderby_EqualServiceResponse_When_GetPaginateReportHistories()
    {
        // Arrange
        var records = new List<ReportHistory>();
        var expectedOrderBy = "someOrderBy";
        _reportService.Setup(q => q.GetReportHistories(It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<ReportFilter>())
        ).ReturnsAsync(new PaginateResult<ReportHistory>(records, 1, 1, 10, expectedOrderBy, null));

        // Act
        var actual = await _reportQueries.GetPaginateReportHistories(1, 10, null, null, new ReportFilter { });

        // Assert
        Assert.Equal(actual.OrderBy, expectedOrderBy);
    }

    [Fact]
    public async Task Should_Return_Paginate_With_OrderDir_EqualServiceResponse_When_GetPaginateReportHistories()
    {
        // Arrange
        var records = new List<ReportHistory>();
        var expectedOrderDir = "Desc";
        _reportService.Setup(q => q.GetReportHistories(It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<ReportFilter>())
        ).ReturnsAsync(new PaginateResult<ReportHistory>(records, 1, 1, 10, null, expectedOrderDir));

        // Act
        var actual = await _reportQueries.GetPaginateReportHistories(1, 10, null, null, new ReportFilter { });

        // Assert
        Assert.Equal(actual.OrderDir, expectedOrderDir);
    }

    [Theory]
    [InlineData("www.test.com")]
    [InlineData(null)]
    public async Task Should_Return_ExpectedUrl_When_GetUrlByReportId(string? expectedUrl)
    {
        // Arrange
        _reportService.Setup(q => q.GetFileUrl(It.IsAny<Guid>()))
            .ReturnsAsync(expectedUrl);

        // Act
        var actual = await _reportQueries.GetUrl(Guid.NewGuid());

        // Assert
        Assert.Equal(actual, expectedUrl);
    }
}
