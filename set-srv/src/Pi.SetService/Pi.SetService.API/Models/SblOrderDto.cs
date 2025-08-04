using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pi.SetService.API.Models;

public record SblOrderSubmitReviewRequest
{
    [Required]
    public required SblSubmitReviewStatus Status { get; init; }
    [Required]
    public required Guid ReviewerId { get; init; }
    public string? RejectedReason { get; init; }
}

public enum SblSubmitReviewStatus
{
    [Description("Approved")]
    Approved,
    [Description("Rejected")]
    Rejected
}
