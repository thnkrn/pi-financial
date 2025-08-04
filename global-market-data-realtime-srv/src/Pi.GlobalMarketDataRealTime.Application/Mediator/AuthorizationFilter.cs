using MassTransit;

namespace Pi.GlobalMarketDataRealTime.Application.Mediator;

// ReSharper disable once UnusedType.Global
public class AuthorizationFilter<T> : IFilter<SendContext<T>>
    where T : class
{
    public void Probe(ProbeContext context)
    {
    }

    public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        await next.Send(context);
    }
}