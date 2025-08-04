using Pi.Financial.FundService.Application.Models.Enums;

namespace Pi.Financial.FundService.Application.Models.Customer;

public record Address(string No,
    string SubDistrict,
    string District,
    string Province,
    string PostalCode,
    Country Country)
{
    public string? Floor { get; init; }
    public string? Building { get; init; }
    public string? RoomNo { get; init; }
    public string? Soi { get; init; }
    public string? Road { get; init; }
    public string? Moo { get; init; }
}
