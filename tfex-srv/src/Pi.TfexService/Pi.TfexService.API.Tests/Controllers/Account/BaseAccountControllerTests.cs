using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.TfexService.API.Controllers;
using Pi.TfexService.Application.Commands.Account;
using Pi.TfexService.Application.Queries.Account;

namespace Pi.TfexService.API.Tests.Controllers.Account;

public class BaseAccountControllerTests
{
    protected readonly Mock<ISetTradeAccountQueries> SetTradeAccountQueriesMock = new();
    protected readonly AccountController AccountController;

    protected BaseAccountControllerTests()
    {
        AccountController = new AccountController(SetTradeAccountQueriesMock.Object);
    }
}