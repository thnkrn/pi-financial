using Pi.OnePort.TCP.Client;
using Pi.OnePort.TCP.Models;
using Pi.OnePort.TCP.Models.Packets;

namespace Pi.OnePort.TCP.Tests.Client;

public class ResponseMapTest
{
    [Theory]
    [InlineData("key1")]
    [InlineData("key2")]
    [InlineData("key3")]
    [InlineData("key4")]
    public void Should_ReturnTrue_And_Add_Null_When_AddKey(string key)
    {
        // Arrange
        var responseMap = new ResponseMap();

        // Act
        var actual = responseMap.AddKey(key);

        // Assert
        responseMap.TryGetValue(key, out var value);
        Assert.True(actual);
        Assert.Null(value);
    }
    [Theory]
    [InlineData("key1")]
    [InlineData("key2")]
    [InlineData("key3")]
    [InlineData("key4")]
    public void Should_ReturnFalse_When_AddKey_And_KeyDuplicated(string key)
    {
        // Arrange
        var responseMap = new ResponseMap();
        responseMap.AddKey(key);

        // Act
        var actual = responseMap.AddKey(key);

        // Assert
        responseMap.TryGetValue(key, out var value);
        Assert.False(actual);
        Assert.Null(value);
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("key2")]
    [InlineData("key3")]
    [InlineData("key4")]
    public void Should_ReturnTrue_And_UpdateValue_When_UpdateKey_And_KeyExist(string key)
    {
        // Arrange
        var packet = new Packet(new PacketTest("something"));
        var responseMap = new ResponseMap();
        responseMap.AddKey(key);

        // Act
        var actual = responseMap.UpdateKey(key, packet);

        // Assert
        responseMap.TryGetValue(key, out var value);
        Assert.True(actual);
        Assert.Equal(value, packet);
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("key2")]
    [InlineData("key3")]
    [InlineData("key4")]
    public void Should_ReturnFalse_When_UpdateKey_And_KeyNotExist(string key)
    {
        // Arrange
        var responseMap = new ResponseMap();

        // Act
        var actual = responseMap.UpdateKey(key, null);

        // Assert
        Assert.False(actual);
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("key2")]
    [InlineData("key3")]
    [InlineData("key4")]
    public void Should_ReturnTrue_And_Value_When_TryGetValue_With_ExistKey(string key)
    {
        // Arrange
        var packet = new Packet(new PacketTest("something"));
        var responseMap = new ResponseMap();
        responseMap.AddKey(key);
        responseMap.UpdateKey(key, packet);

        // Act
        var actual = responseMap.TryGetValue(key, out var value);

        // Assert
        Assert.True(actual);
        Assert.Equal(packet, value);
        Assert.True(responseMap.UpdateKey(key, null));
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("key2")]
    [InlineData("key3")]
    [InlineData("key4")]
    public void Should_ReturnFalse_And_Value_When_TryGetValue_Without_ExistKey(string key)
    {
        // Arrange
        var responseMap = new ResponseMap();

        // Act
        var actual = responseMap.TryGetValue(key, out var value);

        // Assert
        Assert.False(actual);
        Assert.Null(value);
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("key2")]
    [InlineData("key3")]
    [InlineData("key4")]
    public void Should_ReturnTrue_And_Value_When_TryGetValue_With_ExistKey_And_ValueIsNull(string key)
    {
        // Arrange
        var responseMap = new ResponseMap();
        responseMap.AddKey(key);

        // Act
        var actual = responseMap.TryGetValue(key, out var value);

        // Assert
        Assert.True(actual);
        Assert.Null(value);
        Assert.True(responseMap.UpdateKey(key, null));
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("key2")]
    [InlineData("key3")]
    [InlineData("key4")]
    public void Should_ReturnTrue_And_Value_When_TryRemove_With_ExistKey(string key)
    {
        // Arrange
        var packet = new Packet(new PacketTest("something"));
        var responseMap = new ResponseMap();
        responseMap.AddKey(key);
        responseMap.UpdateKey(key, packet);

        // Act
        var actual = responseMap.TryRemove(key, out var value);

        // Assert
        Assert.True(actual);
        Assert.Equal(packet, value);
        Assert.False(responseMap.UpdateKey(key, null));
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("key2")]
    [InlineData("key3")]
    [InlineData("key4")]
    public void Should_ReturnTrue_And_Value_When_TryRemove_With_ExistKey_And_ValueIsNull(string key)
    {
        // Arrange
        var responseMap = new ResponseMap();
        responseMap.AddKey(key);

        // Act
        var actual = responseMap.TryRemove(key, out var value);

        // Assert
        Assert.True(actual);
        Assert.Null(value);
        Assert.False(responseMap.UpdateKey(key, null));
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("key2")]
    [InlineData("key3")]
    [InlineData("key4")]
    public void Should_ReturnFalse_And_Value_When_TryRemove_Without_ExistKey(string key)
    {
        // Arrange
        var responseMap = new ResponseMap();

        // Act
        var actual = responseMap.TryRemove(key, out var value);

        // Assert
        Assert.False(actual);
        Assert.Null(value);
        Assert.False(responseMap.UpdateKey(key, null));
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("key2")]
    [InlineData("key3")]
    [InlineData("key4")]
    public async Task Should_ReturnPacket_When_GetAndRemoveWithWaitingAsync(string key)
    {
        // Arrange
        var packet = new Packet(new PacketTest("something"));
        var responseMap = new ResponseMap();
        responseMap.AddKey(key);
        responseMap.UpdateKey(key, packet);

        // Act
        var actual = await responseMap.GetAndRemoveWithWaitingAsync(key, CancellationToken.None);

        // Assert
        Assert.Equal(packet, actual);
        Assert.False(responseMap.UpdateKey(key, null));
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("key2")]
    [InlineData("key3")]
    [InlineData("key4")]
    public async Task Should_ReturnNull_When_GetAndRemoveWithWaitingAsync_And_CT_CancelRequested(string key)
    {
        // Arrange
        var packet = new Packet(new PacketTest("something"));
        var responseMap = new ResponseMap(100);
        responseMap.AddKey(key);
        responseMap.UpdateKey(key, packet);
        var ct = new CancellationToken(true);

        // Act
        var actual = await responseMap.GetAndRemoveWithWaitingAsync(key, ct);

        // Assert
        Assert.Null(actual);
        Assert.True(responseMap.UpdateKey(key, null));
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("key2")]
    [InlineData("key3")]
    [InlineData("key4")]
    public async Task Should_ThrowTimeOutException_When_GetAndRemoveWithWaitingAsync_Without_ExistKey(string key)
    {
        // Arrange
        var responseMap = new ResponseMap(100);

        // Act
        var act = async () =>
        {
            return await responseMap.GetAndRemoveWithWaitingAsync(key, CancellationToken.None);
        };

        // Assert
        await Assert.ThrowsAsync<TimeoutException>(act);
    }
}


