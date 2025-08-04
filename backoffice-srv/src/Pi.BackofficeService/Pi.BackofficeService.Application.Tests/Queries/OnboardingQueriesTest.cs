using Moq;
using Pi.BackofficeService.Application.Models.Customer;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Application.Services.OnboardService;
using Pi.BackofficeService.Application.Services.UserService;
using Pi.BackofficeService.Domain.AggregateModels.User;


namespace Pi.BackofficeService.Application.Tests.Queries
{
    public class OnboardingQueriesTest
    {
        private readonly Mock<IOnboardService> _onboardService;
        private readonly Mock<IUserService> _userService;

        private OpenAccountInfoDto _expectedResult1;
        private OpenAccountInfoDto _expectedResult2;

        private CustomerDto _expectedUser;

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
                CustCode = "123",
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
                    new Document("Url 4", "Test3.jpg", "Passport"),
                }
            };
            _expectedUser = new CustomerDto(
                "123",
                new List<DeviceDto>(),
                new List<CustomerCodeDto>(),
                new List<TradingAccountDto>(),
                "สมชาย", "ชัยดี", "John", "Smith", "0413111111", "", "test@email.com");
        }

        [Fact]
        public async Task Should_return_Correct_Page_Properties_When_Valid_Data_Is_Passed()
        {
            var expectedAccounts = new List<OpenAccountInfoDto>()
            {
                _expectedResult1,
                _expectedResult2
            };

            var expectedPage = 2;
            var expectedPageSize = 10;
            var expectedTotal = 100;
            var expectedOrderValue = "custCode";
            var expectedOrderDir = "ASC";

            _onboardService.Setup(x => x.GetOpenAccounts(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<OnboardingAccountListFilter>()))
            .ReturnsAsync(new PaginateOpenAccountListResult(expectedAccounts, expectedPage, expectedPageSize, expectedTotal, expectedOrderValue, expectedOrderDir));

            var service = new OnboardingQueries(_onboardService.Object, _userService.Object);

            //ACT
            var actual = await service.GetOpenAccountsPaginate(0, 0, "", "", new OnboardingAccountListFilter(null, null, null, null, null, null));

            // ASSERT
            _onboardService.Verify(p => p.GetOpenAccounts(0, 0, "", "", new OnboardingAccountListFilter(null, null, null, null, null, null)), Times.Once());

            Assert.Equal(expectedAccounts.Count, actual.OpenAccountInfos.Count);
            Assert.Equal(expectedPage, actual.Page);
            Assert.Equal(expectedPageSize, actual.PageSize);
            Assert.Equal(expectedTotal, actual.Total);
            Assert.Equal(expectedOrderValue, actual.OrderBy);
            Assert.Equal(expectedOrderDir, actual.OrderDir);
        }

        [Fact]
        public async Task Should_return_Correct_Account_Info_When_Service_Returned_1_account()
        {
            var expectedAccounts = new List<OpenAccountInfoDto>()
            {
                _expectedResult1
            };

            var expectedPage = 2;
            var expectedPageSize = 10;
            var expectedTotal = 100;
            var expectedOrderValue = "custCode";
            var expectedOrderDir = "ASC";

            _onboardService.Setup(x => x.GetOpenAccounts(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<OnboardingAccountListFilter>()))
            .ReturnsAsync(new PaginateOpenAccountListResult(expectedAccounts, expectedPage, expectedPageSize, expectedTotal, expectedOrderValue, expectedOrderDir));

            var service = new OnboardingQueries(_onboardService.Object, _userService.Object);

            //ACT
            var actual = await service.GetOpenAccountsPaginate(0, 0, "", "", new OnboardingAccountListFilter(null, null, null, null, null, null));

            // ASSERT
            _onboardService.Verify(p => p.GetOpenAccounts(0, 0, "", "", new OnboardingAccountListFilter(null, null, null, null, null, null)), Times.Once());

            Assert.Equal(expectedAccounts.Count, actual.OpenAccountInfos.Count);

            var actualAccount = actual.OpenAccountInfos[0];
            Assert.Equal(_expectedResult1, actualAccount);
        }

        [Fact]
        public async Task Should_Return_Correct_Account_Info_When_Service_Returned_2_accounts()
        {
            var expectedAccounts = new List<OpenAccountInfoDto>()
            {
                _expectedResult1,
                _expectedResult2
            };

            var expectedPage = 2;
            var expectedPageSize = 10;
            var expectedTotal = 100;
            var expectedOrderValue = "custCode";
            var expectedOrderDir = "ASC";

            _onboardService.Setup(x => x.GetOpenAccounts(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<OnboardingAccountListFilter>()))
            .ReturnsAsync(new PaginateOpenAccountListResult(expectedAccounts, expectedPage, expectedPageSize, expectedTotal, expectedOrderValue, expectedOrderDir));

            var service = new OnboardingQueries(_onboardService.Object, _userService.Object);

            //ACT
            var actual = await service.GetOpenAccountsPaginate(0, 0, "", "", new OnboardingAccountListFilter(null, null, null, null, null, null));

            // ASSERT
            _onboardService.Verify(p => p.GetOpenAccounts(0, 0, "", "", new OnboardingAccountListFilter(null, null, null, null, null, null)), Times.Once());

            Assert.Equal(expectedAccounts.Count, actual.OpenAccountInfos.Count);

            var actualAccount = actual.OpenAccountInfos[0];
            Assert.Equal(_expectedResult1, actualAccount);

            var actualAccount2 = actual.OpenAccountInfos[1];
            Assert.Equal(_expectedResult2, actualAccount2);
        }

        [Fact]
        public async Task Should_Return_AccountInfos_When_Valid_CustCode()
        {
            var expectedAccounts = new List<OpenAccountInfoDto>()
            {
                _expectedResult1,
                _expectedResult2
            };

            var userId = _expectedUser.Id;
            _userService.Setup(x => x.GetUserByIdOrCustomerCode(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(_expectedUser);

            _onboardService.Setup(x => x.GetOpenAccounts(userId.ToString())
            ).ReturnsAsync(expectedAccounts);

            var service = new OnboardingQueries(_onboardService.Object, _userService.Object);

            //ACT
            var response = await service.GetOpenAccountsByCustCode("123");

            // ASSERT
            _userService.Verify(p => p.GetUserByIdOrCustomerCode("123", true), Times.Once());
            _onboardService.Verify(p => p.GetOpenAccounts(userId.ToString()), Times.Once());

            Assert.NotNull(response);
            Assert.Equal(expectedAccounts.Count, response.Count);

            var actualAccount = response.FirstOrDefault(x => x.Id == _expectedResult1.Id);
            Assert.NotNull(actualAccount);
            Assert.Equal(_expectedResult1.Id, actualAccount.Id);
            Assert.Equal(_expectedResult1.Status, actualAccount.Status);
            Assert.Equal(_expectedResult1.BpmReceived, actualAccount.BpmReceived);
            Assert.Equal(_expectedResult1.CreatedDate, actualAccount.CreatedDate);
            Assert.Equal(_expectedResult1.CustCode, actualAccount.CustCode);

            var actualAccount2 = response.FirstOrDefault(x => x.Id == _expectedResult2.Id);
            Assert.NotNull(actualAccount2);
            Assert.Equal(_expectedResult2.Id, actualAccount2.Id);
            Assert.Equal(_expectedResult2.Status, actualAccount2.Status);
            Assert.Equal(_expectedResult2.BpmReceived, actualAccount2.BpmReceived);
            Assert.Equal(_expectedResult2.CreatedDate, actualAccount2.CreatedDate);
            Assert.Equal(_expectedResult2.CustCode, actualAccount2.CustCode);
        }

        [Fact]
        public async Task Should_Return_Null_When_User_Not_Found()
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
