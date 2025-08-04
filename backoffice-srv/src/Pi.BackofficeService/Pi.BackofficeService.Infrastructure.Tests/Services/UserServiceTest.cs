using Moq;
using Pi.BackofficeService.Application.Models.Customer;
using Pi.BackofficeService.Infrastructure.Services;
using Pi.Client.UserService.Api;
using Pi.Client.UserService.Model;

namespace Pi.BackofficeService.Infrastructure.Tests.Services
{
    public class UserServiceTest
    {
        private readonly Mock<IUserApi> _userApi;

        private CustomerDto _expectedUser;

        public UserServiceTest()
        {
            _userApi = new Mock<IUserApi>();
            _expectedUser = new CustomerDto(
                Guid.NewGuid().ToString(),
                new List<DeviceDto>() { new DeviceDto(Guid.NewGuid(), "token1", "deviceIdentifier1", "eng", "Android", null) },
                new List<CustomerCodeDto>() { new CustomerCodeDto("123", new List<string>() { "111111", "222222" }) },
                new List<TradingAccountDto>() { new TradingAccountDto("ABC111"), new TradingAccountDto("ABC222") },
                "สมชาย", "ชัยดี", "John", "Smith", "0413111111", "", "test@email.com");
        }

        [Fact]
        public async Task Should_Call_GetUserByIdOrCustomerCode_When_GetUserByIdOrCustomerCodeV2Async_is_called_StatusCodeOk()
        {
            _userApi.Setup(x => x.GetUserByIdOrCustomerCodeV2Async(It.IsAny<string>(), It.IsAny<bool?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PiUserApplicationModelsUserApiResponse()
                {
                    Data = new PiUserApplicationModelsUser()
                    {
                        Devices = new List<PiUserApplicationModelsDevice>(),
                        CustomerCodes = new List<PiUserApplicationModelsCustomerCode>(),
                        TradingAccounts = new List<PiUserDomainAggregatesModelUserInfoAggregateTradingAccount>(),
                    }
                });

            var service = new UserService(_userApi.Object);

            //ACT
            var response = await service.GetUserByIdOrCustomerCode("123", true);

            // ASSERT
            _userApi.Verify(p => p.GetUserByIdOrCustomerCodeV2Async("123", true, new CancellationToken()), Times.Once());
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Should_Return_Null_If_GetUserByIdOrCustomerCodeV2Async_Thrown_An_Error()
        {
            _userApi.Setup(x => x.GetUserByIdOrCustomerCodeV2Async(It.IsAny<string>(), It.IsAny<bool?>(), It.IsAny<CancellationToken>()))
                .Throws(new ApplicationException("error occured"));

            var service = new UserService(_userApi.Object);

            //ACT
            var response = await service.GetUserByIdOrCustomerCode("123", true);

            // ASSERT
            _userApi.Verify(p => p.GetUserByIdOrCustomerCodeV2Async("123", true, new CancellationToken()), Times.Once());
            Assert.Null(response);
        }
    }
}
