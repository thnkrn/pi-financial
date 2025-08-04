using MassTransit;
using MassTransit.Contracts;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;

namespace Pi.WalletService.Application.Utilities;

public static class MassTransitUtils
{
    public static EventActivityBinder<TInstance, TData> SendMetric<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source,
        string name,
        Func<BehaviorContext<TInstance>, MetricTags>? tagSelector
    )
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        return source.Publish(
            ctx => new SendMetric(name, 1, tagSelector!(ctx))
        );
    }

    public static EventActivityBinder<TInstance, TData> SendMetric<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source,
        string name,
        Func<BehaviorContext<TInstance>, double> valueSelector,
        Func<BehaviorContext<TInstance>, MetricTags>? tagSelector
    )
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        return source.Publish(
            ctx => new SendMetric(name, valueSelector(ctx), tagSelector!(ctx))
        );
    }
}