using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.SeedWork;

namespace Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;

public class ResponseCode : IAggregateRoot
{
    public ResponseCode()
    {
    }

    public Guid Id { get; set; }

    public Machine Machine { get; set; }

    public string State { get; set; } = string.Empty;
    public ProductType? ProductType { get; set; }

    public string? Suggestion { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsFilterable { get; set; }

    public ICollection<ResponseCodeAction>? Actions { get; set; }
}
