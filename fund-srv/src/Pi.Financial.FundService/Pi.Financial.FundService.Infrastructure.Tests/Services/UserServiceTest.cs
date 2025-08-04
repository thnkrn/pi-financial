using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Client.UserService.Api;
using Pi.Client.UserService.Model;
using Pi.Client.UserSrvV2.Model;
using Pi.Common.Features;
using Pi.Financial.FundService.Application.Models.Bank;
using Pi.Financial.FundService.Infrastructure.Services;
using UserV2ApiException = Pi.Client.UserSrvV2.Client.ApiException;

namespace Pi.Financial.FundService.Infrastructure.Tests.Services;

public class UserServiceTest
{
    private readonly Mock<IUserApi> _mockUserApi = new();
    private readonly Mock<IDocumentApi> _mockDocumentApi = new();
    private readonly Mock<IUserMigrationApi> _mockUserMigrationApi = new();
    private readonly Mock<ILogger<UserService>> _mockLogger = new();
    private readonly UserService _userService;
    private readonly Mock<IFeatureService> _mockFeatureService = new();
    private readonly Mock<Pi.Client.UserSrvV2.Api.IUserApi> _mockUserApi2 = new();
    private readonly Mock<Pi.Client.UserSrvV2.Api.IBankAccountApi> _mockBankAccountApi = new();

    public UserServiceTest()
    {
        _userService = new UserService(
                _mockUserApi.Object,
                _mockUserMigrationApi.Object,
                _mockLogger.Object,
                _mockDocumentApi.Object,
                _mockUserApi2.Object,
                _mockFeatureService.Object,
                _mockBankAccountApi.Object
                );
    }

    [Fact]
    async void GetDocumentByUserId_WhenUserHasMultipleDocuments_ThenReturnDocumentDataSuccessfully()
    {
        //Arrange
        var mockData1 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl",
            "fileName",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.CitizenCard);
        var mockData2 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl",
            "fileName",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.Signature);

        List<PiUserApplicationModelsDocumentDocumentDto> expected = new() { mockData1, mockData2 };

        _mockDocumentApi
            .Setup(q => q.GetDocumentsByUserIdAsync(It.IsAny<string>(), default))
            .ReturnsAsync(new PiUserApplicationModelsDocumentDocumentDtoListApiResponse(expected));
        //Act
        var result = await _userService.GetDocumentByUserId(Guid.NewGuid());

        //Assert
        IEnumerable<PiUserApplicationModelsDocumentDocumentDto> actual = result as PiUserApplicationModelsDocumentDocumentDto[] ?? result.ToArray();

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    async void GetDocumentByUserId_WhenUserDoesNotHaveAnyDocument_ThenReturnEmpty()
    {
        //Arrange
        List<PiUserApplicationModelsDocumentDocumentDto> expected = [];

        _mockDocumentApi
            .Setup(q => q.GetDocumentsByUserIdAsync(It.IsAny<string>(), default))
            .Throws(new Exception("no data"));
        //Act
        var result = await _userService.GetDocumentByUserId(Guid.NewGuid());

        //Assert
        IEnumerable<PiUserApplicationModelsDocumentDocumentDto> actual = result as PiUserApplicationModelsDocumentDocumentDto[] ?? result.ToArray();

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    private async void GetATSBankAccountsByCustomerCodeAsync_ShouldReturnATSBankAccounts()
    {
        const string customerCode = "ABC123";

        // Setup
        var mockData = new DtoDepositWithdrawBankAccountResponse(
            "bankAccountNo", "0000", "002", "bankLogo", "bankName", "shortname", "id", "payment",
            "paymentExpiry");
        var mockResponse = new InternalV2BankAccountDepositWithdrawGet200Response(data: [mockData]);
        _mockBankAccountApi.Setup(api =>
                api.InternalV2BankAccountDepositWithdrawGetAsync(customerCode, It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _userService.GetAtsBankAccountsByCustomerCodeAsync(customerCode);

        // Assert
        result.Should().BeEquivalentTo(new AtsBankAccounts(
            RedemptionBankAccounts: [new BankAccount(mockData.BankCode, mockData.BankAccountNo, "0001", true, mockData.PaymentToken)],
            SubscriptionBankAccounts: [new BankAccount(mockData.BankCode, mockData.BankAccountNo, "0001", true, mockData.PaymentToken)]
            ));
        _mockBankAccountApi.Verify(api => api.InternalV2BankAccountDepositWithdrawGetAsync(customerCode, It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    private async void GetATSBankAccountsByCustomerCodeAsync_ShouldReturnNull_WhenBankAccountNotFound()
    {
        const string customerCode = "ABC123";

        // Setup
        _mockBankAccountApi.Setup(api =>
                api.InternalV2BankAccountDepositWithdrawGetAsync(customerCode, It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserV2ApiException(400, "mock"));

        // Act
        var result = await _userService.GetAtsBankAccountsByCustomerCodeAsync(customerCode);

        // Assert
        result.Should().BeEquivalentTo(new AtsBankAccounts(
            RedemptionBankAccounts: [],
            SubscriptionBankAccounts: []
        ));
        _mockBankAccountApi.Verify(api => api.InternalV2BankAccountDepositWithdrawGetAsync(customerCode, It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
