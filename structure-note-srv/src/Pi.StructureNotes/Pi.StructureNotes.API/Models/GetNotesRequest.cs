using System.ComponentModel.DataAnnotations;

namespace Pi.StructureNotes.API.Models;

//public record GetNotesRequest([FromRoute,Required] string Currency);

public class GetNotesRequest
{
    [Required]
    [FromRoute(Name = "currency")]
    public string Currency { get; set; }
}
