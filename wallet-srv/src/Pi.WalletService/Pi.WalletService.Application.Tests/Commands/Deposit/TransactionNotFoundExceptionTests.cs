using Pi.WalletService.Application.Models;

namespace Pi.WalletService.Application.Tests.Commands.Deposit;

public class TransactionNotFoundExceptionTests
{
    private const string Message = "some message";

    [Fact]
    public void WhenMessageSpecified_ThenSetMessage()
    {
        var sut = new TransactionNotFoundException(Message);

        Assert.Equal(Message, sut.Message);
    }

    [Fact]
    public void WhenMessageAndInnerExSpecified_ThenSetMessageAndInnerEx()
    {
        var innerException = new Exception();

        var sut = new TransactionNotFoundException(Message, innerException);

        Assert.Equal(Message, sut.Message);
        Assert.Equal(innerException, sut.InnerException);
    }

    [Fact]
    public void WhenSerialized_ThenDeserializeCorrectly()
    {
        var sut = new TransactionNotFoundException();

        var result = Serializer.SerializeAndDeserialize(sut);

        Assert.Equal(result.ToString(), sut.ToString());
    }
}