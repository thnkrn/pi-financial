using Pi.BackofficeService.Domain.Exceptions;

namespace Pi.BackofficeService.Domain.Tests.Exceptions;

public class TicketExecuteExceptionTest
{
    private const string Message = "some message";

    [Fact]
    public void WhenMessageSpecified_ThenSetMessage()
    {
        var sut = new TicketExecuteException(Message);

        Assert.Equal(Message, sut.Message);
    }

    [Fact]
    public void WhenMessageAndInnerExSpecified_ThenSetMessageAndInnerEx()
    {
        var innerException = new Exception();

        var sut = new TicketExecuteException(Message, innerException);

        Assert.Equal(Message, sut.Message);
        Assert.Equal(innerException, sut.InnerException);
    }

    [Fact]
    public void WhenSerialized_ThenDeserializeCorrectly()
    {
        var sut = new TicketExecuteException();

        var result = Serializer.SerializeAndDeserialize(sut);

        Assert.Equal(result.ToString(), sut.ToString());
    }
}
