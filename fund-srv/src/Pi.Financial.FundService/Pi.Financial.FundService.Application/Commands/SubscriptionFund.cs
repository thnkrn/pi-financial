using MassTransit;
using MassTransit.Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Trading;
using Pi.Financial.FundService.Application.Options;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.MarketService;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Commands;

public record SubscriptFund
{
    public required Guid CorrelationId { get; init; }
    public required string OrderNo { get; init; }
    public required string FundCode { get; init; }
    public required DateOnly EffectiveDate { get; init; }
    public required string BankAccount { get; init; }
    public required Channel Channel { get; init; }
    public required string BankCode { get; init; }
    public required string TradingAccountNo { get; init; }
    public required decimal Amount { get; init; }
    public required PaymentType PaymentType { get; init; }
    public required string? UnitHolderId { get; init; }
}

public record SubscriptionFundOrder
{
    public required string TransactionId { get; init; }
    public required string SaOrderReferenceNo { get; init; }
    public required bool NewUnitHolder { get; init; }
    public required string UnitHolderId { get; init; }
    public required UnitHolderType UnitHolderType { get; init; }
    public required string SaleLicense { get; init; }
}

public class SubscriptionFundConsumer : IConsumer<SubscriptFund>
{
    private readonly IFundConnextService _fundConnextService;
    private readonly IMarketService _marketservice;
    private readonly IOptions<FundTradingOptions> _options;
    private readonly ILogger<SubscriptionFundConsumer> _logger;
    private readonly IMediator _mediator;

    public SubscriptionFundConsumer(ILogger<SubscriptionFundConsumer> logger,
        IFundConnextService fundConnextService,
        IMarketService marketService,
        IOptions<FundTradingOptions> options,
        IMediator mediator)
    {
        _logger = logger;
        _fundConnextService = fundConnextService;
        _marketservice = marketService;
        _options = options;
        _mediator = mediator;
    }
    public async Task Consume(ConsumeContext<SubscriptFund> context)
    {
        var payload = context.Message;
        var fundInfo = await _marketservice.GetFundInfoByFundCodeAsync(payload.FundCode, context.CancellationToken);

        if (fundInfo == null)
        {
            _logger.LogError("Fund info cannot be found: {FundCode}", payload.FundCode);
            throw new FundOrderException(FundOrderErrorCode.FOE101);
        }

        if (fundInfo.FirstMinBuyAmount > _options.Value.MinBuyAmountLimit || fundInfo.NextMinBuyAmount > _options.Value.MinBuyAmountLimit)
        {
            _logger.LogError("Fund min buy amount exceed limit");
            throw new FundOrderException(FundOrderErrorCode.FOE003);
        }

        var fundUnitHolderType = UnitHolderUtils.GetUnitHolderType(_options.Value.SegTaxTypes, fundInfo);
        CustomerAccountUnitHolder? unitHolder;
        var customerAccount =
            await _fundConnextService.GetCustomerAccountByAccountNoAsync(payload.TradingAccountNo,
                context.CancellationToken);
        if (customerAccount == null || string.IsNullOrEmpty(customerAccount.IcLicense))
        {
            _logger.LogError("Sale license cannot be found");
            throw new FundOrderException(FundOrderErrorCode.FOE103);
        }

        if (!fundInfo.VerifyCustomerAccess(customerAccount))
        {
            _logger.LogError("Customer can't buy non retail fund");
            throw new FundOrderException(FundOrderErrorCode.FOE111);
        }

        if (payload.UnitHolderId == null)
        {
            unitHolder = UnitHolderUtils.GetUnitholder(
                customerAccount.CustomerAccountUnitHolders.Where(uh =>
                    string.Equals(uh.AmcCode, fundInfo.AmcCode, StringComparison.CurrentCultureIgnoreCase)).ToList(),
                fundUnitHolderType);
        }
        else
        {
            unitHolder = customerAccount.CustomerAccountUnitHolders.FirstOrDefault(uh => uh.UnitHolderId == payload.UnitHolderId);
            if (unitHolder == null)
            {
                _logger.LogError("Unit Holder not found");
                throw new FundOrderException(FundOrderErrorCode.FOE106);
            }
        }

        var unitHolderType = GetUnitHolderType(_options, payload.TradingAccountNo, fundUnitHolderType, unitHolder);
        var subsciptionRequest = new CreateSubscriptionRequest
        {
            SaOrderReferenceNo = payload.OrderNo,
            SaCode = _options.Value.SaCode,
            TransactionDateTime = DateUtils.GetThDateTimeNow(),
            AccountId = payload.TradingAccountNo,
            UnitholderId = unitHolder?.UnitHolderId ?? unitHolderType.ToString(),
            OverrideRiskProfile = true,
            OverrideFxRisk = true,
            FundCode = payload.FundCode,
            Amount = payload.Amount,
            EffectiveDate = payload.EffectiveDate,
            BankCode = payload.BankCode,
            BankAccount = payload.BankAccount,
            Channel = payload.Channel,
            SaleLicense = customerAccount.IcLicense,
            ForceEntry = false,
            PaymentType = payload.PaymentType
        };
        try
        {
            await ProcessRequest(
                context,
                subsciptionRequest,
                unitHolder == null,
                unitHolderType,
                customerAccount.IcLicense);
        }
        catch (FundOrderException e) when (e.Code == FundOrderErrorCode.FOE214)
        {
            await _mediator.Send(new SyncCustomerData(payload.TradingAccountNo.Split("-")[0], context.CorrelationId ?? Guid.NewGuid(), payload.BankAccount), context.CancellationToken);
            await ProcessRequest(
                context,
                subsciptionRequest,
                unitHolder == null,
                unitHolderType,
                customerAccount.IcLicense);
        }
    }

    private async Task ProcessRequest(ConsumeContext<SubscriptFund> context, CreateSubscriptionRequest subsciptionRequest, bool isNewUnitHolder, UnitHolderType unitHolderType, string icLicense)
    {
        var response = await _fundConnextService.CreateSubscriptionAsync(subsciptionRequest, context.CancellationToken);

        await context.RespondAsync(new SubscriptionFundOrder
        {
            TransactionId = response.TransactionId,
            SaOrderReferenceNo = response.SaOrderReferenceNo,
            NewUnitHolder = isNewUnitHolder,
            UnitHolderId = response.UnitHolderId,
            UnitHolderType = unitHolderType,
            SaleLicense = icLicense
        });
    }

    private static UnitHolderType GetUnitHolderType(IOptions<FundTradingOptions> options, string tradingAccountNo, UnitHolderType fundUnitHolderType, CustomerAccountUnitHolder? unitHolder)
    {
        if (unitHolder != null)
        {
            return unitHolder.UnitHolderType;
        }
        return tradingAccountNo.EndsWith(options.Value.SegAccountSuffix) ? UnitHolderType.SEG : fundUnitHolderType;
    }
}
