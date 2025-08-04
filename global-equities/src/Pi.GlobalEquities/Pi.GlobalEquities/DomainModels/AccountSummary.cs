using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.DomainModels;

public class AccountSummary
{
    public Currency Currency { get; init; }
    public required decimal NetAssetValue { get; init; }
    public required decimal TotalMarketValue { get; init; }
    public required decimal TotalCost { get; init; }
    public required decimal TotalUpnl { get; init; }
    public required decimal LineAvailable { get; init; }
    public required decimal WithdrawableCash { get; init; }
    public decimal TotalUpnlPercentage => TotalCost == 0m
            ? 0m
            : 100 * TotalUpnl / TotalCost;

    private readonly IEnumerable<Position> _positions;
    public required IEnumerable<Position> Positions
    {
        get => _positions;
        init => _positions = value ?? Enumerable.Empty<Position>();
    }
}


