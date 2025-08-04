using Pi.FundMarketData.API.Models.Filters;

namespace Pi.FundMarketData.API.Models.Responses;

public record FilterResponse
{
    public required string FilterCategory { get; init; }
    public required string FilterKey { get; init; }
    public required FilterType FilterType { get; init; } // TODO: [TBC] check this field again when FE start using this response
    public required FilterOption[] Filters { get; init; }
    public required int Order { get; init; }
    public bool SupportSecondaryFilter { get => FilterType == FilterType.Primary; } // TODO: [TBC] check this field again when FE start using this response
}

public record FilterOption
{
    public required string FilterName { get; init; }
    public required string Value { get; init; }
    public required int Order { get; init; }
    public required bool IsDefault { get; set; } // TODO: [TBC] check this field again when FE start using this response
    public bool IsHighlight { get; set; } = false; // TODO: [TBC] check this field again when FE start using this response
}
