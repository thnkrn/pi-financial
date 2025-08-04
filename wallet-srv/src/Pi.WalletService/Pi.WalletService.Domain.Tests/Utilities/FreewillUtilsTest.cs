using Pi.WalletService.Domain.Utilities;

namespace Pi.WalletService.Domain.Tests.Utilities;

public class FreewillUtilsTest
{
    [Theory]
    [InlineData("006", "Can not Approve to Front Office")]
    [InlineData("008", "Lock Table in Back Office")]
    [InlineData("023", "Deposit Withdraw Disabled")]
    [InlineData("900", "Connection Time Out")]
    [InlineData("906", "Internal Server Error")]
    [InlineData("000", "")]
    [InlineData("ABC", "")]
    [InlineData("E001", "")]
    [InlineData(null, "")]
    public void Should_Be_Return_Correct_Result_Code_Message22(string resultCode, string expected)
    {
        // Act
        var resultMessage = FreewillUtils.GetResultMessage(resultCode);

        // Assert
        Assert.Equal(expected, resultMessage);
    }
}
