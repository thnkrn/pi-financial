using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Pi.User.API.Controllers;
using Pi.User.API.Models;
using Pi.User.Application.Commands;
using Pi.User.Domain.AggregatesModel.UserAccountAggregate;

namespace Pi.User.API.Tests.Controllers;

public class UserAccountControllerTests : ConsumerTest
{
    private readonly UserAccountController _userAccountController;

    public UserAccountControllerTests()
    {
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddHandler<CreateUserAccount>(async ctx =>
                {
                    await ctx.RespondAsync(Task.CompletedTask);
                });
            })
            .AddMediator(cfg => { })
            .BuildServiceProvider(true);

        var bus = Provider.GetRequiredService<IBus>();
        _userAccountController = new UserAccountController(bus);
    }

    [Fact]
    public async Task CreateUserAccount_WhenCalled_ReturnsAccepted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createUserRequest = new CreateUserAccountRequest
        {
            UserAccountId = "WALLET001",
            UserAccountType = UserAccountType.CashWallet
        };

        // Act
        var result = await _userAccountController.CreateUserAccount(userId, createUserRequest);

        // Assert
        Assert.IsType<AcceptedResult>(result);
    }
}