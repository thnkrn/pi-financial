using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Services.MarketService;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.Financial.FundService.Application.Commands;

public class CreateUnitHolder
{
    public required string? CustomerCode { get; init; }
    public required string TradingAccountNo { get; init; }
    public required string UnitHolderId { get; init; }
    public required string FundCode { get; init; }
    public required UnitHolderType UnitHolderType { get; init; }
}

public class CreateUnitHolderConsumer : IConsumer<CreateUnitHolder>
{
    private readonly IUnitHolderRepository _unitHolderRepository;
    private readonly IMarketService _marketService;
    private readonly ILogger<CreateUnitHolderConsumer> _logger;

    public CreateUnitHolderConsumer(ILogger<CreateUnitHolderConsumer> logger, IUnitHolderRepository unitHolderRepository, IMarketService marketService)
    {
        _unitHolderRepository = unitHolderRepository;
        _marketService = marketService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CreateUnitHolder> context)
    {
        var fundInfo = await _marketService.GetFundInfoByFundCodeAsync(context.Message.FundCode, context.CancellationToken);
        if (fundInfo == null)
        {
            _logger.LogError("Fund Info not found: {FundInfoSymbol}", context.Message.FundCode);
            throw new RecordNotFoundException();
        }

        var unitHolder = new UnitHolder(Guid.NewGuid(),
            context.Message.TradingAccountNo,
            fundInfo.AmcCode,
            context.Message.UnitHolderId,
            context.Message.UnitHolderType,
            UnitHolderStatus.Normal)
        {
            CustomerCode = context.Message.CustomerCode
        };
        await _unitHolderRepository.CreateAsync(unitHolder, context.CancellationToken);
        await _unitHolderRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}
