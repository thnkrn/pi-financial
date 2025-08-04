using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
namespace Pi.WalletService.Application.Tests.Commands.GlobalWalletTransfer;

public class UnableToConfirmFxExceptionTests
{
    private const string Message = "some message";

    [Fact]
    public void WhenMessageSpecified_ThenSetMessage()
    {
        var sut = new UnableToConfirmFxException(Message);

        Assert.Equal(Message, sut.Message);
    }

    [Fact]
    public void WhenMessageAndInnerExSpecified_ThenSetMessageAndInnerEx()
    {
        var innerException = new Exception();

        var sut = new UnableToConfirmFxException(Message, innerException);

        Assert.Equal(Message, sut.Message);
        Assert.Equal(innerException, sut.InnerException);
    }

    [Fact]
    public void WhenSerialized_ThenDeserializeCorrectly()
    {
        var sut = new UnableToConfirmFxException();

        var result = Serializer.SerializeAndDeserialize(sut);

        Assert.Equal(result.ToString(), sut.ToString());
    }
}