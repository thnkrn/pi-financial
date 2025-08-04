using MassTransit;

namespace Pi.Financial.FundService.Domain.Events
{
    public record CustomerDataSynced(string CustomerCode, Guid CorrelationId) : CorrelatedBy<Guid>;
}
