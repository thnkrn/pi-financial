using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.SeedWork;
using Pi.User.Application.Commands;
using Pi.User.Application.Services.Cryptography;
using Pi.User.Domain.AggregatesModel.BankAccountAggregate;

namespace Pi.User.Application.Tests.Commands;

public class SubmitBankAccountConsumerTests : ConsumerTest
{
    private readonly Mock<ILogger<SubmitBankAccountConsumer>> _loggerMock = new();
    private readonly Mock<IBankAccountRepository> _bankAccountRepositoryMock = new();
    private readonly Mock<ICryptographyService> _cryptographyServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public SubmitBankAccountConsumerTests()
    {
        _bankAccountRepositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<SubmitBankAccountConsumer>(); })
            .AddScoped(_ => _loggerMock.Object)
            .AddScoped(_ => _bankAccountRepositoryMock.Object)
            .AddScoped(_ => _cryptographyServiceMock.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async Task Consume_ValidMessage_AddsBankAccount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bankAccountNo = "1234567890";
        var bankAccountName = "Test Account";
        var bankCode = "001";
        var bankBranchCode = "0001";
        var fileUrl = "http://example.com/file.pdf";
        var fileName = "file.pdf";

        var request = new SubmitBankAccountRequest(
            userId,
            bankAccountNo,
            bankAccountName,
            bankCode,
            bankBranchCode,
            fileUrl,
            fileName
        );

        var bankAccount = new BankAccount(
            Guid.NewGuid(),
            userId,
            bankAccountNo,
            "hashedBankAccountNo",
            bankAccountName,
            "hashedBankAccountName",
            bankCode,
            bankBranchCode
        );

        _bankAccountRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((BankAccount)null!);
        _cryptographyServiceMock.Setup(s => s.Hash(It.IsAny<string>())).Returns("hashedValue");

        // Act
        await Harness.Bus.Publish(request);

        // Assert
        Assert.True(await Harness.Consumed.Any<SubmitBankAccountRequest>());
        _bankAccountRepositoryMock.Verify(r => r.AddAsync(It.Is<BankAccount>(b =>
            b.UserId == userId &&
            b.AccountNo == bankAccountNo &&
            b.AccountName == bankAccountName &&
            b.BankCode == bankCode &&
            b.BankBranchCode == bankBranchCode
        )), Times.Once);
    }

    [Fact]
    public async Task Consume_ExistingBankAccount_UpdatesBankAccount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bankAccountNo = "1234567890";
        var bankAccountName = "Updated Account";
        var bankCode = "001";
        var bankBranchCode = "0001";
        var fileUrl = "http://example.com/file.pdf";
        var fileName = "file.pdf";

        var request = new SubmitBankAccountRequest(
            userId,
            bankAccountNo,
            bankAccountName,
            bankCode,
            bankBranchCode,
            fileUrl,
            fileName
        );

        var existingBankAccount = new BankAccount(
            Guid.NewGuid(),
            userId,
            "0987654321",
            "hashedOldBankAccountNo",
            "Old Account",
            "hashedOldBankAccountName",
            bankCode,
            "0000"
        );

        _bankAccountRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(existingBankAccount);
        _cryptographyServiceMock.Setup(s => s.Hash(It.IsAny<string>())).Returns("hashedValue");

        // Act
        await Harness.Bus.Publish(request);

        // Assert
        Assert.True(await Harness.Consumed.Any<SubmitBankAccountRequest>());
        _bankAccountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<BankAccount>()), Times.Never);

        Assert.Equal(bankAccountNo, existingBankAccount.AccountNo);
        Assert.Equal(bankAccountName, existingBankAccount.AccountName);
        Assert.Equal(bankCode, existingBankAccount.BankCode);
        Assert.Equal(bankBranchCode, existingBankAccount.BankBranchCode);
    }
}
