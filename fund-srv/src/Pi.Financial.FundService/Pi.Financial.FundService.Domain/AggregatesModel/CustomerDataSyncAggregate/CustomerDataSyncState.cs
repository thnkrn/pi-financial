using Pi.Common.SeedWork;

namespace Pi.Financial.FundService.Domain.AggregatesModel.CustomerDataAggregate
{
    public class CustomerDataSyncHistory : Entity, IAuditableEntity, IAggregateRoot
    {
        public CustomerDataSyncHistory(Guid correlationId, string customerCode)
        {
            CorrelationId = correlationId;
            CustomerCode = customerCode;
        }

        public Guid CorrelationId { get; private set; }
        public string CustomerCode { get; private set; }
        public bool? ProfileUpdateSuccess { get; private set; }
        public bool? AccountUpdateSuccess { get; private set; }
        public string? FailedReason { get; private set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public void MarkAllAsFailed(string failedReason)
        {
            ProfileUpdateSuccess = false;
            AccountUpdateSuccess = false;
            FailedReason = failedReason;
        }

        public void MarkProfileUpdateAsSuccess()
        {
            ProfileUpdateSuccess = true;
        }

        public void MarkProfileUpdateAsFailed(string failedReason)
        {
            ProfileUpdateSuccess = false;
            FailedReason = failedReason;
        }

        public void MarkAccountUpdateAsSuccess()
        {
            AccountUpdateSuccess = true;
        }

        public void MarkAccountUpdateAsFailed(string failedReason)
        {
            AccountUpdateSuccess = false;
            FailedReason = failedReason;
        }

        public bool IsAllSuccess
        {
            get
            {
                return ProfileUpdateSuccess == true && AccountUpdateSuccess == true;
            }
        }
    }
}
