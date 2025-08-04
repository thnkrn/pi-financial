using Moq;
using MQTTnet.Client;
using Pi.TfexService.Listener.Models;

namespace Pi.TfexService.Listener.Tests.Models;

public class MqttClientFactoryTests
{
    private readonly MqttClientFactory _mqttClientFactory = new();

    [Fact]
    public void Should_Return_Valid_Mqtt_Client()
    {
        // Act
        var client = _mqttClientFactory.CreateMqttClient();

        // Assert
        Assert.NotNull(client);
        Assert.IsType<MqttClient>(client);
    }

    [Fact]
    public void Should_Return_Valid_Subscribe_Options()
    {
        // Arrange
        var topics = new List<string> { "topic1", "topic2", "topic3" };

        // Act
        var options = _mqttClientFactory.CreateSubscribeOptions(topics);

        // Assert
        Assert.NotNull(options);
        Assert.NotEmpty(options.TopicFilters);
        foreach (var topic in topics)
        {
            Assert.Contains(options.TopicFilters, tf => tf.Topic == topic);
        }
    }

    [Fact]
    public void Should_Return_Valid_Mqtt_Client_Options()
    {
        // Arrange
        const string url = "wss://test.mqtt";

        // Act
        var options = _mqttClientFactory.CreateBrokerOptions(url);

        // Assert
        Assert.NotNull(options);
        Assert.Equal(url, options.ChannelOptions.ToString());
        Assert.True(options.CleanSession);
    }

    [Fact]
    public async Task Should_Call_Connect_Async()
    {
        // Arrange
        const string url = "wss://test.mqtt";
        var mockClient = new Mock<IMqttClient>();
        var options = _mqttClientFactory.CreateBrokerOptions(url);
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _mqttClientFactory.Connect(mockClient.Object, options, cancellationToken);

        // Assert
        mockClient.Verify(x => x.ConnectAsync(options, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Should_Call_Disconnect_Async()
    {
        // Arrange
        var mockClient = new Mock<IMqttClient>();
        var cancellationToken = new CancellationTokenSource().Token;
        var options = new MqttClientDisconnectOptions();

        // Arrange
        await _mqttClientFactory.Disconnect(mockClient.Object, options, cancellationToken);

        // Assert
        mockClient.Verify(x => x.DisconnectAsync(options, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Should_Call_Subscribe_Async()
    {
        // Arrange
        var mockClient = new Mock<IMqttClient>();
        var options = new MqttClientSubscribeOptions();
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _mqttClientFactory.Subscribe(mockClient.Object, options, cancellationToken);

        // Assert
        mockClient.Verify(x => x.SubscribeAsync(options, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Should_Call_Ping_Async()
    {
        // Arrange
        var mockClient = new Mock<IMqttClient>();
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _mqttClientFactory.Ping(mockClient.Object, cancellationToken);

        // Assert
        mockClient.Verify(x => x.PingAsync(cancellationToken), Times.Once);
    }
}