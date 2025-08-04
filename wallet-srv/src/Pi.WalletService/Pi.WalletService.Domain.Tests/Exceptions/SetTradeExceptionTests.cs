using Pi.WalletService.Domain.Exceptions;
namespace Pi.WalletService.Domain.Tests.Exceptions;

public class SetTradeExceptionTests
{
    private const string Message = "some message";

    [Fact]
    public void SetTradeDepositException_WhenMessageSpecified_ThenSetMessage()
    {
        var sut = new SetTradeDepositException(Message);

        Assert.Equal(Message, sut.Message);
    }

    [Fact]
    public void SetTradeDepositException_WhenMessageAndInnerExSpecified_ThenSetMessageAndInnerEx()
    {
        var innerException = new Exception();

        var sut = new SetTradeDepositException(Message, innerException);

        Assert.Equal(Message, sut.Message);
        Assert.Equal(innerException, sut.InnerException);
    }

    [Fact]
    public void SetTradeDepositException_WhenSerialized_ThenDeserializeCorrectly()
    {
        var sut = new SetTradeDepositException();

        var result = Serializer.SerializeAndDeserialize(sut);

        Assert.Equal(result.ToString(), sut.ToString());
    }

    [Fact]
    public void SetTradeWithdrawException_WhenMessageSpecified_ThenSetMessage()
    {
        var sut = new SetTradeWithdrawException(Message);

        Assert.Equal(Message, sut.Message);
    }

    [Fact]
    public void SetTradeWithdrawException_WhenMessageAndInnerExSpecified_ThenSetMessageAndInnerEx()
    {
        var innerException = new Exception();

        var sut = new SetTradeWithdrawException(Message, innerException);

        Assert.Equal(Message, sut.Message);
        Assert.Equal(innerException, sut.InnerException);
    }

    [Fact]
    public void SetTradeWithdrawException_WhenSerialized_ThenDeserializeCorrectly()
    {
        var sut = new SetTradeWithdrawException();

        var result = Serializer.SerializeAndDeserialize(sut);

        Assert.Equal(result.ToString(), sut.ToString());
    }

    [Fact]
    public void SetTradeAuthException_WhenMessageSpecified_ThenSetMessage()
    {
        var sut = new SetTradeAuthException(Message);

        Assert.Equal(Message, sut.Message);
    }

    [Fact]
    public void SetTradeAuthException_WhenMessageAndInnerExSpecified_ThenSetMessageAndInnerEx()
    {
        var innerException = new Exception();

        var sut = new SetTradeAuthException(Message, innerException);

        Assert.Equal(Message, sut.Message);
        Assert.Equal(innerException, sut.InnerException);
    }

    [Fact]
    public void SetTradeAuthException_WhenSerialized_ThenDeserializeCorrectly()
    {
        var sut = new SetTradeAuthException();

        var result = Serializer.SerializeAndDeserialize(sut);

        Assert.Equal(result.ToString(), sut.ToString());
    }

    [Fact]
    public void SetTradeRefreshTokenException_WhenMessageSpecified_ThenSetMessage()
    {
        var sut = new SetTradeRefreshTokenException(Message);

        Assert.Equal(Message, sut.Message);
    }

    [Fact]
    public void SetTradeRefreshTokenException_WhenMessageAndInnerExSpecified_ThenSetMessageAndInnerEx()
    {
        var innerException = new Exception();

        var sut = new SetTradeRefreshTokenException(Message, innerException);

        Assert.Equal(Message, sut.Message);
        Assert.Equal(innerException, sut.InnerException);
    }

    [Fact]
    public void SetTradeRefreshTokenException_WhenSerialized_ThenDeserializeCorrectly()
    {
        var sut = new SetTradeRefreshTokenException();

        var result = Serializer.SerializeAndDeserialize(sut);

        Assert.Equal(result.ToString(), sut.ToString());
    }

    //

    [Fact]
    public void SetTradeAccountInfoException_WhenMessageSpecified_ThenSetMessage()
    {
        var sut = new SetTradeAccountInfoException(Message);

        Assert.Equal(Message, sut.Message);
    }

    [Fact]
    public void SetTradeAccountInfoException_WhenMessageAndInnerExSpecified_ThenSetMessageAndInnerEx()
    {
        var innerException = new Exception();

        var sut = new SetTradeAccountInfoException(Message, innerException);

        Assert.Equal(Message, sut.Message);
        Assert.Equal(innerException, sut.InnerException);
    }

    [Fact]
    public void SetTradeAccountInfoException_WhenSerialized_ThenDeserializeCorrectly()
    {
        var sut = new SetTradeAccountInfoException();

        var result = Serializer.SerializeAndDeserialize(sut);

        Assert.Equal(result.ToString(), sut.ToString());
    }
}
