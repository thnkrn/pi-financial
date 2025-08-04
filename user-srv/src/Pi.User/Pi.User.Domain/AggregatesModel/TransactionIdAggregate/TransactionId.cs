using System.Globalization;
using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.TransactionIdAggregate
{
    public class TransactionId : Entity, IAggregateRoot
    {
        public TransactionId(string prefix, DateOnly date, string referId, string customerCode, int sequence)
        {
            Prefix = prefix;
            Date = date;
            ReferId = referId;
            CustomerCode = customerCode;
            Sequence = sequence;
        }

        public string Prefix { get; }
        public DateOnly Date { get; }
        public string ReferId { get; }
        public string CustomerCode { get; private set; }
        public int Sequence { get; private set; }

        public TransactionId Next()
        {
            this.Sequence++;
            return this;
        }

        public override string ToString()
        {
            return $"{Prefix}{Date.ToString("yyyyMMdd", CultureInfo.InvariantCulture)}{Sequence:D5}";
        }
    }
}

