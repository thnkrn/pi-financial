using MassTransit;
using Pi.Common.Generators.Number;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Commands;

public record GenerateOrderNo(Guid CorrelationId, OrderSide OrderSide, bool Recurring = false);

public record OrderNoGenerated(Guid CorrelationId, string OrderNo);

public class GenerateOrderNumberConsumer : IConsumer<GenerateOrderNo>
{
    private readonly INumberGeneratorService _numberGeneratorService;
    private readonly IFundOrderRepository _fundOrderRepository;
    private const string Module = "fund_order";
    private const string TimeZone = "Asia/Bangkok";

    public GenerateOrderNumberConsumer(INumberGeneratorService numberGeneratorService,
        IFundOrderRepository fundOrderRepository)
    {
        _numberGeneratorService = numberGeneratorService;
        _fundOrderRepository = fundOrderRepository;
    }

    public async Task Consume(ConsumeContext<GenerateOrderNo> context)
    {
        var updateFunc = async (string number) =>
        {
            await _fundOrderRepository.UpdateOrderNoAsync(context.Message.CorrelationId, number,
                context.CancellationToken);
        };

        var settings = context.Message.OrderSide switch
        {
            OrderSide.Buy => new NumberGeneratorSettings(Module, context.Message.Recurring ? "FODCASUB" : "FOSUB", true,
                TimeZoneInfo.FindSystemTimeZoneById(TimeZone)),
            OrderSide.Sell => new NumberGeneratorSettings(Module, "FORED", true, TimeZoneInfo.FindSystemTimeZoneById(TimeZone)),
            OrderSide.Switch => new NumberGeneratorSettings(Module, "FOSW", true, TimeZoneInfo.FindSystemTimeZoneById(TimeZone)),
            _ => throw new ArgumentOutOfRangeException(nameof(context.Message.OrderSide), context.Message.OrderSide, null)
        };

        var orderNo = await _numberGeneratorService.GenerateAndUpdateAsync(updateFunc, settings, context.CancellationToken);

        await context.RespondAsync(new OrderNoGenerated(context.Message.CorrelationId, orderNo));
    }
}
