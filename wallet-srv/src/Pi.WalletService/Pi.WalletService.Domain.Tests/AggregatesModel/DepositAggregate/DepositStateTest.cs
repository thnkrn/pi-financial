using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;

namespace Pi.WalletService.Domain.Tests.AggregatesModel.DepositAggregate;

public class DepositStateTest
{
    [Theory]
    [InlineData("1234-1234-1234-1234", "1234123412341234")]
    [InlineData("1234 1234 1234-1234", "1234123412341234")]
    [InlineData(" 1234 1234 1234-1234 ", "1234123412341234")]
    public void Should_Be_NumberOnly_When_Set_BankAccountNo(string bankAccountNo, string expected)
    {
        // Act
        var deposit = new DepositState() { BankAccountNo = bankAccountNo };

        // Assert
        Assert.Equal(expected, deposit.BankAccountNo);
    }
}
