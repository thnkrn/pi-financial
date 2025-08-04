using Moq;
using Pi.BackofficeService.Application.Models.Commons;
using Pi.BackofficeService.Application.Models.Sbl;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Application.Services.SblService;

namespace Pi.BackofficeService.Application.Tests.Queries;

public class SblQueriesTest
{
    private readonly Mock<ISblService> _sblService;
    private readonly SblQueries _sblQueries;

    public SblQueriesTest()
    {
        _sblService = new Mock<ISblService>();
        _sblQueries = new SblQueries(_sblService.Object);
    }

    [Fact]
    public async Task Test_GetSblOrderPaginate_With_ValidFilters()
    {
        // Arrange
        var expected = new PaginateResult<SblOrder>([], 1, 10, 100, "created_at", "desc");
        _sblService.Setup(s => s.GetSblOrderPaginateAsync(It.IsAny<SblOrderFilters>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var actual = await _sblQueries.GetSblOrderPaginate(new SblOrderFilters(), 1, 10, "created_at", "descl");

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Test_GetSblInstrumentsPaginate_With_ValidFilters()
    {
        // Arrange
        var expected = new PaginateResult<SblInstrument>([], 1, 10, 100, "created_at", "desc");
        _sblService.Setup(s => s.GetSblInstrumentsPaginateAsync(It.IsAny<SblInstrumentFilters>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var actual = await _sblQueries.GetSblInstrumentsPaginate(new SblInstrumentFilters(), 1, 10, "created_at", "descl");

        // Assert
        Assert.Equal(expected, actual);
    }
}
