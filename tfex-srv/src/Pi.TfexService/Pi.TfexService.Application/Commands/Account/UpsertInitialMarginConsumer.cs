using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Domain.Models.InitialMargin;

namespace Pi.TfexService.Application.Commands.Account;

public class UpsertInitialMarginConsumer(
    IInitialMarginRepository initialMarginRepository,
    ILogger<UpsertInitialMarginConsumer> logger
) : IConsumer<UpsertInitialMargin>
{
    private const double ImOutrightMultiplier = 1.75;
    private const double ImSpreadMultiplier = 0.25;

    private static readonly Dictionary<string, string> SymbolMap = new() { { "SET50", "S50" } };

    public async Task Consume(ConsumeContext<UpsertInitialMargin> context)
    {
        try
        {
            // calculate the initial margin outright + spread
            await initialMarginRepository.UpsertInitialMargin(
                context.Message.ImData.Select(data => new InitialMargin
                {
                    Symbol = SymbolMap.TryGetValue(data.Symbol, out var modifiedSymbol) ? modifiedSymbol : data.Symbol,
                    ProductType = "FUT",
                    Im = data.Im,
                    ImOutright = data.Im * (decimal)ImOutrightMultiplier,
                    ImSpread = (data.Im * (decimal)ImOutrightMultiplier) * (decimal)ImSpreadMultiplier,
                    AsOfDate = DateOnly.FromDateTime(context.Message.AsOfDate)
                }).ToList(), context.CancellationToken);

            await context.RespondAsync(new UpsertInitialMarginResponse(true));
        }
        catch (Exception e)
        {
            logger.LogError("Failed to upsert initial margin: {Error}", e);
            throw;
        }
    }
}
