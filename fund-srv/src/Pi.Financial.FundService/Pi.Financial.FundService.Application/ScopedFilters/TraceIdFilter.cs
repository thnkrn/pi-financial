using System.Diagnostics;
using MassTransit;

namespace Pi.Financial.FundService.Application.ScopedFilters;

public class TraceIdFilter<T> : IFilter<SendContext<T>>,
    IFilter<PublishContext<T>>,
    IFilter<ConsumeContext<T>>,
    IFilter<ExecuteContext<T>> where T : class
{
    public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        var currentActivity = Activity.Current;
        if (currentActivity != null)
        {
            context.Headers.Set("X-Trace-Id", currentActivity.TraceId.ToString());
        }

        await next.Send(context);
    }

    public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {

        var currentActivity = Activity.Current;
        if (currentActivity != null)
        {
            context.Headers.Set("X-Trace-Id", currentActivity.TraceId.ToString());
        }

        await next.Send(context);
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var traceId = context.Headers.Get<string>("X-Trace-Id");
        if (traceId != null)
        {
            Activity.Current?.SetParentId(ActivityTraceId.CreateFromString(traceId.AsSpan()), default, ActivityTraceFlags.Recorded);
        }

        await next.Send(context);
    }

    public async Task Send(ExecuteContext<T> context, IPipe<ExecuteContext<T>> next)
    {

        var traceId = context.Headers.Get<string>("X-Trace-Id");
        if (traceId != null)
        {
            Activity.Current?.SetParentId(ActivityTraceId.CreateFromString(traceId.AsSpan()), default, ActivityTraceFlags.Recorded);
        }

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
    }
}
