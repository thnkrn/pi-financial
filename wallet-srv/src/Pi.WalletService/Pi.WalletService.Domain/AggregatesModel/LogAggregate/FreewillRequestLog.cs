using Pi.WalletService.Domain.SeedWork;

namespace Pi.WalletService.Domain.AggregatesModel.LogAggregate;

public class FreewillRequestLog : BaseEntity
{
    public FreewillRequestLog(string referId, string? transId, string request, string? response, FreewillRequestType type, string? callback = null)
    {
        Id = Guid.NewGuid();
        ReferId = referId;
        TransId = transId;
        Request = request;
        Response = response;
        Type = type;
        Callback = callback;
    }

    public Guid Id { get; private set; }

    public string ReferId { get; private set; }
    public string? TransId { get; private set; }
    public string Request { get; private set; }
    public string? Response { get; private set; }
    public string? Callback { get; private set; }
    public FreewillRequestType Type { get; private set; }
    public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public FreewillRequestLog SetCallback(string callback)
    {
        Callback = callback;
        return this;
    }
}