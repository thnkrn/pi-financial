namespace Pi.Financial.FundService.Domain.Events;

public record CancelOrderFailed(string BrokerOrderId, string ErrMessage, string? ErrorCode = null);
