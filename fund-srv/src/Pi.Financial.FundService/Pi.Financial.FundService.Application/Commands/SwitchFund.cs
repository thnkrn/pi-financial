using MassTransit;
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

public record SwitchingFund
{
    public required Guid CorrelationId { get; init; }
    public required string OrderNo { get; init; }
    public required string FundCode { get; init; }
    public required string CounterFundCode { get; init; }
    public required DateOnly EffectiveDate { get; init; }
    public required string BankAccount { get; init; }
    public required string UnitHolderId { get; init; }
    public required string BankCode { get; init; }
    public required Channel Channel { get; init; }
    public required string TradingAccountNo { get; init; }
    public required decimal? Amount { get; init; }
    public required decimal? Unit { get; init; }
    public required RedemptionType RedemptionType { get; init; }
    public required bool? SellAllFlag { get; init; }
}

public record SwitchingFundOrder
{
    public required string TransactionId { get; init; }
    public required string SaOrderReferenceNo { get; init; }
    public required string UnitHolderId { get; init; }
    public required bool SellAllFlag { get; init; }
    public required string SaleLicense { get; init; }
}

public class SwitchFundConsumer : IConsumer<SwitchingFund>
{
    private readonly IFundConnextService _fundConnextService;
    private readonly IMarketService _marketservice;
    private readonly IOptions<FundTradingOptions> _options;
    private readonly IUnitHolderRepository _unitHolderRepository;
    private readonly ILogger<SwitchFundConsumer> _logger;


    public SwitchFundConsumer(ILogger<SwitchFundConsumer> logger,
        IFundConnextService fundConnextService,
        IMarketService marketService,
        IUnitHolderRepository unitHolderRepository,
        IOptions<FundTradingOptions> options)
    {
        _logger = logger;
        _fundConnextService = fundConnextService;
        _marketservice = marketService;
        _unitHolderRepository = unitHolderRepository;
        _options = options;
    }

    public async Task Consume(ConsumeContext<SwitchingFund> context)
    {
        var payload = context.Message;
        ValidatePayload(payload);

        var fundInfos = await _marketservice.GetFundInfosAsync(new[] { payload.FundCode, payload.CounterFundCode }, context.CancellationToken);
        var infos = fundInfos.ToArray();

        var fundInfo = infos.FirstOrDefault(q => string.Equals(q.FundCode, payload.FundCode, StringComparison.CurrentCultureIgnoreCase));
        if (fundInfo == null || infos.FirstOrDefault(q => string.Equals(q.FundCode, payload.CounterFundCode, StringComparison.CurrentCultureIgnoreCase)) == null)
        {
            _logger.LogError("Fund info not found: {Symbol}", payload.FundCode);
            throw new FundOrderException(FundOrderErrorCode.FOE101);
        }

        var assets = await _fundConnextService.GetAccountBalanceAsync(payload.TradingAccountNo, context.CancellationToken);
        var fundAsset = assets.FirstOrDefault(f =>
            string.Equals(f.FundCode, payload.FundCode, StringComparison.CurrentCultureIgnoreCase) &&
            f.UnitholderId == payload.UnitHolderId);

        if (fundAsset == null)
        {
            _logger.LogError("Fund Asset cannot be found");
            throw new FundOrderException(FundOrderErrorCode.FOE107);
        }

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
            _logger.LogError("Customer can't switch non retail fund");
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

        var sellAllFlag = GetSwitchSellAllUnitFlag(payload, fundAsset);
        var response = await _fundConnextService.CreateSwitchAsync(new CreateSwitchRequest
        {
            SaOrderReferenceNo = payload.OrderNo,
            SaCode = _options.Value.SaCode,
            TransactionDateTime = DateUtils.GetThDateTimeNow(),
            AccountId = payload.TradingAccountNo,
            UnitholderId = fundAsset.UnitholderId,
            FundCode = payload.FundCode,
            CounterFundCode = payload.CounterFundCode,
            Amount = payload.RedemptionType == RedemptionType.Amount ? (decimal)payload.Amount! : 0m,
            Unit = payload.RedemptionType == RedemptionType.Unit ? (decimal)payload.Unit! : 0m,
            EffectiveDate = payload.EffectiveDate,
            Channel = payload.Channel,
            OverrideRiskProfile = true,
            OverrideFxRisk = true,
            IcLicense = customerAccount.IcLicense,
            RedemptionType = payload.RedemptionType,
            ForceEntry = false,
            SellAllUnitFlag = sellAllFlag,
        }, context.CancellationToken);

        await context.RespondAsync(new SwitchingFundOrder
        {
            TransactionId = response.TransactionId,
            SaOrderReferenceNo = response.SaOrderReferenceNo,
            UnitHolderId = fundAsset.UnitholderId,
            SellAllFlag = sellAllFlag,
            SaleLicense = customerAccount.IcLicense
        });
    }

    private void ValidatePayload(SwitchingFund payload)
    {
        if (payload is { RedemptionType: RedemptionType.Unit, Unit: null } || payload.Unit <= 0)
        {
            _logger.LogError("Switch unit can't be null {Unit}", payload.Unit);
            throw new FundOrderException(FundOrderErrorCode.FOE002);
        }

        if (payload is { RedemptionType: RedemptionType.Amount, Amount: null } || payload.Amount <= 0)
        {
            _logger.LogError("Switch amount can't be null {Amount}", payload.Amount);
            throw new FundOrderException(FundOrderErrorCode.FOE002);
        }
    }

    private static bool GetSwitchSellAllUnitFlag(SwitchingFund payload, FundAssetResponse fundAsset)
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
}
