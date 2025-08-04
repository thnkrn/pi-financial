using System.Threading;
using System.Threading.Tasks;
using Moq;
using Pi.Common.Domain.AggregatesModel.BankAggregate;
using Pi.Financial.Client.FundConnext.Model;
using Pi.Financial.FundService.Application.Queries;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.ItBackofficeService;
using Pi.Financial.FundService.Application.Services.MarketService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Xunit;

namespace Pi.Financial.FundService.Application.Tests.Queries.FundQueries
{
    public class IsFundAccountExistTests
    {
        private readonly Mock<IFundConnextService> _fundConnextServiceMock;
        private readonly Application.Queries.FundQueries _fundQueries;

        public IsFundAccountExistTests()
        {
            _fundConnextServiceMock = new Mock<IFundConnextService>();
            _fundQueries = new Application.Queries.FundQueries(
                _fundConnextServiceMock.Object,
                Mock.Of<IOnboardService>(),
                Mock.Of<IMarketService>(),
                Mock.Of<IBankInfoRepository>(),
                Mock.Of<IItBackofficeService>(),
                Mock.Of<IUserService>(),
                Mock.Of<IFundOrderRepository>()
            );
        }

        [Fact]
        public async Task IsFundAccountExistAsync_ShouldReturnTrue_WhenAccountExists()
        {
            // Arrange
            var identificationCardNo = "1234567890";
            var passportCountry = "US";
            var cancellationToken = CancellationToken.None;

            _fundConnextServiceMock
                .Setup(service =>
                    service.GetCustomerProfileAndAccount(identificationCardNo, passportCountry, cancellationToken))
                .ReturnsAsync(
                    new IndividualInvestorV5Response(accounts: new List<AccountV5Response> { new AccountV5Response() }));

            // Act
            var result =
                await _fundQueries.IsFundAccountExistAsync(identificationCardNo, passportCountry, cancellationToken);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsFundAccountExistAsync_ShouldReturnFalse_WhenAccountDoesNotExist()
        {
            // Arrange
            var identificationCardNo = "1234567890";
            var passportCountry = "US";
            var cancellationToken = CancellationToken.None;

            _fundConnextServiceMock
                .Setup(service =>
                    service.GetCustomerProfileAndAccount(identificationCardNo, passportCountry, cancellationToken))
                .ReturnsAsync(new IndividualInvestorV5Response());

            // Act
            var result =
                await _fundQueries.IsFundAccountExistAsync(identificationCardNo, passportCountry, cancellationToken);

            // Assert
            Assert.False(result);
        }
    }
}
