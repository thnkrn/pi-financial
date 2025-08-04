using System;
using Pi.Common.SeedWork;

namespace Pi.WalletService.Domain.AggregatesModel.OnlineDirectDebitRegistrationAggregate
{
    public class OnlineDirectDebitRegistration : Entity<string>, IAggregateRoot
    {
        public OnlineDirectDebitRegistration(string id, Guid userId, string bank, DateTime createdAt) : base(id)
        {
            UserId = userId;
            Bank = bank;
            CreatedAt = createdAt;
        }

        public Guid UserId { get; private set; }
        public string Bank { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsSuccess { get; private set; }
        public string? ExternalStatusCode { get; private set; }

        public void Success()
        {
            this.IsSuccess = true;
        }

        public void Failed(string externalStatusCode)
        {
            this.IsSuccess = false;
            this.ExternalStatusCode = externalStatusCode;
        }
    }
}

