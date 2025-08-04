using MassTransit;
using Pi.Common.SeedWork;

namespace Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate
{
    // https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/net-core-microservice-domain-model
    // https://masstransit-project.com/usage/sagas/automatonymous.html#state-machine
    public class FundAccountOpeningState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public Guid UserId { get; set; }
        public string? CustomerCode { get; set; }
        public bool IsOpenSegregateAccount { get; set; }
        public bool Ndid { get; set; }
        public DateTime? NdidDateTime { get; set; }
        public string? NdidRequestId { get; set; }
        public DateTime RequestReceivedTime { get; set; }
        public Guid? DocumentsGenerationTicketId { get; set; }
        public string? OpenAccountRegisterUid { get; set; }
        public long? CustomerId { get; set; }
        public string? CurrentState { get; set; }
        public string? FailedReason { get; set; }

        // If using Optimistic concurrency, this property is required
        public byte[]? RowVersion { get; set; }
    }
}
