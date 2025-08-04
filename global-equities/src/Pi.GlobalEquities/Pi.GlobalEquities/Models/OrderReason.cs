using System.Text.Json.Serialization;

namespace Pi.GlobalEquities.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderReason
{
    Unknown = 0,
    InsufficientFund,
    IncorrectQuantity,
    InvalidPrice,
    OperationRejected,
    WaitingParentExecution,
    WaitingNextTradingSession,
}
