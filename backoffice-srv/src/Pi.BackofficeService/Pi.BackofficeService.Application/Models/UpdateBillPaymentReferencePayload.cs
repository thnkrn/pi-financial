using System.Text.Json.Serialization;

namespace Pi.BackofficeService.Application.Models;

public record UpdateBillPaymentReferencePayload
{
    [JsonPropertyName("oldReference")]
    public required string OldReference { get; init; }
    [JsonPropertyName("newReference")]
    public required string NewReference { get; init; }
}
