using Pi.BackofficeService.API.Factories;
using Pi.BackofficeService.API.Models;
using Pi.BackofficeService.Application.Commands.User;
using Pi.BackofficeService.Application.Factories;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.User;

namespace Pi.BackofficeService.Application.Tests.Factories;

public class DtoFactoryTest
{
    [Fact]
    public void Should_Return_Valid_OpenAccountsDto_When_Valid_Filter_Is_Passed_In()
    {
        var filter = new OnBoardingFilterRequest()
        {
            UserId = Guid.NewGuid(),
            Status = OpenAccountRequestStatus.PENDING,
            CitizenId = "456",
            Custcode = "789",
            Date = null,
            Page = 2,
            PageSize = 10,
            OrderBy = "custcode",
            OrderDir = "Desc"
        };

        // Act
        var action = DtoFactory.NewOnboardAccountFilter(filter);

        // Assert
        Assert.NotNull(action);
        Assert.Equal(filter.UserId, action.UserId);
        Assert.Equal(filter.Status.ToString(), action.Status);
        Assert.Equal(filter.CitizenId, action.CitizenId);
        Assert.Equal(filter.Custcode, action.Custcode);
        Assert.Equal(filter.Date, action.Date);
    }

    [Fact]
    public void Should_Return_Valid_OpenAccountsDto_When_Empty_Filter_Is_Passed_In()
    {
        var filter = new OnBoardingFilterRequest();
        // Act
        var action = DtoFactory.NewOnboardAccountFilter(filter);

        // Assert
        Assert.NotNull(action);
    }
}
