using System.ComponentModel;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public enum CustomerType
{
    [Description("Broker Client")]
    BrokerClient,
    [Description("Broker")]
    Broker,
    [Description("Broker Foreign")]
    BrokerForeign,
    [Description("Mutual Broker")]
    MutualBroker,
    [Description("Sub Broker Client")]
    SubBrokerClient,
    [Description("Sub Broker Portfolio")]
    SubBrokerPortfolio,
    [Description("Sub Broker Foreign")]
    SubBrokerForeign,
    [Description("Sub Broker Mutual Fund")]
    SubBrokerMutualFund
}