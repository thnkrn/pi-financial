namespace Pi.BackofficeService.API.Models;

public record SubmitReviewSblOrderRequest
{
    public required SblReviewOrderStatus Status { get; init; }
    public string? RejectedReason { get; init; }
}

public enum SblReviewOrderStatus
{
    Approved,
    Rejected
}
