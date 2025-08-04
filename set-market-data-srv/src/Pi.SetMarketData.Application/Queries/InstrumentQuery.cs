using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Queries;

public record InstrumentInfo
{
    public required string Symbol { get; init; }
    public required string Market { get; init; }
    public required string MarketStatus { get; init; }
    public required Profile Profile { get; init; }
    public required PricingDetail PricingDetail { get; init; }
    public required IEnumerable<CorporateActionDetail> CorporateActions { get; init; }
}

public record Profile
{
    public required string Symbol { get; init; }
    public required string Logo { get; init; }
    public required string FriendlyName { get; init; }
    public required string InstrumentCategory { get; init; }
}

public record PricingDetail
{
    public required string? Price { get; init; }
    public required string? PrevClose { get; init; }
    public required string? Ceiling { get; init; }
    public required string? Floor { get; init; }
}

public record CorporateActionDetail
{
    public required string Date { get; init; }
    public required string Code { get; init; }
}

public record InstrumentQueryParam(string Symbol, string Venue);

public interface IInstrumentQuery
{
    Task<IEnumerable<MarketStreamingResponse>> GetStreamingData(string[] symbols);
    Task<IEnumerable<InstrumentInfo>> GetInstrumentsInfo(InstrumentQueryParam[] queryParams);
}
