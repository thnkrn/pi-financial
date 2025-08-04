using System.ComponentModel.DataAnnotations;

namespace Pi.Financial.FundService.API.Models;

public class FundAccountExistenceRequest
{
    [Required]
    public required string IdentificationCardNo { get; set; }
    public string? PassportCountry { get; set; }
}
