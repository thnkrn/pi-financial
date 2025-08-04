using System.ComponentModel.DataAnnotations;

namespace Pi.StructureNotes.API.Models;

public class GetAccountNotesRequest
{
    [Required]
    [FromRoute(Name = "accountId")]
    public string AccountId { get; init; }

    [Required]
    [FromRoute(Name = "currency")]
    public string Currency { get; init; }
}
