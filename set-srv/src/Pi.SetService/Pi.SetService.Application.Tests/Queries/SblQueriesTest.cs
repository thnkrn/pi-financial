using Moq;
using Pi.SetService.Application.Filters;
using Pi.SetService.Application.Queries;
using Pi.SetService.Domain.AggregatesModel.CommonAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using PaginateQuery = Pi.SetService.Domain.AggregatesModel.CommonAggregate.PaginateQuery;

namespace Pi.SetService.Application.Tests.Queries;

public class SblQueriesTest
{
    private readonly Mock<ISblOrderRepository> _sblOrderRepository;
    private readonly Mock<IInstrumentRepository> _instrumentRepository;
    private readonly SblQueries _sblQueries;

    public SblQueriesTest()
    {
        _sblOrderRepository = new Mock<ISblOrderRepository>();
        _instrumentRepository = new Mock<IInstrumentRepository>();
        _sblQueries = new SblQueries(_sblOrderRepository.Object, _instrumentRepository.Object);
    }

    [Fact]
    public async Task Should_ReturnSblOrder_When_GetSblOrdersAsync()
    {
        // Arrange
        var filters = new SblOrderFilters();
        var sblOrders = new List<SblOrder>()
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "0803174-6", "0803174", 1, "EA", SblOrderStatus.Pending, 100, SblOrderType.Borrow, null, null),
            new(Guid.NewGuid(), Guid.NewGuid(), "0803174-6", "0803174", 2, "EA", SblOrderStatus.Pending, 100, SblOrderType.Return, null, null),
            new(Guid.NewGuid(), Guid.NewGuid(), "0803174-6", "0803174", 3, "EA", SblOrderStatus.Approved, 100, SblOrderType.Borrow, null, null),
            new(Guid.NewGuid(), Guid.NewGuid(), "0803174-6", "0803174", 4, "EA", SblOrderStatus.Approved, 100, SblOrderType.Return, null, null),
            new(Guid.NewGuid(), Guid.NewGuid(), "0803174-6", "0803174", 5, "EA", SblOrderStatus.Rejected, 100, SblOrderType.Borrow, null, null),
            new(Guid.NewGuid(), Guid.NewGuid(), "0803174-6", "0803174", 6, "EA", SblOrderStatus.Rejected, 100, SblOrderType.Return, null, null),
        };
        var paginate = new PaginateResult<SblOrder>
        {
            Data = sblOrders,
            Page = 1,
            PageSize = 20,
            Total = 61
        };

        _sblOrderRepository.Setup(q => q.Paginate(It.IsAny<PaginateQuery>(), It.IsAny<SblOrderFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginate);

        // Act
        var actual = await _sblQueries.GetSblOrdersAsync(new PaginateQuery(), filters);

        // Assert
        Assert.Equal(paginate, actual);
    }

    [Fact]
    public async Task Should_ReturnSblOrder_When_GetSblInstrumentsAsync()
    {
        // Arrange
        var filters = new SblInstrumentFilters();
        var sblOrders = new List<SblInstrument>()
        {
            new(Guid.NewGuid(), "EA", 5.00m, 2000000, 1000000, 1000000),
            new(Guid.NewGuid(), "IVL", 5.00m, 2000000, 1000000, 1000000),
            new(Guid.NewGuid(), "AOT", 5.00m, 2000000, 1000000, 1000000),
        };
        var paginate = new PaginateResult<SblInstrument>
        {
            Data = sblOrders,
            Page = 1,
            PageSize = 20,
            Total = 61
        };

        _instrumentRepository.Setup(q => q.PaginateSblInstruments(It.IsAny<PaginateQuery>(), It.IsAny<SblInstrumentFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginate);

        // Act
        var actual = await _sblQueries.GetSblInstrumentsAsync(new PaginateQuery(), filters);

        // Assert
        Assert.Equal(paginate, actual);
    }
}
