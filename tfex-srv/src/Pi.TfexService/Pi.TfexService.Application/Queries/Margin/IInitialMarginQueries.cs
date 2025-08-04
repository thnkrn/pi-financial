using Pi.TfexService.Application.Models;

namespace Pi.TfexService.Application.Queries.Margin;

public record InitialMarginDto(
    string Symbol,
    string ProductType,
    decimal Im,
    decimal ImOutright,
    decimal ImSpread);

public interface IInitialMarginQueries
{
    public Task<InitialMarginDto> GetInitialMargin(string series, CancellationToken cancellationToken);
    public Task<decimal> GetEstRequiredInitialMargin(string accountCode, Side side, string series, int placingUnit, CancellationToken cancellationToken);
}