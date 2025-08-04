namespace Pi.Financial.FundService.Application.Models.Trading;

public record SwitchInfo
{
    public required decimal MinSwitchUnit { get; init; }
    public required decimal MinSwitchAmount { get; init; }
}
