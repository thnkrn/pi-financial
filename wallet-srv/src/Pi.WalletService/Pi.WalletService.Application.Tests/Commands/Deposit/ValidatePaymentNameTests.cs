using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.Domain.Exceptions;
namespace Pi.WalletService.Application.Tests.Commands.Deposit;

public class ValidatePaymentNameTests : ConsumerTest
{
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Mock<IDepositEntrypointRepository> _depositEntrypointRepository;

    public ValidatePaymentNameTests()
    {
        _onboardService = new Mock<IOnboardService>();
        _depositEntrypointRepository = new Mock<IDepositEntrypointRepository>();
        var withdrawEntrypointRepository = new Mock<IWithdrawEntrypointRepository>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<ValidatePaymentNameConsumer>(); })
            .AddScoped<IDepositEntrypointRepository>(_ => _depositEntrypointRepository.Object)
            .AddScoped<IWithdrawEntrypointRepository>(_ => withdrawEntrypointRepository.Object)
            .AddScoped<IOnboardService>(_ => _onboardService.Object)
            .BuildServiceProvider(true);
    }

    [Theory]
    [InlineData("เอ", "นางสาวบี", "ด.ญ.เอ นางสาวบี")]
    [InlineData("เอ", "นางสาวบี", "นายนางสาวบี เอ")]
    [InlineData("นางสาวหนู", "กันยา", "น.ส. นางสาวหนู กันยา")]
    [InlineData("ว่าที่ ร.ต. เอก", "ภพ", "ว่าที่ ร.ต. เอก ภพ")]
    public async void Should_Be_Able_To_Validate_Thai_Name(string firstname, string lastname, string bankAccountName)
    {
        // Arrange
        _onboardService
            .Setup(u => u.GetCustomerInfo(It.IsAny<string>()))
            .ReturnsAsync(() => new CustomerInfo(
                "",
                firstname,
                lastname,
                "",
                "",
                ""
            ));

        var client = Harness.GetRequestClient<ValidatePaymentName>();
        var payload = new ValidatePaymentName("123456", bankAccountName);

        // Act
        var response = await client.GetResponse<DepositValidatePaymentNameSucceed>(payload);

        // Assert
        Assert.True(await Harness.Consumed.Any<DepositValidatePaymentNameSucceed>());
        Assert.Equal($"{firstname} {lastname}", response.Message.CustomerThaiName);
    }

    [Theory]
    [InlineData("นาย หนู", "กันยา", "หนู ยา")]
    [InlineData("นางสาวหนู", "กันยา", "นายหนู ยา")]
    [InlineData("ว่าที่ ร.ต. เอก", "ภพ", "ว่าที่ ร.อ. เอก ภพ")]
    public async void Should_Be_Throw_Name_Mismatch_Exception_When_Validate_Thai_Name(string firstname, string lastname, string bankAccountName)
    {
        // Arrange
        _onboardService
            .Setup(u => u.GetCustomerInfo(It.IsAny<string>()))
            .ReturnsAsync(() => new CustomerInfo(
                "",
                firstname,
                lastname,
                "",
                "",
                ""
            ));

        var client = Harness.GetRequestClient<ValidatePaymentName>();
        var payload = new ValidatePaymentName("123456", bankAccountName);

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () => await client.GetResponse<DepositValidatePaymentNameSucceed>(payload));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(CustomerNameAndBankAccountNameMismatchException).ToString())));
    }

    [Theory]
    [InlineData("John", "Doh", "John Doh")]
    [InlineData("John", "Doh", "MR.John Doh")]
    [InlineData("John", "Doh", "Mr. Doh John")]
    [InlineData("John", "Doh", "MissDoh John")]
    [InlineData("John F.", "Doh", "Mr. John F. Doh")]
    [InlineData("Mr. John", "Doh", "John Doh")]
    [InlineData("Mr. John", "Doh", "MrJohn Doh")]
    [InlineData("Miss John", "Doh", "Mr. Doh Miss-John")]
    [InlineData("ACT LT John", "Doh", "ACT LT John Doh")]
    public async void Should_Be_Able_To_Validate_English_Name(string firstname, string lastname, string bankAccountName)
    {
        // Arrange
        _onboardService
            .Setup(u => u.GetCustomerInfo(It.IsAny<string>()))
            .ReturnsAsync(() => new CustomerInfo(
                "",
                "",
                "",
                firstname,
                lastname,
                ""
            ));

        var client = Harness.GetRequestClient<ValidatePaymentName>();
        var payload = new ValidatePaymentName("123456", bankAccountName);

        // Act
        await client.GetResponse<DepositValidatePaymentNameSucceed>(payload);

        // Assert
        Assert.True(await Harness.Consumed.Any<DepositValidatePaymentNameSucceed>()); ;
    }

    [Theory]
    [InlineData("Pita", "Lim", "Pita Lam")]
    [InlineData("MrPita", "Lim", "MissPita Lim")]
    [InlineData("ACT CAPT Prayuth", "Chan-o-cha", "CAPT Prayuth Chan-o-cha")]
    public async void Should_Be_Throw_Name_Mismatch_Exception_When_Validate_English_Name(string firstname, string lastname, string bankAccountName)
    {
        // Arrange
        _onboardService
            .Setup(u => u.GetCustomerInfo(It.IsAny<string>()))
            .ReturnsAsync(() => new CustomerInfo(
                "",
                firstname,
                lastname,
                firstname,
                lastname,
                ""
            ));

        var client = Harness.GetRequestClient<ValidatePaymentName>();
        var payload = new ValidatePaymentName("123456", bankAccountName);

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () => await client.GetResponse<DepositValidatePaymentNameSucceed>(payload));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(CustomerNameAndBankAccountNameMismatchException).ToString())));
    }

    [Theory]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "นาย สมชาย มั่งมี")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "นายสมชาย มั่งมี")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "นายสมชายมั่งมี")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "สมชาย มั่งมี")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "สมชายมั่งมี")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "นาย มั่งมี สมชาย")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "นายมั่งมี สมชาย")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "นายมั่งมีสมชาย")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "มั่งมี สมชาย")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "มั่งมีสมชาย")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "MR. Somchai Mungmee")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "MR.Somchai Mungmee")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "MR.SomchaiMungmee")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "Somchai Mungmee")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "SomchaiMungmee")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "MR. Mungmee Somchai")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "MR.Mungmee Somchai")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "MR.MungmeeSomchai")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "Mungmee Somchai")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "MungmeeSomchai")]
    public async void Should_Be_Able_To_Validate_Both_Eng_And_Thai_For_Parentheses_Format(
        string firstnameTh, string lastnameTh, string firstnameEn, string lastnameEn, string bankAccountName)
    {
        // Arrange
        _onboardService
            .Setup(u => u.GetCustomerInfo(It.IsAny<string>()))
            .ReturnsAsync(() => new CustomerInfo(
                "",
                firstnameTh,
                lastnameTh,
                firstnameEn,
                lastnameEn,
                ""
            ));

        _depositEntrypointRepository
            .Setup(u => u.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(() => new DepositEntrypointState()
            {
                BankAccountName = bankAccountName,
                CustomerCode = "123456"
            });

        var client = Harness.GetRequestClient<ValidatePaymentNameV2>();
        var payload = new ValidatePaymentNameV2(Guid.NewGuid());

        // Act
        await client.GetResponse<DepositValidatePaymentNameSucceed>(payload);

        // Assert
        Assert.True(await Harness.Consumed.Any<DepositValidatePaymentNameSucceed>()); ;
    }

    [Theory]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "สมชายย มั่งมี")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "สมชาย มั่งมีม")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "สมชายมั่งมีม")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "นายสมชายย มั่งมี")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "นายสมชาย มั่งมีม")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "นายสมชายมั่งมีม")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "มั่งมี สมชายย")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "มั่งมีม สมชาย")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "มั่งมีมสมชาย")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "นายมั่งมี สมชายย")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "นายมั่งมีม สมชาย")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "นายมั่งมีมสมชาย")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "Somchaii Mungmee")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "Somchai Mungmeee")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "SomchaiMungmeee")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "Mr.Somchaii Mungmee")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "Mr.Somchai Mungmeee")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "Mr.SomchaiMungmeee")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "Mungmee Somchaii")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "Mungmeee Somchai")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "MungmeeeSomchai")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "Mr.Mungmee Somchaii")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "Mr.Mungmeee Somchai")]
    [InlineData("สมชาย", "มั่งมี", "Somchai", "Mungmee", "Mr.MungmeeeSomchai")]
    public async void Should_Be_Throw_Name_Mismatch_Exception_When_Validate_Both_Eng_And_Thai_For_Parentheses_Format(
        string firstnameTh, string lastnameTh, string firstnameEn, string lastnameEn, string bankAccountName)
    {
        // Arrange
        _onboardService
            .Setup(u => u.GetCustomerInfo(It.IsAny<string>()))
            .ReturnsAsync(() => new CustomerInfo(
                "",
                firstnameTh,
                lastnameTh,
                firstnameEn,
                lastnameEn,
                ""
            ));

        _depositEntrypointRepository
            .Setup(u => u.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(() => new DepositEntrypointState()
            {
                BankAccountName = bankAccountName,
                CustomerCode = "123456"
            });

        var client = Harness.GetRequestClient<ValidatePaymentNameV2>();
        var payload = new ValidatePaymentNameV2(Guid.NewGuid());

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () => await client.GetResponse<DepositValidatePaymentNameSucceed>(payload));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(CustomerNameAndBankAccountNameMismatchException).ToString())));
    }

    [Fact]
    public async void Should_Be_Throw_Exception_When_Invalid_Request_ID()
    {
        // Arrange
        var client = Harness.GetRequestClient<ValidatePaymentNameV2>();
        var payload = new ValidatePaymentNameV2(Guid.NewGuid());

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () => await client.GetResponse<DepositValidatePaymentNameSucceed>(payload));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(Exception).ToString())));
    }
}
