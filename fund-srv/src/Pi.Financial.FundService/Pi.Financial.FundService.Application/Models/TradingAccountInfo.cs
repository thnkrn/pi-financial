using Pi.Financial.FundService.Application.Models.Bank;

namespace Pi.Financial.FundService.Application.Models;

public record TradingAccountInfo
{
    public required Guid Id { get; init; }
    public required string AccountNo { get; init; }
    public required string SaleLicense { get; init; }
    public DateOnly? OpenDate { get; init; }
}
