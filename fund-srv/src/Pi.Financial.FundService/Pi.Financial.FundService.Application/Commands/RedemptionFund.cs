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
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Commands;

public record RedeemFund
{
    public required Guid CorrelationId { get; init; }
    public required string OrderNo { get; init; }
    public required string FundCode { get; init; }
    public required DateOnly EffectiveDate { get; init; }
    public required string BankAccount { get; init; }
    public required string BankCode { get; init; }
    public required string TradingAccountNo { get; init; }
    public required Channel Channel { get; init; }
    public required string UnitHolderId { get; init; }
    public required decimal? Amount { get; init; }
    public required decimal? Unit { get; init; }
    public required RedemptionType RedemptionType { get; init; }
    public required bool? SellAllFlag { get; init; }
}

public record RedemptionFundOrder
{
    public required string TransactionId { get; init; }
    public required string UnitHolderId { get; init; }
    public required string SaOrderReferenceNo { get; init; }
    public required DateOnly SettlementDate { get; init; }
    public required PaymentType PaymentType { get; init; }
    public required bool SellAllFlag { get; init; }
    public required string SaleLicense { get; init; }
}

public class RedemptionFundConsumer : IConsumer<RedeemFund>
{
    private readonly IFundConnextService _fundConnextService;
    private readonly IMarketService _marketservice;
    private readonly IOptions<FundTradingOptions> _options;
    private readonly ILogger<RedemptionFundConsumer> _logger;
    private readonly IMediator _mediator;

    public RedemptionFundConsumer(ILogger<RedemptionFundConsumer> logger,
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

    public async Task Consume(ConsumeContext<RedeemFund> context)
    {
        var payload = context.Message;

        ValidatePayload(payload);
        var fundInfo = await _marketservice.GetFundInfoByFundCodeAsync(payload.FundCode, context.CancellationToken);

        if (fundInfo == null)
        {
            _logger.LogError("Fund info not found: {Symbol}", payload.FundCode);
            throw new FundOrderException(FundOrderErrorCode.FOE101);
        }

        var assets = await _fundConnextService.GetAccountBalanceAsync(payload.TradingAccountNo, context.CancellationToken);
        var fundAsset = GetFundAsset(assets, payload.FundCode, payload.UnitHolderId);

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
            _logger.LogError("Customer can't sell non retail fund");
            throw new FundOrderException(FundOrderErrorCode.FOE111);
        }

        if (customerAccount.CustomerAccountUnitHolders.Count == 0)
        {
            _logger.LogError("Customer Account Unit Holders are not found");
            throw new FundOrderException(FundOrderErrorCode.FOE106);
        }

        unitHolder = customerAccount.CustomerAccountUnitHolders.FirstOrDefault(uh => uh.UnitHolderId == payload.UnitHolderId);
        if (unitHolder == null)
        {
            _logger.LogError("Unit Holder not found");
            throw new FundOrderException(FundOrderErrorCode.FOE106);
        }

        if ((payload.RedemptionType == RedemptionType.Unit && fundAsset.RemainUnit < payload.Unit) ||
            payload.RedemptionType == RedemptionType.Amount && fundAsset.RemainAmount < payload.Amount)
        {
            _logger.LogError("Invalid Quantity for Redemption");
            throw new FundOrderException(FundOrderErrorCode.FOE004);
        }

        var paymentType = UnitHolderUtils.GetPaymentTypeFromUnitHolderType(unitHolder.UnitHolderType);
        var sellAllFlag = GetRedemptionSellAllUnitFlag(payload, fundAsset);
        var redemptionRequest = new CreateRedemptionRequest
        {
            SaOrderReferenceNo = payload.OrderNo,
            SaCode = _options.Value.SaCode,
            TransactionDateTime = DateUtils.GetThDateTimeNow(),
            AccountId = payload.TradingAccountNo,
            UnitholderId = unitHolder.UnitHolderId,
            FundCode = payload.FundCode,
            Amount = payload.RedemptionType == RedemptionType.Amount ? (decimal)payload.Amount! : 0,
            Unit = payload.RedemptionType == RedemptionType.Unit ? (decimal)payload.Unit! : 0,
            EffectiveDate = payload.EffectiveDate,
            BankCode = payload.BankCode,
            BankAccount = payload.BankAccount,
            Channel = payload.Channel,
            IcLicense = customerAccount.IcLicense,
            RedemptionType = payload.RedemptionType,
            ForceEntry = false,
            SellAllUnitFlag = sellAllFlag,
            PaymentType = paymentType,
        };

        try
        {
            await ProcessRequest(context, redemptionRequest, unitHolder.UnitHolderId, paymentType, sellAllFlag,
                customerAccount.IcLicense);
        }
        catch (FundOrderException e) when (e.Code == FundOrderErrorCode.FOE214)
        {
            await _mediator.Send(new SyncCustomerData(payload.TradingAccountNo.Split("-")[0], context.CorrelationId ?? Guid.NewGuid(), payload.BankAccount), context.CancellationToken);
            await ProcessRequest(context, redemptionRequest, unitHolder.UnitHolderId, paymentType, sellAllFlag,
                customerAccount.IcLicense);
        }
    }

    private async Task ProcessRequest(ConsumeContext<RedeemFund> context, CreateRedemptionRequest redemptionRequest, string unitHolderId, PaymentType paymentType, bool isSellAll, string icLicense)
    {
        var response = await _fundConnextService.CreateRedemptionAsync(redemptionRequest, context.CancellationToken);

        await context.RespondAsync(new RedemptionFundOrder
        {
            TransactionId = response.TransactionId,
            UnitHolderId = unitHolderId,
            SaOrderReferenceNo = response.SaOrderReferenceNo,
            SettlementDate = response.SettlementDate,
            PaymentType = paymentType,
            SellAllFlag = isSellAll,
            SaleLicense = icLicense
        });
    }

    private void ValidatePayload(RedeemFund payload)
    {
        if (payload is { RedemptionType: RedemptionType.Unit, Unit: null } || payload.Unit <= 0)
        {
            _logger.LogError("Redemption unit can't be null {Unit}", payload.Unit);
            throw new FundOrderException(FundOrderErrorCode.FOE002);
        }

        if (payload is { RedemptionType: RedemptionType.Amount, Amount: null } || payload.Amount <= 0)
        {
            _logger.LogError("Redemption amount can't be null {Amount}", payload.Amount);
            throw new FundOrderException(FundOrderErrorCode.FOE002);
        }
    }

    private static bool GetRedemptionSellAllUnitFlag(RedeemFund payload, FundAssetResponse fundAsset)
    {
        if (payload.SellAllFlag != null)
        {
            return payload is { SellAllFlag: true, RedemptionType: RedemptionType.Unit };
        }

        return payload.RedemptionType switch
        {
            RedemptionType.Unit when fundAsset.RemainUnit == payload.Unit => true,
            _ => false
        };
    }

    private FundAssetResponse GetFundAsset(IEnumerable<FundAssetResponse> assets, string fundCode, string unitHolderId)
    {
        var fundAsset = assets.FirstOrDefault(f =>
            string.Equals(f.FundCode, fundCode, StringComparison.CurrentCultureIgnoreCase) &&
            f.UnitholderId == unitHolderId);

        if (fundAsset != null)
        {
            return fundAsset;
        }

        _logger.LogError("Fund Asset cannot be found");
        throw new FundOrderException(FundOrderErrorCode.FOE107);
    }
}
