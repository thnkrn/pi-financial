using MassTransit;
using MassTransit.Contracts;
using MassTransit.Events;
using MassTransit.Metadata;
using MassTransit.Saga;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Pi.BackofficeService.Application.Tests;

public class ConsumerTest : IAsyncLifetime
{
    protected ITestHarness Harness { get; private set; } = null!;
    protected ServiceProvider Provider { get; init; } = null!;

    public async Task InitializeAsync()
    {
        Harness = Provider.GetRequiredService<ITestHarness>();
        await Harness.Start();
    }

    public async Task DisposeAsync()
    {
        await Harness.Stop();
        await Provider.DisposeAsync();
    }

    protected async Task MockSagaResponse<TRequest, TResponse>(TResponse responseMessage) where TRequest : class
    {
        var context = Harness.Published.Select<TRequest>().First().Context;
        var responseEndpoint = await Harness.Bus.GetSendEndpoint(context.ResponseAddress!);
        await responseEndpoint.Send(responseMessage!, callback: ctx => ctx.RequestId = context.RequestId);
    }

    protected void MockSagaInstance<T>(T saga) where T : class, ISaga
    {
        var container = Provider.GetRequiredService<IndexedSagaDictionary<T>>();
        container.Add(new SagaInstance<T>(saga));
    }

    protected static Fault<T> NewFaultEvent<T>(T message)
    {
        return new FaultEvent<T>(message, Guid.NewGuid(), new BusHostInfo(false),
            new List<ExceptionInfo>
            {
                new FaultExceptionInfo(new Exception("Error"))
            },
            new[] { "string" });
    }

    protected async Task MockTimeoutResponse<T>(Guid sagaId) where T : class
    {
        var context = Harness.Published.Select<T>().First().Context;
        var timeoutResponse = new TimeoutExpired<T>(DateTime.Today, DateTime.Now, sagaId,
            (Guid)context.RequestId!, context.Message);
        var responseEndpoint = await Harness.Bus.GetSendEndpoint(context.ResponseAddress!);
        await responseEndpoint.Send(timeoutResponse!, callback: ctx => ctx.RequestId = context.RequestId);
    }
}

class TimeoutExpired<T> :
    RequestTimeoutExpired<T>
    where T : class
{
    public TimeoutExpired(DateTime timestamp, DateTime expirationTime, Guid correlationId, Guid requestId, T message)
    {
        Timestamp = timestamp;
        ExpirationTime = expirationTime;
        CorrelationId = correlationId;
        RequestId = requestId;
        Message = message;
    }

    public DateTime Timestamp { get; }

    public DateTime ExpirationTime { get; }

    public Guid CorrelationId { get; }

    public Guid RequestId { get; set; }

    public T Message { get; }
}
