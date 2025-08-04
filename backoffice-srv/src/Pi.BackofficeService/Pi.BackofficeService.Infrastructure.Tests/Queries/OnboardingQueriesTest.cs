using Pi.BackofficeService.Application.Services.OnboardService;
using Pi.BackofficeService.Application.Services.UserService;
using Moq;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Application.Models.Customer;

namespace Pi.BackofficeService.Infrastructure.Tests.Queries
{
    public class OnboardingQueriesTest
    {
        private readonly Mock<IOnboardService> _onboardService;
        private readonly Mock<IUserService> _userService;

        private OpenAccountInfoDto _expectedResult1;
        private OpenAccountInfoDto _expectedResult2;

        public OnboardingQueriesTest()
        {
            _onboardService = new Mock<IOnboardService>();
            _userService = new Mock<IUserService>();

            _expectedResult1 = new OpenAccountInfoDto()
            {
                BpmReceived = true,
                CreatedDate = "23/Sept/2023",
                CustCode = "123",
                Id = "123",
                Status = "Pending",
                Identification = new Identification()
                {
                    CitizenId = "456",
                    DateOfBirth = "11/09/2023",
                    FirstNameEn = "John",
                    LastNameEn = "Smith",
                    FirstNameTh = "สมชาย",
                    LastNameTh = "ชัยดี",
                    LaserCode = "123",
                    Title = "Mr"
                },
                Documents = new List<Document>()
                {
                    new Document("Url 1", "Test1.jpg", "Passport"),
                    new Document("Url 2", "Test2.jpg", "Selfie"),
                }
            };

            _expectedResult2 = new OpenAccountInfoDto()
            {
                BpmReceived = true,
                CreatedDate = "23/Jan/2023",
                CustCode = "111",
                Id = "222",
                Status = "Submitted",
                Identification = new Identification()
                {
                    CitizenId = "333",
                    DateOfBirth = "11/09/2023",
                    FirstNameEn = "Ellen",
                    LastNameEn = "Bell",
                    FirstNameTh = "อาทิตย์",
                    LastNameTh = "น้อย",
                    LaserCode = "444",
                    Title = "Mr"
                },
                Documents = new List<Document>()
                {
                    new Document("Url 3", "Test3.jpg", "BankStatement"),
                    new Document("Url 4", "Test4.jpg", "Passport"),
                }
            };
        }

        [Fact]
        public async Task Should_Return_Accounts_When_Valid_Data_Is_Sent_Through_GetOpenAccountsPaginate()
        {
            var expectedPageNum = 2;
            var expectedPageSize = 20;
            var expectedPageTotal = 100;
            var expectedOrderBy = "custCode";
            var expectedOrderDir = "ASC";
            var filter = new OnboardingAccountListFilter(
                Guid.NewGuid(),
                "Pending",
                "456",
                "789",
                DateTime.Now,
                false
            );

            var expectedAccounts = new List<OpenAccountInfoDto>()
            {
                _expectedResult1,
                _expectedResult2
            };

            _onboardService.Setup(x => x.GetOpenAccounts(expectedPageNum, expectedPageSize, expectedOrderBy, expectedOrderDir, filter))
                .ReturnsAsync(new PaginateOpenAccountListResult(expectedAccounts, expectedPageNum, expectedPageSize, expectedPageTotal, expectedOrderBy, expectedOrderDir));

            var service = new OnboardingQueries(_onboardService.Object, _userService.Object);

            //ACT
            var response = await service.GetOpenAccountsPaginate(expectedPageNum, expectedPageSize, expectedOrderBy, expectedOrderDir, filter);

            // ASSERT
            _onboardService.Verify(p => p.GetOpenAccounts(expectedPageNum, expectedPageSize, expectedOrderBy, expectedOrderDir, filter), Times.Once());
            Assert.NotNull(response);
            Assert.NotNull(response.OpenAccountInfos);
            Assert.Equal(expectedAccounts.Count, response.OpenAccountInfos.Count);
        }


        [Fact]
        public async Task Should_Return_Accounts_When_Valid_CustCode_GetOpenAccountsByCustCode()
        {
            var userId = Guid.NewGuid();
            var custCode = "123";
            var filter = new OnboardingAccountListFilter(
                Guid.NewGuid(),
                null,
                null,
                null,
                null,
                false
            );

            var expectedAccounts = new List<OpenAccountInfoDto>()
            {
                _expectedResult1,
                _expectedResult2
            };


            _userService.Setup(x => x.GetUserByIdOrCustomerCode(custCode, true))
                .ReturnsAsync(new CustomerDto(userId.ToString(), new List<DeviceDto>(), new List<CustomerCodeDto>(), new List<TradingAccountDto>(), "", "", "", "", "", "", ""));

            _onboardService.Setup(x => x.GetOpenAccounts(userId.ToString()))
                .ReturnsAsync(expectedAccounts);

            var service = new OnboardingQueries(_onboardService.Object, _userService.Object);

            //ACT
            var response = await service.GetOpenAccountsByCustCode(custCode);

            // ASSERT
            _userService.Verify(p => p.GetUserByIdOrCustomerCode(custCode, true), Times.Once());
            _onboardService.Verify(p => p.GetOpenAccounts(userId.ToString()), Times.Once());

            Assert.NotNull(response);
            Assert.NotNull(response);
            Assert.Equal(expectedAccounts.Count, response.Count);
        }

        [Fact]
        public async Task Should_Return_Null_When_User_Not_Found_GetOpenAccountsByCustCode()
        {
            _userService.Setup(x => x.GetUserByIdOrCustomerCode(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((CustomerDto?)null);

            var service = new OnboardingQueries(_onboardService.Object, _userService.Object);

            //ACT
            var result = await service.GetOpenAccountsByCustCode("123");

            // ASSERT
            _userService.Verify(p => p.GetUserByIdOrCustomerCode("123", true), Times.Once());
            Assert.Null(result);
        }
    }
}
