namespace Pi.BackofficeService.Application.Models.Sbl;

public record SubmitReview
{
    public required Guid Id { get; init; }
    public required SblOrderStatus SblOrderStatus { get; init; }
    public required Guid ReviewerId { get; init; }
    public string? RejectedReason { get; init; }
}
