using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.TfexService.API.Controllers;
using Pi.TfexService.Application.Commands.Order;
using Pi.TfexService.Application.Queries.Order;

namespace Pi.TfexService.API.Tests.Controllers.Order;

public class BaseOrderControllerTests
{
    private readonly Mock<IBus> _busMock = new();
    protected readonly Mock<ISetTradeOrderQueries> SetTradeOrderQueriesMock = new();
    protected readonly Mock<IItOrderTradeQueries> ItOrderQueriesMock = new();
    protected readonly OrderController OrderController;
    protected ServiceProvider Provider { get; set; }

    protected BaseOrderControllerTests()
    {
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<PlaceOrderConsumer>(); })
            .BuildServiceProvider(true);

        OrderController = new OrderController(_busMock.Object, SetTradeOrderQueriesMock.Object, ItOrderQueriesMock.Object);
    }
}