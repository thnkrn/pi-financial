using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Common.Generators.Number;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Services.NumberGeneratorService;
using Pi.SetService.Application.Services.OneportService;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using Pi.SetService.Domain.Events;

namespace Pi.SetService.Application.Commands;

public record CreateSblOrder
{
    public required Guid Id { get; init; }
    public required TradingAccount TradingAccount { get; init; }
    public required string Symbol { get; init; }
    public required int Volume { get; init; }
    public required SblOrderType Type { get; init; }
};

public class CreateSblOrderConsumer(
    IInstrumentRepository instrumentRepository,
    ISblOrderRepository sblOrderRepository,
    IEquityNumberGeneratorService numberGeneratorService,
    IOnePortService onePortService,
    ILogger<CreateSblOrderConsumer> logger)
    : IConsumer<CreateSblOrder>
{
    public async Task Consume(ConsumeContext<CreateSblOrder> context)
    {
        try
        {
            if (!context.Message.TradingAccount.SblEnabled)
            {
                throw new SetException(SetErrorCode.SE115);
            }

            var sblInstrument = await instrumentRepository.GetSblInstrument(context.Message.Symbol, context.CancellationToken);
            if (sblInstrument == null)
            {
                throw new SetException(SetErrorCode.SE111);
            }

            switch (context.Message.Type)
            {
                case SblOrderType.Borrow:
                    if (sblInstrument.AvailableLending < context.Message.Volume)
                    {
                        throw new SetException(SetErrorCode.SE112);
                    }

                    sblInstrument.Borrow(context.Message.Volume);
                    break;
                case SblOrderType.Return:
                    await ValidateReturn(context);
                    sblInstrument.Return(context.Message.Volume);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(context.Message.Type), context.Message.Type,
                        string.Empty);
            }

            var orderId = await numberGeneratorService.GenerateSblOrderOrderIdAsync(context.CancellationToken);
            var sblOrder = new SblOrder(context.Message.Id, context.Message.TradingAccount, orderId, context.Message.Symbol, context.Message.Volume, context.Message.Type);

            var result = await CreateSblOrder(sblOrder, context.CancellationToken);

            await UpdateSblInstrument(context, sblInstrument, result);
            await context.RespondAsync(new PlaceOrderSuccess(result.OrderId.ToString(), result.OrderId.ToString()));
        }
        catch (SetException e)
        {
            logger.LogError(e, "Can't create SBL order cause error {Code} ({Msg})", e.Code, e.Message);
            await context.RespondAsync(new PlaceOrderFailed(context.Message.Id, e.Code, e.Message, null));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Can't create SBL order");
            await context.RespondAsync(new PlaceOrderFailed(context.Message.Id, SetErrorCode.SE113, e.Message, null));
        }
    }

    private async Task UpdateSblInstrument(ConsumeContext<CreateSblOrder> context, SblInstrument sblInstrument, SblOrder result)
    {
        try
        {
            instrumentRepository.UpdateSblInstrument(sblInstrument);
            await instrumentRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception)
        {
            await sblOrderRepository.DeleteAsync(result, context.CancellationToken);
            throw;
        }
    }

    private async Task<SblOrder> CreateSblOrder(SblOrder sblOrder, CancellationToken ct = default)
    {
        try
        {
            return await sblOrderRepository.CreateAsync(sblOrder, ct);
        }
        catch (DuplicateRecordNoException)
        {
            var orderId = await numberGeneratorService.GenerateSblOrderOrderIdAsync(ct);
            sblOrder.UpdateOrderId(orderId);
            return await CreateSblOrder(sblOrder, ct);
        }
    }

    private async Task ValidateReturn(ConsumeContext<CreateSblOrder> context)
    {
        var positions = await onePortService.GetPositions(context.Message.TradingAccount.TradingAccountNo, context.CancellationToken);

        var position = positions.Find(q => q.StockType is StockType.Borrow && q.SecSymbol == context.Message.Symbol);

        if (position == null)
        {
            throw new SetException(SetErrorCode.SE114);
        }

        if (position.AvailableVolume < context.Message.Volume)
        {
            throw new SetException(SetErrorCode.SE106);
        }
    }
}
