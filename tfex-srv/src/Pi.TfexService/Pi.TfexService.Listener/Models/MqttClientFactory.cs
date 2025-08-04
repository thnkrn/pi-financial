using MQTTnet;
using MQTTnet.Client;

namespace Pi.TfexService.Listener.Models;

public interface IMqttClientFactory
{
    IMqttClient CreateMqttClient();
    MqttClientSubscribeOptions CreateSubscribeOptions(IEnumerable<string> topics);
    MqttClientOptions CreateBrokerOptions(string url);
    Task Connect(IMqttClient client, MqttClientOptions options, CancellationToken cancellationToken);
    Task Disconnect(IMqttClient client, MqttClientDisconnectOptions options, CancellationToken cancellationToken);
    Task Subscribe(IMqttClient client, MqttClientSubscribeOptions options, CancellationToken cancellationToken);
    Task Ping(IMqttClient client, CancellationToken cancellationToken);
}

public class MqttClientFactory : IMqttClientFactory
{
    public IMqttClient CreateMqttClient()
    {
        var mqttFactory = new MqttFactory();
        return mqttFactory.CreateMqttClient();
    }

    public MqttClientSubscribeOptions CreateSubscribeOptions(IEnumerable<string> topics)
    {
        var mqttFactory = new MqttFactory();
        var subscribeOptionsBuilder = mqttFactory.CreateSubscribeOptionsBuilder();

        foreach (var topic in topics)
        {
            subscribeOptionsBuilder.WithTopicFilter(f => f.WithTopic(topic));
        }

        return subscribeOptionsBuilder.Build();
    }

    public MqttClientOptions CreateBrokerOptions(string url)
    {
        return new MqttClientOptionsBuilder()
            .WithWebSocketServer(configure: builder => builder.WithUri(url))
            .WithCleanSession()
            .Build();
    }

    public async Task Connect(IMqttClient client, MqttClientOptions options, CancellationToken cancellationToken)
    {
        await client.ConnectAsync(options, cancellationToken);
    }

    public async Task Disconnect(IMqttClient client, MqttClientDisconnectOptions options, CancellationToken cancellationToken)
    {
        await client.DisconnectAsync(options, cancellationToken);
    }

    public async Task Subscribe(IMqttClient client, MqttClientSubscribeOptions options, CancellationToken cancellationToken)
    {
        await client.SubscribeAsync(options, cancellationToken);
    }

    public async Task Ping(IMqttClient client, CancellationToken cancellationToken)
    {
        await client.PingAsync(cancellationToken);
    }
}