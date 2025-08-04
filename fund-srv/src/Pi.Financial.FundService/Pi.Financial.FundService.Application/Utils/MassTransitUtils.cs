using MassTransit;
using Pi.Financial.FundService.Application.Commands;

namespace Pi.Financial.FundService.Application.Utils;

public static class MassTransitUtils
{
    public static EventActivityBinder<TInstance, TData> SendMetric<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source,
        string name,
        Func<BehaviorContext<TInstance>, KeyValuePair<string, object?>[]>? tagSelector = null
    )
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        return source.Publish(
            ctx => new SendMetric(name, 1, tagSelector?.Invoke(ctx))
        );
    }

    public static EventActivityBinder<TInstance, TData> SendMetric<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source,
        string name,
        Func<BehaviorContext<TInstance>, double> valueSelector,
        Func<BehaviorContext<TInstance>, KeyValuePair<string, object?>[]>? tagSelector = null
    )
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        return source.Publish(
            ctx => new SendMetric(name, valueSelector(ctx), tagSelector?.Invoke(ctx))
        );
    }
}
